using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

enum MENU{
	NONE = 0,
	SKIN,
	WEAPON,
	COLOR,
	OVER
}

[System.Serializable]
public class PlayerCustomOption{
	[SerializeField] public string name;
	[SerializeField] public Sprite image;
}
	
[System.Serializable]
public class Player_AvatarOption : PlayerCustomOption{
	[SerializeField] public string playerController;
}
	
[System.Serializable]
public class Player_WeaponOption : PlayerCustomOption{
	[SerializeField] public GameObject playerWeapon;
}

public class RoomController_Refactored : MonoBehaviour {
	[SerializeField] int id;
	[SerializeField] GameObject playerPrefab;
	[SerializeField] GameObject myTutorial;
	[SerializeField] GameObject myPressA;
	[SerializeField] GameObject[] myInfoGameObjects;
	[SerializeField] Text nameText;
	[SerializeField] Player_WeaponOption[] playerWeapons;
	[SerializeField] Player_AvatarOption[] playerAvatars;
	[SerializeField] Color[] playerColors;
	[SerializeField] GameObject wallCanvas;
	[SerializeField] AudioClip clipSwitch,clipSelect,clipDeselect;

	GameObject currentWeapon;
	string currentAvatar;
	Color currentColor;
	GameObject player;

	PlayerCustomOption[] currentOptions;
	public static bool [] colorTaken;

	GameObject titleCanvas;
	GameObject playWall, exitWall;
	GameObject[] wallsToRemove;

	bool aPressed, bPressed, hMove, vMove;
	float h,v, hLast, vLast;

	SpriteRenderer spriteRenderer;

	static int players_total, players_ready;
	int currentMenu;
	int pointerIndex;
	Text myText;
	MENU menu = MENU.NONE;
	public static string[] globalPlayerAnimators;
	public static GameObject[] globalPlayerWeapons;


	void Awake()
	{
		players_ready = 0;
		players_total = 0;
		if (GameManager.player_in == null)
			GameManager.player_in = new bool[4];
		else {
			for (int i = 0; i < 4; i++) {
				GameManager.player_in [i] = false;
			}
		}

		if (globalPlayerAnimators == null) globalPlayerAnimators = new string[4];
		if (globalPlayerWeapons== null) globalPlayerWeapons = new GameObject[4];

		IniWallTriggers ();
		titleCanvas = GameObject.FindGameObjectWithTag ("Title");
		colorTaken = new bool[playerColors.Length];
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
	}

	void IniWallTriggers()
	{
		Transform parent = transform.Find ("Walls");
		Transform triggers = parent.Find ("Triggers");

		playWall = triggers.Find ("PlayWall").gameObject;
		exitWall = triggers.Find ("ExitWall").gameObject;

		wallsToRemove = new GameObject[2];
		switch (id)
		{
		case 0:
			wallsToRemove [0] = parent.Find ("lWall").gameObject;
			wallsToRemove [1] = parent.Find ("uWall").gameObject;
			break;								
		case 1:								
			wallsToRemove [0] = parent.Find ("rWall").gameObject;
			wallsToRemove [1] = parent.Find ("uWall").gameObject;
			break;								 
		case 2:									
			wallsToRemove [0] = parent.Find ("lWall").gameObject;
			wallsToRemove [1] = parent.Find ("dWall").gameObject;
			break;								  
		case 3:									  
			wallsToRemove [0] = parent.Find ("rWall").gameObject;
			wallsToRemove [1] = parent.Find ("dWall").gameObject;
			break;
		}
	}

	void GetInput()
	{
		aPressed = Input.GetButtonDown ("aButton_" + id);
		bPressed = Input.GetButtonDown ("bButton_" + id);

		hLast = h;
		vLast = v;

		hMove = (Mathf.Round(hLast) == 0f);
		vMove = (Mathf.Round (vLast) == 0f);

		h = Mathf.Round(Input.GetAxisRaw ("h1_" + id));
		v = Mathf.Round (Input.GetAxisRaw ("v1_" + id));
	}

	void CheckForReady()
	{
		if (players_total > 4)
			players_total = 4;
		if (players_ready > 4)
			players_ready = 4;
		if (players_total > 0 && players_total <= players_ready) {
			SceneManager.LoadScene ("ModeSelect");
		}
	}

	void Update()
	{
		GetInput ();
		UpdateMenu ();
		AdvanceMenu ();
		RetreatMenu ();
		CheckForReady ();
	}

	void UpdateMenu()
	{
		if (menu == MENU.NONE || menu == MENU.OVER)
			return;

		if (hMove) {
			if (h != 0f)
				GlobalAudioManager.Instance.PlaySound (clipSwitch);
			pointerIndex += (int)Mathf.Round (h);
		}
		
		if (menu == MENU.SKIN || menu == MENU.WEAPON) {
			if (pointerIndex >= currentOptions.Length)
				pointerIndex = 0;
			if (pointerIndex < 0)
				pointerIndex = currentOptions.Length - 1;
			int count = 0;
			while (currentOptions [pointerIndex].image == null) {
				pointerIndex += Mathf.RoundToInt(h);
				if (pointerIndex >= currentOptions.Length)
					pointerIndex = 0;
				if (pointerIndex < 0)
					pointerIndex = currentOptions.Length - 1;
				count++;
				if (count > 100) {
					pointerIndex = 0;
					break;
				}
			}
			spriteRenderer.sprite =  currentOptions [pointerIndex]. image;
			nameText.text = currentOptions [pointerIndex].name;
		} else if (menu == MENU.COLOR) {
			if (pointerIndex >= colorTaken.Length)
				pointerIndex = 0;
			if (pointerIndex < 0)
				pointerIndex = colorTaken.Length - 1;
			/*
			while (colorTaken [pointerIndex]) {
				pointerIndex += Mathf.RoundToInt(h);
				if (pointerIndex >= colorTaken.Length)
					pointerIndex = 0;
				if (pointerIndex < 0)
					pointerIndex = colorTaken.Length - 1;
			}
			*/
			spriteRenderer.color = playerColors [pointerIndex];
		}
	}

	void AdvanceMenu()
	{
		if (menu != MENU.OVER)
		if (aPressed) {
			GlobalAudioManager.Instance.PlaySound (clipSelect);
			switch (menu) {
			case MENU.NONE:
				players_total++;
				IniMenuSkin ();
				break;
			case MENU.SKIN:
				currentAvatar = playerAvatars [pointerIndex].playerController;
				globalPlayerAnimators [id] = playerAvatars [pointerIndex].playerController;
			//	IniMenuColor ();
			//	break;
			//case MENU.COLOR:
			//	SelectColor ();
				IniMenuWeapon ();
				break;
			case MENU.WEAPON:
				currentWeapon = playerWeapons [pointerIndex].playerWeapon;
				globalPlayerWeapons [id] = playerWeapons [pointerIndex].playerWeapon;
				myInfoGameObjects [0].SetActive (false);
				myInfoGameObjects [1].SetActive (false);
				CreatePlayer ();
				break;
			}

			pointerIndex = 0;
		}
	}

	void SelectColor()
	{
		colorTaken [pointerIndex] = true;
		currentColor = playerColors [pointerIndex];
	}

	void DeselectColor()
	{
		for (int i = 0; i < playerColors.Length; i++) {
			if (playerColors [i] == currentColor) {
				colorTaken [i] = false;
				break;
			}
		}
	}

	void RetreatMenu()
	{
		if (!bPressed || menu == MENU.NONE || menu == MENU.OVER)
			return;

		GlobalAudioManager.Instance.PlaySound (clipDeselect);
		switch (menu) {
		case MENU.SKIN:
			IniMenuNone ();
			break;
		case MENU.COLOR:
			IniMenuSkin ();
			break;
		case MENU.WEAPON:
			DeselectColor ();
			IniMenuColor ();
			break;
		}
		pointerIndex = 0;
	}

	void IniMenuSkin()
	{
		myInfoGameObjects [0].SetActive (true);
		myInfoGameObjects [1].SetActive (true);
		titleCanvas.SetActive (false);
		myPressA.SetActive (false);
		menu = MENU.SKIN;
		currentOptions = playerAvatars;
		spriteRenderer.color = Color.white;
	}

	void IniMenuColor()
	{
		menu = MENU.COLOR;
		nameText.text = "Choose color";
	}

	void IniMenuWeapon()
	{
		menu = MENU.WEAPON;
		currentOptions = playerWeapons;
		spriteRenderer.color = Color.white;
	}

	void IniMenuNone()
	{
		foreach (GameObject go in myInfoGameObjects) {
			go.SetActive (false);
		}
		myPressA.SetActive (true);
		menu = MENU.NONE;
		spriteRenderer.sprite = null;
		nameText.text = "";
		players_total--;
		if (players_total < 0)
			players_total = 0;
		if (players_total == 0)
			titleCanvas.SetActive (true);
		else {
			// PRESS A
		}
	}
		
	void CreatePlayer()
	{
		nameText.text = "";
		spriteRenderer.sprite = null;
		menu = MENU.OVER;
		player = Instantiate (playerPrefab, transform.position, Quaternion.identity) as GameObject;
		player.GetComponent<Base_Health> ().SetInvincibilty (true);

		player.GetComponent<PlayerController> ().IniPlayer(id,0,currentAvatar,currentWeapon);

		SetWalls (false);
	}

	public void SetWalls(bool wallsOn)
	{
		playWall.SetActive(!wallsOn);
		exitWall.SetActive (!wallsOn);
		wallCanvas.SetActive (!wallsOn);
		foreach (GameObject wall in wallsToRemove) {
			wall.SetActive (wallsOn);
		}
	}

	public void SetReady()
	{
		if (GameManager.player_in == null)GameManager.player_in = new bool[4];
		GameManager.player_in [id] = true;

		players_ready++;
		Destroy (player);
	}

	public void SetExit()
	{
		Destroy (player);
		SetWalls (true);
		IniMenuSkin ();
	}
}

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomMenuController : MonoBehaviour {
	public GameObject playerPrefab;
	public GameObject[] menus;
	public GameObject[] pointers;
	public Vector3[] pointerBase;
	GameObject player;

	int pointerPos;

	[Range(1,4)] public int id;
	[Range(-1,2)] int state = -1;
	[Range(1,2)] int option = 1;

	bool a,b;
	float v, hLast, vLast;

	Child profile;
	string newName;

	bool vMove;

	public static int players_total, players_ready = 0;

	public Text namesText, inputText;

	int namesTotal, namesPos;

	GameObject titleCanvas;
	public GameObject tutorial;

	public GameObject playWall, exitWall;
	public GameObject playWallCol,exitWallCol;

	public bool newPlayer;

	public GameObject statsObject;
	public Text statsText;
	public GameObject nameObject;

	void Start()
	{
		titleCanvas = GameObject.FindGameObjectWithTag ("Title");

		if (ChildQueueController.player_profiles [id - 1] != null) { // Enquee current child, dequeue new child.
			if (ChildQueueController.children.Count > 0) {
				// 
				state = 3;
				if (ChildQueueController.player_profiles [id - 1].plays > 0)
					ChildQueueController.children.Enqueue (ChildQueueController.player_profiles [id - 1]); // only enqueue if they can play still
				newPlayer = true;
				Child newChild = ChildQueueController.children.Dequeue ();
				ChildQueueController.player_profiles [id - 1] = newChild;
				tutorial.SetActive (true);
				tutorial.GetComponent<TutorialController> ().newChild = newChild;
				if (titleCanvas != null)
					titleCanvas.SetActive (false);
				players_total++;
				CreatePlayer ();
			} else {
				if (ChildQueueController.player_profiles [id - 1].plays > 0) {
					state = 3;
					players_total++;
					if (titleCanvas != null)
						titleCanvas.SetActive (false);
					CreatePlayer ();
					OpenSides ();
				} else {
					ChildQueueController.player_profiles [id - 1] = null;
				}
			}
		}
	}
		
	void GetInput()
	{
		a = Input.GetButtonDown ("aButton_" + id);
		b = Input.GetButtonDown ("bButton_" + id);
		if (Mathf.Round(v) == 0f)
			vMove = true;
		//h = Mathf.Round(Input.GetAxisRaw ("h1_" + id));
		v = Mathf.Round (Input.GetAxisRaw ("v1_" + id));
	}
		
	void Update()
	{
		//if (TeamSwitch.teamnum != null)
			//Debug.Log ("Team : " + id + ":: " + TeamSwitch.teamnum [id - 1]);
		//Debug.Log ("pt: " + players_total);
		//Debug.Log ("pr: " + players_ready);
		GetInput ();
		if (players_total > 4)
			players_total = 4;
		if (players_ready > 4)
			players_ready = 4;

		if (state < 3) {
			if (a) {				// A Button
				if (state == -1) 
					SetTitle (false);
				
				else
					AdvanceMenu ();
			}
			if (b) {				// B Button
				if (state == 0) 
					SetTitle (true);
				
				else
					EscapeMenu ();
			}
			if (state > -1)
				UpdateMenu ();
		}
		else
		{
			if (players_total > 0)
			if (players_total == players_ready) {
				//start game
			}
		}
	}
		
	void SetTitle(bool active) // Title
	{
		players_total += active ? -1 : 1;
		if (active && players_total > 0)
			return;
		
		option = (active) ? -1 : 0;
		state = (active) ? -1 : 0;

		if (state == 0) 
			menus [state].SetActive (true);
		else
			menus [state + 1].SetActive (false);
		
		titleCanvas.SetActive (active);
	}

	void AdvanceMenu() // A Button
	{
		if (pointers [state] != null)
			pointers [state].transform.position = pointerBase[state];
		
		menus [state].SetActive (false);

		if (state == 0)
		{
			if (-pointerPos+1 == 3) { // escape
				pointers[state].transform.position = pointerBase[state];
				SetTitle (true);
				return;
			}
			else
				option = -pointerPos + 1; // branch
			if (option == 2 && !Input.GetKey(KeyCode.LeftShift)) return;

			if (option == 2 || ChildQueueController.child_profiles.Count==0)
				if (Input.GetKey(KeyCode.LeftShift))
				state++;
		}
		else {
			

			if (-namesPos >= ChildQueueController.child_profiles.Count || ChildQueueController.child_profiles.Count == 0) {
				SetPlayerProfile (state == 2); // create player
				if (state == 1)
					state++;
				
			}
			else if (ChildQueueController.child_profiles [-namesPos].plays > 0 
			//	&& !ChildQueueController.children.Contains(ChildQueueController.child_profiles[-namesPos])
				//&& !playerAlreadyActive
			) {
				SetPlayerProfile (state == 2); // create player
				if (state == 1)
					state++;
			} else {
				menus [state].SetActive (true);
				return; // If the player has no more plays, end the A press call
			}
		}

		/*
		if (state == 1)
			IniNames ();
		*/
		state++;
		if (state <= 2) // final state (could be better) 
			menus [state].SetActive (true);
		else
			CreatePlayer ();

		if (state == 1)
			if (option == 1) // load
				IniNames ();

		if (state == 3)
			return;
		/*
		if (pointers [state] != null)
			pointerBase = pointers [state].transform.position;
*/
		pointerPos = 0; // reset pointer
	}

	bool CheckForPlayerInScene(int index)
	{
		for (int i = 0; i < ChildQueueController.player_profiles.Length; i++) {
			if (ChildQueueController.player_profiles[i] == null) continue;
			if (ChildQueueController.player_profiles [i].name == ChildQueueController.child_profiles [index].name)
				return true;
		}
		return false;

	}
	void EscapeMenu()		// B button
	{
		if (state == -1)
			return;

		if (pointers [state] != null)
			pointers [state].transform.position = pointerBase[state];
		
		menus [state].SetActive (false);
		if (state == 2)
			state--;
		state--;
		menus [state].SetActive (true);
		/*
		if (pointers [state] != null)
			pointerBase = pointers [state].transform.position;*/
		pointerPos = 0;

	}

	void IniNames() // sets up names
	{
		namesPos = 0;
		namesTotal = ChildQueueController.child_profiles.Count;
		namesText.text = "";

		for (int i = 0; i < Mathf.Min( ChildQueueController.child_profiles.Count, 5); i++) 
		{
			bool playerActive = CheckForPlayerInScene (i);
			if (playerActive || ChildQueueController.children.Contains (ChildQueueController.child_profiles [i]))
				namesText.text += "<color=#999999ff>";
			else if (ChildQueueController.child_profiles [i].plays <= 0)
				namesText.text += "<color=#ff0000ff>";
			else
				namesText.text += "<color=#00ff00ff>";
			namesText.text += ChildQueueController.child_profiles [i].name;/* + " [";
			namesText.text += ChildQueueController.child_profiles [i].plays + "] ";

			if (ChildQueueController.child_profiles [i].plays <= 0) {
				System.DateTime lastPlayTime = ChildQueueController.child_profiles [i].lastPlayTime;
				namesText.text += lastPlayTime.AddHours(3.0).TimeOfDay;
				namesText.text = namesText.text.Remove (namesText.text.Length-8);
			}
			*/
			namesText.text += "\n";
			namesText.text += "</color>";
		}
		UpdateMenu ();

	}

	void UpdateMenu() // V controls
	{
		if (state < 3) {
			if (pointers [state] != null) {
				if (vMove) {
					vMove = false;
					pointerPos += (int)(v);
					namesPos += (int)(v);
					if (state == 0)
						pointerPos = Mathf.Clamp(pointerPos,-2,0);
					if (state == 1) {
						if (namesPos == -namesTotal)
							pointerPos++;
						
						namesPos = Mathf.Clamp(namesPos,-namesTotal+1,0);

						if (pointerPos > 0 || pointerPos == -5) {	//Scroll
							if ((pointerPos > 0 && namesPos != 0) || (pointerPos == -5 && namesPos != -namesTotal)) { // if we can go further, go
								namesText.text = "";
								if (pointerPos > 0)
									namesPos += 4;
								for (int i = -namesPos; i < Mathf.Min (ChildQueueController.child_profiles.Count, -namesPos + 5); i++) {
									bool playerActive = CheckForPlayerInScene (i);
									if (playerActive || ChildQueueController.children.Contains (ChildQueueController.child_profiles [i]))
										namesText.text += "<color=#999999ff>";
									else if (ChildQueueController.child_profiles [i].plays <= 0)
										namesText.text += "<color=#ff0000ff>";
									else
										namesText.text += "<color=#00ff00ff>";
									
									namesText.text += ChildQueueController.child_profiles [i].name;/* + " [";
									namesText.text += ChildQueueController.child_profiles [i].plays + "] ";
									if (ChildQueueController.child_profiles [i].plays <= 0) {
										namesText.text += ChildQueueController.child_profiles [i].lastPlayTime.AddHours(3.0).TimeOfDay;
										namesText.text = namesText.text.Remove (namesText.text.Length-8);
									}
									*/
									namesText.text += "\n";
									namesText.text += "</color>";
								}
								if (pointerPos > 0)
									namesPos -= 4;
								if (pointerPos > 0)
									pointerPos = -4;
								else if (pointerPos == -5)
									pointerPos = 0;
							} else {
								if (pointerPos > 0)
									pointerPos = 0;
								else if (pointerPos == -5)
									pointerPos = -4;
							}
						}

					}
					pointers [state].transform.position = transform.position + pointerBase[state] + Vector3.up * pointers [state].transform.localScale.z * pointerPos;
				}
			} else {
				inputText.text += Input.inputString;

			}
		}
	}




	void SetPlayerProfile(bool _new) 
	{
//		string name = "AYYYYYY";
		if (_new ) {
			// create new profile from string.
			Child newChild = new Child(inputText.text);
			ChildQueueController.player_profiles[id-1] = newChild;
			inputText.text = "";
			ChildQueueController.child_profiles.Add (newChild);
			GameObject temp = GameObject.FindGameObjectWithTag ("QueueController");
			ChildQueueController cqc = temp.GetComponent<ChildQueueController> ();
			name = newChild.name;
			cqc.SaveChildren ();
			//Debug.Log ("Name" + name);
			tutorial.SetActive (true);

		} else {
			
			ChildQueueController.player_profiles[id-1] = (ChildQueueController.child_profiles[-namesPos]);
			name = ChildQueueController.child_profiles [-namesPos].name;

			if (ChildQueueController.child_profiles[-namesPos].gamesPlayed == 0)
				tutorial.SetActive (true);
			else
				OpenSides ();
			
		}
	}

	void CreatePlayer() // PLayer Creatiom
	{
		player = Instantiate (playerPrefab, transform.position, Quaternion.identity) as GameObject;
		PlayerEnemyHandler enemyHand = player.GetComponent<PlayerEnemyHandler> ();

		player.GetComponent<PlayerTag> ().Id = id;
		tutorial.GetComponent<TutorialController> ().enemyHand = enemyHand;

		AssembleStatsText ();
		Player_Health base_health = player.GetComponent<Player_Health> ();
		base_health.SetInvincibilty (true);
	}

	public void ReadyPlayer() // Ready player
	{
		players_ready++;
		if (GameManager.player_in == null)
			GameManager.player_in = new bool[4];
		GameManager.player_in [id - 1] = true;
		if (players_ready == players_total && players_total > 0) {
			SceneManager.LoadScene ("ModeSelect");
		}
		Destroy (player);
	}

	public void ExitPlayer() // Exit player
	{
		statsText.gameObject.SetActive (false);
		Destroy (player);
		state = 0;
		option = 0;
		pointerPos = 0;

		for (int i = 0; i < menus.Length; i++) {
			menus [i].SetActive (false);
		}

		menus [state].SetActive (true);

		playWall.SetActive (false);
		exitWall.SetActive (false);
		playWallCol.SetActive (true);
		exitWallCol.SetActive (true);
		ChildQueueController.player_profiles [id - 1] = null;
		nameObject.SetActive (false);
	}


	public void OpenSides()
	{
		playWall.SetActive(true);
		exitWall.SetActive (true);
		playWallCol.SetActive (false);
		exitWallCol.SetActive (false);
	}

	public void AssembleStatsText()
	{
		statsObject.SetActive (true);

		Child playerStats = ChildQueueController.player_profiles [id - 1];

		string outString = "";

		outString += "GAMES : " + playerStats.gamesPlayed + "| ";
		outString += "WINS : " + playerStats.wins+ "| ";
		outString += "DUNKS : " + playerStats.kills+ "| \n";
		outString += "KDA : " + Mathf.Round((playerStats.kills / (playerStats.deaths + (playerStats.deaths == 0 ? 1 : 0)))/100f)*100f+ "| ";
		outString += "LOSSES : " + playerStats.losses+ "| ";
		outString += "DEATHS : " + playerStats.deaths+ "| ";

		statsText.text = outString;

		EnableName ();
	}

	public void EnableName()
	{
		nameObject.SetActive (true);
		nameObject.GetComponent<Text> ().text = ChildQueueController.player_profiles [id - 1].name;
	}
}

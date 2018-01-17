using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModeSelectScript : MonoBehaviour {

	[SerializeField] AudioClip clipSwitch,clipSelect,clipDeselect,music;

	public int[] playerPos;
	public bool[] playerLocked;
	int player_total;
	float[] v;
	bool[] a,b;
	string[] playerText;
	public Text canvasText, descText;
	public int modeCount=5;
	public float[] lv;
	int minPlayer=3;
	// Use this for initialization

	void Start () {
		GlobalAudioManager.Instance.PlayMusic (music);
		GameManager.gameMode = GAMEMODE.PVP;
		//SceneManager.LoadScene ("pvpScene");
		v = new float[4];
		lv = new float[4];
		a = new bool[4];
		b = new bool[4];

		playerPos = new int[4];
		playerLocked = new bool[4];
		playerText = new string[4];

		for (int i = 0; i < 4; i++) {
			if (GameManager.player_in[i]) {
				player_total++;
				playerText [i] = PlayerColor (i);
				playerText [i] += "P" + (i + 1) + "></color>";
				if (minPlayer < i) {
					minPlayer = i;
				}
			} 

		}
	}
	
	// Update is called once per frame
	void Update () {
		GetInput ();
		UpdateText ();
		CheckPlayers ();
	}

	void GetInput()
	{	
		for (int i = 0; i < 4; i++) {
			if (!GameManager.player_in[i])
				continue;
			lv [i] = v[i];
			v [i] = -Mathf.Round(Input.GetAxisRaw("v1_"+(i).ToString()));
			a [i] = Input.GetButtonDown ("aButton_" +   (i).ToString());
			b [i] = Input.GetButtonDown ("bButton_" +   (i).ToString());
		}
	}

	void UpdateText()
	{
		for (int i = 0; i < 4; i++) {
			if (!GameManager.player_in[i])
				continue;
			
			if (!playerLocked [i] && lv [i] == 0f) {
				if (v[i] != 0f)
					GlobalAudioManager.Instance.PlaySound (clipSwitch);
				playerPos [i] = Mathf.Max (0,			//min
					Mathf.Min (modeCount ,//max	
						playerPos [i] + (int)Mathf.Round (v [i])//value
					));
			}
			
			if (a [i]) {
				GlobalAudioManager.Instance.PlaySound (clipSelect);
				if (playerPos [i] == 5) {
					GameManager.player_in [i] = false;
					player_total--;
					playerText[i] = "";
					if (player_total <= 0)
						SceneManager.LoadScene ("menuScene");
				}
				playerLocked [i] = true;
				playerText [i] = "<color=#aaa>P" + (i + 1) + "></color>";
			}
			if (b [i]) {
				if (playerLocked [i]) {
					GlobalAudioManager.Instance.PlaySound (clipDeselect);
					playerLocked [i] = false;
					playerText [i] = PlayerColor (i);
					playerText [i] += "P" + (i + 1) + "></color>";
				} else {
					
				}
			}
		}
		canvasText.text = "";
		UpdateMapText ();
	}

	void UpdateModeText()
	{
		
		ConstructText ("dunk-match",0);
		ConstructText ("dunk-arena",1);
		ConstructText ("dunkatron classic",2);
		ConstructText ("dunka-naut",3);
		ConstructText ("capture-dunk",4);

		if (playerPos [minPlayer] == 1) {
			descText.text = "dunk-match is a pvp mode with robot allies on the battlefield.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "dunk-arena is a pvp mode where players can pick up different weapons.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "dunkatron is a co-op gamemode where players fight against robot hordes.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "in dunka-naut, one player is granted a ton of power, and everyone else has to take them out!";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "capture-dunk is a pvp mode where players control different capture points (some of which can give players buffs!)";
		}
	}

	void UpdateMapText()
	{
		ConstructText ("Classic",0);
		ConstructText ("Spiral Crater",1);
		ConstructText ("The Pit",2);
		ConstructText ("Offplanet Market",3);
		ConstructText ("Corridors",4);
		ConstructText ("<color=#f00>Exit</color>",5);

		if (playerPos [minPlayer] == 1) {
			descText.text = "Classic Arena: all dunkatrons, no weapon pickups.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "A symmetrical map. Defend your quadrant or invade your neighbors.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "Go for the cornucopia or shoot at those who do.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "Strategic map with hidden passages.";
		} else if (playerPos [minPlayer] == 2) {
			descText.text = "Tight corridors heavily involves PinkoPankos and rockets.";
		}
	}

	void ConstructText(string modeName, int modePos)
	{
		for (int t = 0; t < modePos; t++) {
			canvasText.text += "\t";
		}

		for (int i = 0; i < 4; i++) {
			if (!GameManager.player_in[i])
				continue;
			if (playerPos [i] == modePos)
				canvasText.text += playerText [i];
		}
		canvasText.text += modeName+"\n";
	}

	void CheckPlayers()
	{
		if (player_total == 0) {
			SceneManager.LoadScene ("menuScene");
			return;
		}

		int checkPos=playerPos[0], players_locked=0;
		bool posFlag=false;

		for (int i = 0; i < 4; i++) { // CHECK PLAYSER
			if (!GameManager.player_in[i])
				continue;
			if (playerLocked [i]) {
				players_locked++;
				posFlag = (checkPos==playerPos[i]||posFlag);
				checkPos = playerPos [i];
			}
		}


		if (players_locked == player_total) { // DO SEOMTHING
			if (!posFlag) {
				int mode=0;
				for (int i = 0; i < 4; i++) {
					if (GameManager.player_in[i]) {
						mode = playerPos [i];
						break;
					}
				}
				SetupMap (mode);

			} else { // IF NO CONSENSUS
				List<int>[] modeScores = new List<int>[5]; for (int i = 0; i < 5; i++) {	modeScores [i] = new List<int> ();	} // setup lists

				int pos = 0;
				for (int i = 0; i < 4; i++) {
					if (!GameManager.player_in[i])
						continue;
					modeScores [playerPos [i]].Add (pos);
					pos++;
				}

				int r = Mathf.RoundToInt(Random.Range (0, pos-1));

				for (int i = 0; i < modeScores.Length; i++) { // run numbers
					if (modeScores [i].Contains (r)) {
						SetupMap (i);
					}
				}
			}
		}
	}

	void SetupMode(int mode)
	{
		if (mode == 0) {
			SceneManager.LoadScene ("pvpScene");
			GameManager.gameMode = GAMEMODE.PVP;
		} else if (mode == 1) {
			SceneManager.LoadScene ("pvpScene");
			GameManager.gameMode = GAMEMODE.PVP_GUNS;
			GameManager.robotronLike = false;
		} else if (mode == 2) {
			SceneManager.LoadScene ("CoopRoom");
			GameManager.gameMode = GAMEMODE.PVE;
			GameManager.robotronLike = true;
		} else if (mode == 3) {
			SceneManager.LoadScene ("jugScene");
			GameManager.gameMode = GAMEMODE.JUGGERNAUT;
		} else if (mode == 4) {
			SceneManager.LoadScene ("capScene");
			GameManager.gameMode = GAMEMODE.CP;
		}else if (mode == 5) {
			SceneManager.LoadScene ("testScene");
			GameManager.gameMode = GAMEMODE.CTF;
		}else if (mode == 6) {
			SceneManager.LoadScene ("MobaRoom");
			GameManager.gameMode = GAMEMODE.MOBA;
		}

	}

	void SetupMap(int map)
	{
		GameManager.map = (MAP)map;
		if (map == 5)
			SceneManager.LoadScene ("menuScene");
		else
			SceneManager.LoadScene ("pvpScene");
	}

	string PlayerColor(int i)
	{
		if (i == 0)
			return "<color=#00f>";
		else if (i == 1)
			return "<color=#ff0>";
		else if (i == 2)
			return "<color=#f00>";
		else
			return "<color=#0ff>";
	}
}


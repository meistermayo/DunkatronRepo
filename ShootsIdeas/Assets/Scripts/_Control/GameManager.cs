using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GAMEMODE{
	NONE=0,
	PVP,
	PVE,
	MOBA,
	CTF,
	JUGGERNAUT,
	CP,
	PVP_GUNS
}

public enum MAP{
	CLASSIC = 0,
	SPIRAL,
	PIT,
	MARKET,
	MAZE
}

public enum WeaponSetting{
	LEMON_ONLY,
	PERSONAL_ONLY,
	SECONDARY_ONLY,
	RANDOM,
}

public enum SpawnType{
	DEF,
	RANDOM,
	SAME_SPOT
}


[System.Serializable]
public class GameOptions{
	public bool numbers;

	[Header("Player Settings")]
	public int lives;

	public float base_health;
	public float base_damage;
	public float base_speed;

	public float health_cap;
	public float damage_cap;
	public float speed_cap;

	[Header ("Weapon Settings")]
	public WeaponSetting weaponSetting;
	public bool unlimitedAmmo;
	public int secondaryOnly;

	[Header("Robot Settings")]
	public float health_mult_robot;
	public float damage_mult_robot;
	public float speed_mult_robot;

	public int pinkopanko_bullet;
	public float pinkopanko_attack_speed;

	[Header("Map Settings")]
	public bool mapWeaponsOff;
	public bool mapAmmoOff;
	public bool mapPowerupsOff;
	public bool mapRobotsOff;
	public SpawnType spawnType;

}

public class GameManager : MonoBehaviour {
	#region Singleton Functionality
	private static GameManager instance;
	public static GameManager Instance{
		get{ return instance; }
		set{ }
	}

	void Singleton()
	{
		if (instance == null)
			instance = this;
		else
			DestroyImmediate (this);
		
		//if (gameObject != null)
		//	DontDestroyOnLoad (gameObject);
	}
	#endregion

	#region Variables
	[Header("General Variables")]
		[SerializeField] Text timerText;
		[SerializeField] float timer = 20f;//60f*5f; // five minutes
		[SerializeField] GameObject playerPrefab;
		[SerializeField] GameObject[] maps;
		[SerializeField] public Transform[] spawnPoints;

		[HideInInspector] public static GAMEMODE gameMode = GAMEMODE.JUGGERNAUT;
		[HideInInspector] public static bool[] player_in;

		GameObject[] players;
		int players_total;
		Player_Health[] playerLives;
		public Text[] playerTexts;
		int[] respawnTicks;
		Coroutine cEndGame;
		TextTransitions textTransitions;
		


	[Header("PVP/PVP_GUNS Variables")]
		public int[] playerKills;
		public static MAP map;

	[Header("PVE Variables")]
		[SerializeField] GameObject[] enemySpawns;
		[SerializeField] Text scoreText;
		
		[HideInInspector] public float enemyMult = 1f;
		[HideInInspector] public static float score;
		[HideInInspector] public static bool robotronLike = false;
		
		GameObject[] enemies;
		int maxEnemies = 25;
		static float highestMult;
		static int wave;
		int scoreLife;
		bool doubleRate = true;
		float[] saveFloat;

	[Header("Juggernaut Variables")]
		[HideInInspector] public int[] juggernautPoints;


	[Header("Capture Point Variables")]
		[SerializeField] CapturePoint[] capturePoints;
		[HideInInspector] public static int[] cpScore;

	[Header("GAME OPTIONS")]
		[SerializeField] GameOptions gameOptions;

	#endregion

	#region Setup Fxns
	void GeneralSetup()
	{
		textTransitions = GetComponent<TextTransitions> ();
		respawnTicks = new int[4];
		players = new GameObject[4];
		playerLives = new Player_Health[4];
		InstantiatePlayers ();
		StartCoroutine (textTransitions.FadeIn ());
	}

	void SetUpGame()
	{
		GeneralSetup ();
		if (gameMode == GAMEMODE.PVE) {
			IniClassic ();
		} else if (gameMode == GAMEMODE.PVP) {
			IniDunkmatch ();
		} else if (gameMode == GAMEMODE.PVP_GUNS) {
			IniArena ();
		}
			
	}

	void IniClassic()
	{
		scoreLife = 1;
		wave = 0;
		highestMult = 1f;
		score = 0f;
		saveFloat = new float[4];

		StartCoroutine (SpawnEnemies ());
		StartCoroutine (CheckPlayers ());
	}

	void IniDunkmatch(){
		StartCoroutine (CheckPlayers ());
	}

	void IniArena(){
		StartCoroutine (CheckPlayers ());
	}

	void IniJuggernaut()
	{
		StartCoroutine (GameTime ());
		int j = Random.Range (0, 4);
		int count = 0;
		while (players [j] == null) {
			j = Random.Range (0, 4);
			count++; if (count > 100) {
				Debug.Log ("too big");
				break;
			}
		}
		juggernautPoints = new int[4];
		players [j].GetComponent<PlayerController>().BecomeJuggernaut();
	}

	void IniCapturePoint()
	{
		cpScore = new int[4];
		StartCoroutine(SetNewCapturePoint ());
		for (int i = 0; i < 4; i++) {
			if (players [i] == null)
				continue;
			PlayerTag playerTag = players [i].GetComponent<PlayerTag> ();
			if (playerTag.Team == 0) {
				playerTag.SetTeam(i + 1);//Random.Range (1, 5); TODO ???
				playerTag.SetId(i + 1);
			}
		}

	}

	void MapSelect()
	{
		//map = MAP.PIT;
		maps [(int)map].SetActive (true);
		spawnPoints = maps [(int)map].GetComponent<SpawnPointsHolder> ().spawnPoints;
		GlobalAudioManager.Instance.PlayMusic (map);
	}

	void InstantiatePlayers()
	{
		MapSelect ();
		for (int i = 0; i < 4; i++) {
			if (!player_in[i]) {
				playerTexts [i].text = "";
				continue;
			}
			if (TeamSwitch.teamnum == null) {
				TeamSwitch.teamnum = new int[4];
				TeamSwitch.teamnum [0] = 0;
				TeamSwitch.teamnum [1] = 0;
				TeamSwitch.teamnum [2] = 0;
				TeamSwitch.teamnum [3] = 0;
			}
			//REFERENCES -- INI
			players [i] = Instantiate (playerPrefab, spawnPoints [i].position, Quaternion.identity) as GameObject;
			PlayerController playerController = players [i].GetComponent<PlayerController> ();
			playerController.IniPlayer (i,TeamSwitch.teamnum[i],RoomController_Refactored.globalPlayerAnimators[i],RoomController_Refactored.globalPlayerWeapons[i]);
			playerTexts [i].text = 
				//ChildQueueController.names[playerController.NameIndex] + "\nLIVES : " + 
				players[i].GetComponent<Player_Health> ().Lives + "\nDUNKS : 0";
			playerLives [i] = players [i].GetComponent<Player_Health> ();
			if (gameMode == GAMEMODE.PVE) {
				players [i].GetComponent<Player_Health> ().SetLives_Hard(2);
			} else {
				playerController.GetPlayerTag().Team = TeamSwitch.teamnum [i];
				/*
				if (gameMode == GAMEMODE.PVP_GUNS) {
					WeaponManager wepman = players [i].GetComponent<WeaponManager> ();
					wepman.priWepInd = 0;
					wepman.secWepInd = 0;
				}
				*/
			}

			//STATS
			players_total++;
			//ChildQueueController.player_profiles [i].gamesPlayed++;
			//ChildQueueController.player_profiles [i].plays--;
			//ChildQueueController.player_profiles [i].lastPlayTime = System.DateTime.Now;
		}

		Camera.main.GetComponent<CameraController> ().players = players;
	}
	#endregion

	#region ----------------- UNITY -------------
	void Awake()
	{
		//map = MAP.PIT;
		Singleton ();
		SetUpGame();
	}
		
	void Update()
	{
		if (Input.GetKeyDown (KeyCode.R)) {
			//ResetGame ();
		}
		UpdateText ();
		if (gameMode == GAMEMODE.PVE) {
			CheckForExtraLife ();
			for (int i = 0; i < 4; i++) {
				if (players [i] == null)
					continue;
				Player_Health hs = players[i].GetComponent<Player_Health> ();
				if (hs.Health < saveFloat[i]) {
					doubleRate = false;
					break;
				}
			}
		}

		if (gameMode == GAMEMODE.CP) {
			for (int i=0; i<4; i++)
			{
				if (players [i] == null)
					continue;
				int t = players [i].GetComponent<PlayerTag> ().Team;

				if (cpScore [t] == 3) {
					cpScore [t] = 4;
					StartCoroutine (EndGame (t, 
						true));
				}
			}
		}
		
		/*
		int count = 0;
		for (int i = 0; i < 4; i++) {
			if (players [i] == null)
				continue;
			if (playerLives [i].Lives == 0)
				count++;
		}
		if (count == players_total && cEndGame == null)
			cEndGame = StartCoroutine (EndGame ());
			*/
	}
	#endregion

	#region ------------------ GENERAL -------------------
	public IEnumerator RespawnPlayer(GameObject player)
	{
		Camera.main.GetComponent<CameraController> ().TurnOnShake ();
		int playerId = player.GetComponent<PlayerTag> ().Id;
		respawnTicks [playerId] = 41;
		/*Time.timeScale = .1f;
		for (int i = 0; i < 9; i++) {
			yield return new WaitForSeconds (.1f);
			Time.timeScale += .1f;
			if (Time.timeScale > 1f) Time.timeScale=1f;
		}*/
		while (respawnTicks[playerId] > 0) {
			yield return new WaitForSeconds (.5f);
			respawnTicks[playerId] -= 5;
		}

		player.SetActive (true);

		Base_WeaponManager weaponManager = player.GetComponent<Base_WeaponManager> ();
		Player_Health healthScript = player.GetComponent<Player_Health> ();
		Base_Player_Movement movementScript = player.GetComponent<Base_Player_Movement> ();
		PlayerController playerController = player.GetComponent<PlayerController> ();

		weaponManager.RefreshWeapons ();

		float tempHealth = healthScript.Health_Max;
		tempHealth = Mathf.Ceil (tempHealth * 1.33f);
		healthScript.SetHealth (tempHealth, tempHealth, 0f);

		movementScript.ResetMovementValues();
		playerController.SetInputActive(true);
		playerController.RefreshCanSwitch();
		player.transform.position = spawnPoints [playerId].position;
		healthScript.StartCoroutine (healthScript.InvincibilityRoutine (60f));
		healthScript.ResetHarsh ();
		//PowerupManager powMan = player.GetComponent<PowerupManager> ();
		//powMan.Reset (); TODO

		if (gameMode != GAMEMODE.PVE || gameMode != GAMEMODE.PVP_GUNS)
				player.GetComponent<PlayerEnemyHandler> ().GiveEnemy (1);
	}

	void UpdateText()
	{
		if (gameMode == GAMEMODE.JUGGERNAUT) {
			timerText.text = ""+timer;
			if (timer <= 10f) {
				timerText.color = Color.red;
				timerText.fontSize = 120;
			}
		}
		for (int i = 0; i < players.Length; i++) {
			if (players [i] == null)
				continue;
			if (gameMode == GAMEMODE.JUGGERNAUT) {
				if (players [i].GetComponent<PlayerController> ().IsJuggernaut) 
					playerTexts [i].fontSize = 45;
				else
					playerTexts [i].fontSize = 30;
				playerTexts [i].text = "";//ChildQueueController.player_profiles [i].name + "\nPOINTS : ";
				playerTexts [i].text += "<color=#ffffffff>";
				playerTexts [i].text += juggernautPoints[i] + "</color>";
			}
			else
			{
				playerTexts [i].text = "";//ChildQueueController.player_profiles [i].name + "\nLIVES : ";

				playerTexts [i].text += "<color=#ffffffff>";

				for (int j=0; j < players[i].GetComponent<Player_Health> ().Lives; j++)
				{
					playerTexts[i].text += "[L] ";
				}

				playerTexts [i].text += "</color>";
				playerTexts[i].text += "\nDUNKS : " + playerKills[i];
			}

			playerTexts [i].text += "\n <color=#ffffffff>";
			if (respawnTicks [i] > 0)
				for (int k = 0; k < respawnTicks [i]; k+=5)
					playerTexts [i].text += " <<!>> ";

			playerTexts [i].text += "</color>";
		}
		if (gameMode == GAMEMODE.PVE) {
			scoreText.text = "WAVE: " + wave;
																	     if (enemyMult == 1f) scoreText.text += "<color=#00ffffff>";
			scoreText.text += "\nENEMY STRENGTH: " + Mathf.Round((enemyMult * 100f)); if (enemyMult == 1f) scoreText.text += "</color>";
			scoreText.text += "% <color=#ff0000ff>(^" +(highestMult*100f)+ "%)</color>\nSCORE: " + score;
		}
	} 

	IEnumerator CheckPlayers()
	{
		bool gameIsOver = false;
		bool team = false;

		if (TeamSwitch.teamnum != null)
		{
			for (int i = 0; i < TeamSwitch.teamnum.Length; i++) { // initial check for teams
				if (TeamSwitch.teamnum [i] != 0)
				{		
					team = true;
					break;
				}
			}
		}


		while (!gameIsOver) {		// loop begin
			yield return new WaitForSeconds (1f);
			float count = 0f;
			if (gameMode == GAMEMODE.PVE) {
				for (int i = 0; i < 4; i++) {
					if (players [i] == null) {
						count++;
						continue;
					}
					if (players [i].GetComponent<Player_Health> ().Lives == 0)
						count++;
					
				}
				if (count == 4) {
					StartCoroutine (EndGame (4,false));
				}
			} else {
				int pIndex = 0;

				for (int i = 0; i < players.Length; i++) { // check for 1 player left
					if (players [i] == null)
						continue;
					if (players [i].GetComponent<Player_Health> ().Lives > 0) {
						count++;
						pIndex = i;
					}
				}

				int teamnum = 0;			// Setup first teamnum variable
				for (int i = 0; i < 4; i++) {
					if (players [i] == null)
						continue;
					if (players [i].GetComponent<Player_Health> ().Lives > 0) {
						teamnum = TeamSwitch.teamnum [i];
						break;
					}
				}


				if (count > 1f) {
					if (team) { // if more than one player
						count = 1f;
						for (int i = 0; i < players.Length; i++) { 							// check teams -- one team leaft count == 1 and continue coroutine. more than one team (ffa or otherwise) can be fwoopd
							if (players [i] == null)
								continue;
							if (players [i].GetComponent<Player_Health> ().Lives > 0) {
								if (TeamSwitch.teamnum [i] != teamnum) {
									count++;
									break;
								}
								TeamSwitch.teamnum [i] = teamnum;
							}
						}
					}
				}

				if (count == 1f) { 															// if 1 player or team left, go.
					for (int i = 0; i < players.Length; i++) {
						if (players [i] == null)
							continue;
						players [i].GetComponent<Player_Health> ().SetInvincibilty( true);
					}

					PlayerDeadController[] pdc = FindObjectsOfType<PlayerDeadController> ();
					for (int i = 0; i < pdc.Length; i++) {
						Destroy (pdc[i].gameObject);
					}

					gameIsOver = true;
					StartCoroutine (EndGame (pIndex, team));								// IENUMERATOR
					//ChildQueueController.player_profiles [pIndex].wins++;
					Debug.Log ("start");
				}
				if (count == 0f) { 															// if no one, draw game
					StartCoroutine (EndGame (-1, team)); 									// IENUMERATOR
				}
			}
		} // loop end

	}
	#endregion

	#region ----------- GAMEMODE SPECIFIC ------------
	// CP ONLY
	public IEnumerator SetNewCapturePoint()
	{
		yield return new WaitForSeconds (3f);
		int r = Random.Range (0, capturePoints.Length);

		capturePoints [r].SetColors (0);
	} 


	//JUGGERNAUT ONLY
	public IEnumerator GameTime()
	{
		while (timer > 0f) {
			timer--;
			yield return new WaitForSeconds (1f);
		}
		/*
		timer = 60f;
		for (int i = 0; i < players.Length; i++) {
			if (players [i] == null)
				continue;
			MovementScript movScr = players [i].GetComponent<MovementScript> ();
			if (movScr.isJuggernaut) {
				movScr.UnbecomeJuggernaut ();
			}
		}
		int min = 100, minIndex = -1;
		for (int i = 0; i < 4; i++) {
			if (players [i] = null)
				continue;
			if (points [i] < min) {
				minIndex = i;
				min = points [i];
			}
		}
		if (minIndex > -1)
		 	players [minIndex].GetComponent<MovementScript> ().BecomeJuggernaut ();

		while (timer > 0f) {
			timer--; yield return new WaitForSeconds (1f);
		}
		*/
		int maxPoints = 0;
		int winner = 0;
		for (int i = 0; i < 4; i++) {
			if (players [i] == null)
				continue;
			if (juggernautPoints [i] > maxPoints) {
				winner = i;
				maxPoints = juggernautPoints [i];
			}
		}
		StartCoroutine(EndGame(winner,false));
	} 


	// PVE ONLY
	void CheckForExtraLife()
	{
		if (score > scoreLife * 20000f) {
			for (int i = 0; i < 4; i++) {
				if (players [i] == null)
					continue;
				Player_Health hs = players [i].GetComponent<Player_Health> ();
				NumberSpawner.Instance.CreateNumber (players [i].transform.position, "EXTRA LIFE", NUMBER_COL.MOB_DMG, .005f, 240f, 5f);
				if (hs.Lives > 0)
					hs.SetLives_Hard(hs.Lives+1);
			}
			scoreLife++;
		}
	} 

	public IEnumerator SpawnEnemies()
	{
		yield return new WaitForSeconds (3f);
		while (wave < 50) {
			float checkMult = enemyMult;
			int spawn = Random.Range (0, enemySpawns.Length - 1);
			int i = Random.Range (0, 5);
			int enemy = Random.Range (0, 3);
			if (enemy > 2)
				enemy = 2;
			/*
			if (i == 1)
				enemy = 2;
			else if (i < 3)
				enemy = 1;

			if (i % 2 == 0) {
				i *= 2;
			}
			*/



			bool reset = false;
			/*
			GameObject[] temp = GameObject.FindGameObjectsWithTag ("Enemy");

			if (temp.Length >= maxEnemies) {

			}
*/
			if (robotronLike) {
				
				i = Mathf.RoundToInt(i*(int)(enemyMult)/2);
				if (enemy == 1) {
					i = 1;
				}
			}
			while (i > 0) {
				GameObject enemyGO;
				EnemyScript enemyScript=null;
				if (robotronLike) {
					for (int j = 0; j < enemySpawns.Length; j++) {
						enemyGO = Instantiate (enemies [enemy], enemySpawns [j].transform.position, Quaternion.identity) as GameObject;
						enemyScript = enemyGO.GetComponent<EnemyScript> ();
						enemyScript.id = 0;

						enemyScript.move_clamp *= .25f;

						if (enemyScript.type == 1)
							enemyScript.hp_max = 100f;
						else
							enemyScript.hp_max = 1f;
						
						enemyScript.hp = enemyScript.hp_max;
						yield return new WaitForSeconds (.1f);
					}
				}
				else {
					enemyGO = Instantiate (enemies [enemy], enemySpawns [spawn].transform.position, Quaternion.identity) as GameObject;
					enemyScript = enemyGO.GetComponent<EnemyScript> ();
					enemyScript.id = 0;

					enemyScript.move_clamp *= .4f;
					enemyScript.hp_max /= 2f;

					enemyScript.hp_max *= enemyMult;
					enemyScript.hp = enemyScript.hp_max;
				}

				if (reset) {
					enemyMult -= 2f;
					if (enemyScript != null)
						enemyScript.move_mult *= 4f * 2f;
				}
				i--;
				yield return new WaitForSeconds (.2f);
			}
			
			if (enemyMult == checkMult)
				enemyMult += .02f;
			highestMult = Mathf.Max (enemyMult, highestMult);
			wave++;
			float delay = 5f/players_total;
			if (doubleRate)
				delay /= 2f;
			doubleRate = true;
			for (int j=0; j<4; j++)
			{
				if (players[j] == null)continue;
				saveFloat[j] = players[j].GetComponent<Player_Health>().Health;
			}
			if (reset)
				delay = 10f;
			/*
			if (robotronLike)
				delay *= 3f;
			*/
				yield return new WaitForSeconds (delay);
		}
		bool gameIsOver = false;
		while (!gameIsOver) {
			GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
			Debug.Log ("enemies.length = " + enemies.Length);
			if (enemies.Length == 0)
				gameIsOver = true;
			else
				yield return new WaitForSeconds (1f);
		}
		StartCoroutine (EndGame (-1, false));
	}
	#endregion

	#region ------------------ Functional ------------------
	public void SetControls(bool active)
	{
		for (int i = 0; i < players.Length; i++) {
			if (players [i] == null)
				continue;
			players [i].GetComponent<PlayerController> ().SetInputActive(active);
		}
	}

	public GameObject GetPlayer(int i)
	{
		if (i > -1 && i < players.Length)
			return players [i];
		return null;
	}
	#endregion

	#region ----------------- End Game ---------------------
	IEnumerator EndGame(int pIndex, bool team)
	{
		if (pIndex == -1) {
			textTransitions.titleText.color = Color.white;
			if (gameMode == GAMEMODE.PVE)
				textTransitions.titleText.text = "YOU SURVIVE!";
			else
				textTransitions.titleText.text = "Draw";

		} else {
			if (!team) // send name
				textTransitions.StartCoroutine (textTransitions.EndGame (""));//ChildQueueController.player_profiles [pIndex].name));
			else {
				string teamName = "Draw Game";

				for (int i = 0; i < 4; i++) { // if in a team, find the winners and leave.
					if (players [i] == null)
						continue;
					if (players [i].GetComponent<Player_Health> ().Lives <= 0)
						continue;
					if (players [i].GetComponent<PlayerController> ().GetPlayerTag ().Team == 0)
						teamName = "";//ChildQueueController.player_profiles [i].name;
					else if (players [i].GetComponent<PlayerController> ().GetPlayerTag().Team == 1)
						teamName = "Blue Team";
					else
						teamName = "Red Team";
					if (teamName != "Draw Game")
						break; // once we find the winner, leave.
				}
				//TODO : Null reference bug -- someones gettin null??
				Debug.Log(pIndex);
				string teamString = "";

				switch (pIndex) {
				case  1:
					teamString = "BLUE";
					break;
				case 2:
					teamString = "YELLOW";
					break;
				case 3:
					teamString = "RED";
					break;
				case 4:
					teamString = "PINK";
					break;
				}

				textTransitions.StartCoroutine (textTransitions.EndGame (teamString));
			}

		}

        //GameObject.FindGameObjectWithTag("QueueController").GetComponent<//ChildQueueController>().SaveChildren ();
        GlobalAudioManager.Instance.PlayRobotVoice(GlobalAudioManager.ROBOTCLIP.MATCH_WIN);
		yield return new WaitForSeconds (6f);
		RoomMenuController.players_ready = 0;
		RoomMenuController.players_total = 0;
		if (TeamSwitch.teamnum != null) {
			for (int i = 0; i < 4; i++) {
				TeamSwitch.teamnum [i] = 0;
			}
		}
		gameMode = GAMEMODE.NONE;
		SceneManager.LoadScene ("menuScene");
	}

	public static void ResetGame()
	{
		
		for (int i = 0; i < 4; i++) {
			player_in [i] = false;
		}

		gameMode = GAMEMODE.NONE;
		SceneManager.LoadScene("iniScene");
	}
	#endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialController : MonoBehaviour {
	Text myText;

	public GameObject[] pickupPrefabs;
	int iPrefab = 0;
	GameObject activePrefab;

	RoomMenuController roomMenu;
	public PlayerEnemyHandler enemyHand;

	Coroutine tutorialRoutine;

	public Child newChild; // TODO

	int id;

	public Transform[] ItemSpawn;

	void Awake()
	{
		roomMenu = GetComponentInParent<RoomMenuController> ();
		id = roomMenu.id;
		myText = GetComponentInChildren<Text> ();
		myText.text = "";
	}

	void Update()
	{
		if (tutorialRoutine != null) {
			if (Input.GetButtonDown ("start_"+id)) { // TODO
				StopCoroutine (tutorialRoutine);
				EndTutorial ();
			}
		}
	}	

	void OnEnable()
	{
		if (!roomMenu.newPlayer)
			tutorialRoutine = StartCoroutine (TutorialCoroutine ());
		else
			StartCoroutine (WaitForPlayer());
	}

	public IEnumerator WaitForPlayer()
	{
		while (!Input.GetButton ("start_"+id)) { // TODO
			myText.text = "Please give the controller to:\n" + newChild.name;
			yield return new WaitForEndOfFrame ();
		}
		tutorialRoutine = StartCoroutine (TutorialCoroutine ());
	}

	public IEnumerator TutorialCoroutine()
	{
		iPrefab = 0;
		myText.text = "Welcome to Dunkatron, a crazy shooter with multiple gamemodes! To start, lets go over controls.";
		yield return new WaitForSeconds (10f);

		myText.text = "Use the left stick to run.";
		yield return new WaitForSeconds (10f);
		myText.text = "Use the right stick to fire. ";
		yield return new WaitForSeconds (10f);
		myText.text = "<RB/LB to switch to personal weapon.";
		yield return new WaitForSeconds (6f);

		myText.text = "To your left is a Robotron. Pick it up!";
		activePrefab = Instantiate (pickupPrefabs [iPrefab], ItemSpawn[0].position, Quaternion.identity) as GameObject;
		iPrefab++;
		while (activePrefab.GetComponent<SpriteRenderer>().enabled)
			yield return new WaitForEndOfFrame();
		

		Destroy (activePrefab);

		myText.text = "Robots fight for you. There are three types.";
		yield return new WaitForSeconds (3f);
		myText.text = "This robot is a Skeel. It is fast and damaging to enemy players, but fragile.";
		yield return new WaitForSeconds (10f);

		myText.text = "Pickup another robot ally.";
		activePrefab = Instantiate (pickupPrefabs [iPrefab], ItemSpawn[1].position, Quaternion.identity) as GameObject;
		iPrefab++;
		while (activePrefab.GetComponent<SpriteRenderer> ().enabled)
			yield return new WaitForEndOfFrame ();
		Destroy (activePrefab);


		myText.text = "This one is a Swolz. It is a tank that obliterates other robots.";
		yield return new WaitForSeconds (10f);

		//Debug.Log ("post delay");

		myText.text = "One more!";
		activePrefab = Instantiate (pickupPrefabs [iPrefab], ItemSpawn[0].position, Quaternion.identity) as GameObject;
		iPrefab++;
		while (activePrefab.GetComponent<SpriteRenderer>().enabled)
			yield return new WaitForEndOfFrame();
		Destroy (activePrefab);


		
		myText.text = "The Pinko Panko steals pickups to spawn faster, but more fragile, Skeels at enemies.";
		yield return new WaitForSeconds (10f);

		myText.text = "They are hard to avoid, but can be shot down.";
		yield return new WaitForSeconds (5f);

		myText.text = "There is a limit to the amount of robots you can have.";
		yield return new WaitForSeconds (7f);

		myText.text = "You can press 'X' to disband your crew.";
		yield return new WaitForSeconds (10f);


		enemyHand.ClearEnemies ();


		DeleteEnemies ();

		myText.text = "There are also two powerups.";
		activePrefab = Instantiate (pickupPrefabs [iPrefab], ItemSpawn[1].position, Quaternion.identity) as GameObject;
		iPrefab++;
		while (activePrefab.GetComponent<SpriteRenderer>().enabled)
			yield return new WaitForEndOfFrame();
		Destroy (activePrefab);

		myText.text = "The red one buffs your damage momentarily.";
		yield return new WaitForSeconds (10f);

		myText.text = "The blue one...";
		activePrefab = Instantiate (pickupPrefabs [iPrefab], ItemSpawn[0].position, Quaternion.identity) as GameObject;
		iPrefab++;
		while (activePrefab.GetComponent<SpriteRenderer>().enabled)
			yield return new WaitForEndOfFrame();
		Destroy (activePrefab);

		myText.text = "Makes you faster!.";
		yield return new WaitForSeconds (10f);

		string upString, leftString;
		upString = (roomMenu.id < 3) ? "up" : "down";
		leftString = (roomMenu.id == 1 || roomMenu.id == 3) ? "left" : "right";

		myText.text = "Every time a player scores a kill, they gain bonus damage!";
		yield return new WaitForSeconds (7f);
		myText.text = "But don't worry -- each time you die you will get bonus health and respawn with more Swolz.";
		yield return new WaitForSeconds (10f);

		myText.text = "This concludes the tutorial. Go " + upString + " to get ready, or go " + leftString + " to exit the game.";
		yield return new WaitForSeconds (5f);

		EndTutorial ();

	}

	public void EndTutorial()
	{
		roomMenu.OpenSides ();
		myText.text = "";
		gameObject.SetActive (false);
		roomMenu.AssembleStatsText ();
		DeleteEnemies ();
		Destroy (activePrefab);
	}

	void DeleteEnemies()
	{
		EnemyScript[] temp = FindObjectsOfType<EnemyScript> ();
		for (int i = 0; i < temp.Length; i++) {
			if (temp [i].id == id) {
				Destroy (temp [i].gameObject);
			}
		}
	}


}

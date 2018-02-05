using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IniGame : MonoBehaviour {

	// Use this for initialization
	void Start () {
		ChildQueueController.player_profiles = new Child[4];
		ChildQueueController.children = new Queue<Child> ();
		ChildQueueController.child_profiles = new List<Child> ();
		GameObject.FindGameObjectWithTag ("QueueController").GetComponent<ChildQueueController> ().LoadChildren ();
			if (Input.GetJoystickNames().Length != 0)
                SceneManager.LoadScene("menuScene");
    }

	IEnumerator StartGame()
	{
		yield return new WaitForSeconds (5f);
		SceneManager.LoadScene ("menuScene");
	}
	// Update is called once per frame
	void Update () {
		if (Input.anyKeyDown) {
			SceneManager.LoadScene ("menuScene");
		}
	}
}

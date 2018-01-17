using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamSwitch : MonoBehaviour {

	SpriteRenderer sr;
	Sprite spriteSave;
	AudioSource audioSource;
	Text myText;
	public static int[] teamnum;
	public int id;
	int mTeamnum;
	static RuntimeAnimatorController[] staticAnimators;
	RuntimeAnimatorController defAnimator;

	bool canSwitch = true;

	string[] titles = {"BLEU","MEGO","EDJ","PRIN","NO TEAM" };
	Color[] myColors = { Color.blue, Color.yellow, Color.red, Color.magenta, Color.white };

	void Awake()
	{
		staticAnimators = new RuntimeAnimatorController[5];
		if (teamnum == null)
			teamnum = new int[4];
		sr = GetComponent<SpriteRenderer> ();
		audioSource = GetComponent<AudioSource> ();
		myText = GetComponentInChildren<Text> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (canSwitch) {
			canSwitch = false;
			audioSource.Play ();
			IncrementTeamNum ();
			AssignAnimator (other.GetComponent<PlayerController> ());
		}
	}

	void IncrementTeamNum()
	{
		mTeamnum = (mTeamnum + 1) % 4;
		teamnum [id - 1] = mTeamnum;
		sr.color = myColors [mTeamnum];
		myText.text = titles [mTeamnum];
		myText.color = myColors [mTeamnum];
	}

	void AssignAnimator(PlayerController playerController)
	{
		RuntimeAnimatorController playerAnimator = playerController.GetComponentInChildren<Animator> ().runtimeAnimatorController;
		if (mTeamnum == 1) {
			defAnimator = playerAnimator;
		} else if (mTeamnum == 0){
			playerAnimator = defAnimator;
		} else 
			playerAnimator = staticAnimators [mTeamnum];

		playerController.GetPlayerTag ().SetTeam (mTeamnum);
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player") {
			canSwitch = true;
		}
	}
}

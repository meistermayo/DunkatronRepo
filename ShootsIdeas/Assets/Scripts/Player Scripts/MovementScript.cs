using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour {

	public float move_speed = 1f;
	Rigidbody2D mBody;
	public Animator mAnim;
	float h,v;
	public int player_num;
	public float move_mult = 1f;
	public bool input_active=true;
	public Sprite[] animatorStartingSprites;
	public RuntimeAnimatorController[] animators;
//	bool checkAnimator;
	public int team;
	int currentAnim;
	public bool isJuggernaut;

	public GameObject[] animatorObjects;

	void Awake()
	{
		mAnim = GetComponentInChildren<Animator> ();
		mBody = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		
		GetInput ();
		mBody.velocity = Vector3.ClampMagnitude ((Vector3.right * h + Vector3.up * v) * move_speed*move_mult, move_speed*move_mult);
		CheckMoveMult (); // FREEZE!
	}

	void CheckMoveMult()
	{
		if (move_mult < 1f) {
			move_mult += .001f;
			if (move_mult > 1f)
				move_mult = 1f;
		}
	}

	void GetInput()
	{
		if (!input_active)
			return;
		h = Input.GetAxisRaw ("h1_"+player_num);
		v = Input.GetAxisRaw ("v1_"+player_num);

		if (h == 0f && v == 0f){
			return; // don't animate
		}

		float dir = v;
		/*
		if (h != 0f)
			transform.localScale = new Vector3 (Mathf.Sign(h)*transform.localScale.z, transform.localScale.z, transform.localScale.z);
*/
		if (h != 0f)
		{
			animatorObjects[currentAnim].GetComponent<SpriteRenderer>().flipX = (h < 0f);
		}
	mAnim.SetFloat("dir",dir);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		/*
		if (other.tag == "Powerup") {
			Weapon wep = GetComponentInChildren<Weapon> ();
			wep.damageMult += .25f;
			PickupRespawn pickupRespawn = other.GetComponent<PickupRespawn> ();
			pickupRespawn.StartRespawnTimer ();
			other.GetComponent<CircleCollider2D>().enabled = false;
		}
		*/
	}

	public void SetAllIds(int id)
	{
		mAnim.StopPlayback();
		Debug.Log ("stopped");
		player_num = id;
		GetComponent<HealthScript> ().id = id;
		GetComponent<WeaponManager> ().player_num = id;

		List<Weapon> weapons = GetComponent<WeaponManager> ().weapons;

		for (int i = 0; i < weapons.Count; i++) {
			weapons[i].player_num = id;
		}

		Child myChild = ChildQueueController.player_profiles [id - 1];


		SetCustomAvatar (id, myChild);

		Color col = Color.white;

		if (id == 1)
			col = Color.blue;
		else if (id == 2)
			col = Color.yellow;
		else if (id == 3)
			col = Color.red;
		else if (id == 4)
			col = Color.magenta;
		
		GetComponentInChildren<UnityEngine.UI.Text>().color = col;

		// TEAMS
		/*
		if (TeamSwitch.teamnum != null) {
			team = TeamSwitch.teamnum [id - 1];
			//Debug.Log("pnum-1: " + (player_num-1) + ". team: " + team + ". ts.teamnum: " +TeamSwitch.teamnum[player_num-1]+".");
			if (team != 0) {
				if (team == 1)
					GetComponent<Animator> ().runtimeAnimatorController = animators [0];
				else
					GetComponent<Animator> ().runtimeAnimatorController = animators [2];
			}
		}
		*/
	}


	public void ChangeAnimator(int index)
	{
		for (int i = 0; i < animatorObjects.Length; i++) {
			animatorObjects [i].SetActive (false);
		}
		animatorObjects [index].SetActive (true);
		currentAnim = index;
		mAnim = animatorObjects [index].GetComponent<Animator> ();
	}

	void SetCustomAvatar(int id, Child myChild)
	{
		if (isJuggernaut) {
			ChangeAnimator (18);
			return;
		}
			if (team > 0) {
			ChangeAnimator (team - 1);
		}
		else if (myChild.name.ToLower() == "odore") {
			ChangeAnimator (4);
		} else if (myChild.name.ToLower() == "flitch") {
			ChangeAnimator (5);
		} else if (myChild.name.ToLower() == "unalis") {
			ChangeAnimator (6);
		} else if (myChild.name.ToLower() == "rangle") {
			ChangeAnimator (7);
		}else if (myChild.name.ToLower() == "bitz") {
			ChangeAnimator (8);
		}else if (myChild.name.ToLower() == "froop") {
			ChangeAnimator (9);
		}else if (myChild.name.ToLower() == "gaze") {
			ChangeAnimator (10);
		}else if (myChild.name.ToLower() == "gug") {
			ChangeAnimator (11);
		}else if (myChild.name.ToLower() == "moeg") {
			ChangeAnimator (12);
		}else if (myChild.name.ToLower() == "mr fish") {
			ChangeAnimator (13);
		}else if (myChild.name.ToLower() == "pippy") {
			ChangeAnimator (14);
		}else if (myChild.name.ToLower() == "senor popo") {
			ChangeAnimator (15);
		}else if (myChild.name.ToLower() == "skulldier") {
			ChangeAnimator (16);
		}else if (myChild.name.ToLower() == "slicey") {
			ChangeAnimator (17);
		} else {
			ChangeAnimator (id-1);
		}
	}

	public void BecomeJuggernaut()
	{
		isJuggernaut = true;
		PlayerEnemyHandler playEnHand = GetComponent<PlayerEnemyHandler> ();
		playEnHand.ClearEnemies ();
		WeaponManager wepMan = GetComponent<WeaponManager> ();
		wepMan.SetAllDamageMults (1f);
		wepMan.SetAllDamageReds (1f);
		wepMan.SetAllAttackSpeedReds (.7f);
		HealthScript healthScript = GetComponent<HealthScript> ();
		move_mult = 1f;
		healthScript.health_max = 500f;
		healthScript.health = healthScript.health_max;
		healthScript.lives = 6;
		playEnHand.enemy_count = 7;
		ParticleController partCont = GetComponentInChildren<ParticleController> ();
		partCont.Play (4);
		SetCustomAvatar (0, null);
	
	}

	public void UnbecomeJuggernaut()
	{
		isJuggernaut = false;
		GetComponent<WeaponManager> ().SetAllDamageMults (1f);
		GetComponent<WeaponManager> ().SetAllAttackSpeedReds (1f);
		SetCustomAvatar (player_num,ChildQueueController.player_profiles [player_num - 1]);
		ParticleController partCont = GetComponentInChildren<ParticleController> ();
		partCont.Stop (4);
	}

}

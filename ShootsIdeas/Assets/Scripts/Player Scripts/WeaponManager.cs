using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponSpace;

public  class WeaponManager : MonoBehaviour {
	
		
	public List< Weapon>weapons= new List<Weapon>();
	public int player_num;
	public bool input_active=true;
	public int iWeapon=0;
	public float attackSpeedRed=1f;
	public int priWepInd, secWepInd;
	void Awake()
	{
		Weapon[] array = GetComponentsInChildren<Weapon> ();

		for (int i = 0; i < array.Length; i++) {
			weapons.Add (array [i]);
		}

		player_num = GetComponent<MovementScript> ().player_num;
	}

	void Start()
	{

		Child myChild = ChildQueueController.player_profiles [player_num - 1];
		secWepInd = 0;
		if (GameManager.gameMode != GAMEMODE.PVP_GUNS) {
			if (myChild.name.ToLower () == "senor popo") 
				secWepInd = 1;
			else if (myChild.name.ToLower () == "odore") 
				secWepInd = 2;
			else if (myChild.name.ToLower () == "flitch")
				secWepInd = 3;
			else if (myChild.name.ToLower () == "unalis")
				secWepInd = 4;
			else if (myChild.name.ToLower () == "rangle")
				secWepInd = 5;
			else if (myChild.name.ToLower () == "gug")
				secWepInd = 6;
			else if (myChild.name.ToLower () == "moeg")
				secWepInd = 7;
			else if (myChild.name.ToLower () == "skulldier")
				secWepInd = 8;
			else if (myChild.name.ToLower () == "slicey")
				secWepInd = 9;
			else if (myChild.name.ToLower () == "pippy")
				secWepInd = 10;
			else if (myChild.name.ToLower () == "mr fish")
				secWepInd = 11;
			else if (myChild.name.ToLower () == "gaze")
				secWepInd = 12;
			else if (myChild.name.ToLower () == "froop")
				secWepInd = 14;
			else  // LUKE >:(
			secWepInd = 13;
		}

	}

	void Update()
	{
		if (!input_active)
			return;
		float h = Input.GetAxis ("h2_" + player_num);
		float v = Input.GetAxis ("v2_" + player_num);
		if (h != 0f || v != 0f) {
			//if (player_num == 2)
				//Debug.Log ("h2: " + h + "/v2: " + v);
			weapons [iWeapon].Attack ();
		}

		if (Input.GetButtonDown("switch_" + player_num)) {
			RotateWeapons ();
		}
	}

	void RotateWeapons()
	{
		if (iWeapon == priWepInd) {
			//Child myChild = ChildQueueController.player_profiles [player_num - 1];
			//Debug.Log(ChildQueueController.player_profiles[player_num-1].name);
			iWeapon = secWepInd;
		} else
			iWeapon = priWepInd;
	}

	public void SetAllDamageMults(float value)
	{
		for (int i=0; i<weapons.Count;i++)
		{
			weapons [i].damageMult = value;
		}
	}

	public void SetAllDamageReds(float value)
	{
		for (int i=0; i<weapons.Count;i++)
		{
			weapons [i].damageRed = value;
		}
	}

	public void SetAllAttackSpeedReds(float value)
	{
		for (int i = 0; i < weapons.Count; i++) {
			weapons [i].attackSpeedRed = value;
		}
	
	}

	public void SetPriWep(int wep)
	{
		priWepInd = wep;
		iWeapon = priWepInd;
	}

	public void SetSecWep(int wep)
	{
		secWepInd = wep;
		iWeapon = secWepInd;
	}
}

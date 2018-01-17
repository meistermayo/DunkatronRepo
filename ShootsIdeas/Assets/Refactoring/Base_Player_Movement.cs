using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CHARACTER
{
	BLEU = 0,
	MEGO,
	EDJ,
	PRIN,

	ODORE,
	FLITCH,
	UNALIS,
	RANGLE,
	BITZ,
	FROOP,
	GAZE,
	GUG,
	MOEG,
	MR_FISH,
	PIPPY,
	SENOR_POPO,
	SKULLDIER,
	SLICEY
}

public class Base_Player_Movement : Base_Movement
{
	[SerializeField] float move_increment;
	bool input_active=true;

	int currentAnim;

	bool isJuggernaut;
	public bool IsJuggernaut{
		get {return isJuggernaut;}
		set {}
	}

	Rigidbody2D mBody;
	Animator mAnim;

	[SerializeField] CHARACTER character;
	[SerializeField] Sprite[] animatorStartingSprites;
	SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		mAnim = GetComponentInChildren<Animator> ();
		mBody = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {
		CheckMoveMult ();
	}

	public void Move(float h, float v)
	{
		if (h == 0f && v == 0f) {
			mBody.velocity = Vector2.ClampMagnitude (mBody.velocity, mBody.velocity.magnitude * move_increment);
		} else 
			mBody.velocity = Vector2.ClampMagnitude (mBody.velocity + (Vector2.right * h + Vector2.up * v) * move_increment, move_speed*move_mult*move_mult_internal);
	}

	void CheckMoveMult()
	{
		if (move_mult < 1f) {
			move_mult += .001f;
			if (move_mult > 1f)
				move_mult = 1f;
		}
	}


	public void SetAllIds(int id)
	{
		List<Weapon> weapons = GetComponent<WeaponManager> ().weapons;

		for (int i = 0; i < weapons.Count; i++) {
			weapons[i].player_num = id;
		}

		Child myChild = ChildQueueController.player_profiles [id - 1];



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

	}

	public void UnbecomeJuggernaut()
	{
		isJuggernaut = false;
		GetComponent<WeaponManager> ().SetAllDamageMults (1f);
		GetComponent<WeaponManager> ().SetAllAttackSpeedReds (1f);
		ParticleController partCont = GetComponentInChildren<ParticleController> ();
		partCont.Stop (4);
	}


	public void ResetMovementValues()
	{
		move_mult = 1f;
		move_mult_internal = 1f;
	}

	public void AddMovement(float value)
	{
		move_mult += value;
	}

	public void SetMovement(float value)
	{
		move_mult = value;
	}
}


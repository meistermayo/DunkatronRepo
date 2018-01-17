using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyHandler : PlayerScript {
	[SerializeField] GameObject[] enemies_prefabs;
	[SerializeField] int enemyMax;
	public GameObject CurrentEnemy{get {return enemies_active[0];}}
	GameObject[] enemies_active;
	public int enemy_count;
	float lastAxis;

	void Awake()
	{
		enemies_active = new GameObject[enemyMax];
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (GetComponent<PlayerController>().IsJuggernaut)
			return;
		if (other.tag == "EnemyPickup") {
			
			if (CheckForPanko ())
				return;

			if (enemy_count >= enemyMax)
				return;
			PickupRespawn pickupRespawn = other.GetComponent<PickupRespawn> ();
			if (!pickupRespawn.active)
				return;
			EnemyPickup pickup = other.GetComponent<EnemyPickup> ();
			pickupRespawn.StartRespawnTimer ();
			other.GetComponent<CircleCollider2D>().enabled = false;

			int r = pickup.enemy;
			if (pickup.random) {
				r = (int)Random.Range (0f, enemies_prefabs.Length);
			}
			if (GameManager.gameMode == GAMEMODE.PVP) {
				if (r == 0)
					GameManager.score += 50f;
				else if (r == 1)
					GameManager.score += 25f;
				else
					GameManager.score += 100f;
			}
			GiveEnemy (r);
		}
	}

	public void GiveEnemy(int r)
	{

		GameObject enemy = Instantiate (enemies_prefabs[r], transform.position, Quaternion.identity) as GameObject;
		Base_Mob enemyScript = enemy.GetComponent<Base_Mob> ();
		PlayerTag enemyPlayerTag = enemy.GetComponent<PlayerTag> ();
		enemyPlayerTag.SetId(playerTag.Id);
		enemyPlayerTag.SetTeam(playerTag.Team);

		Color col = Color.white;
		Color col1 = Color.white;

		for (int i = 0; i < enemies_active.Length; i++) {
			if (enemies_active [i] == null) {
				enemies_active[i] = enemy;
				float angle = (360f / enemies_active.Length) * i;
				Vector3 sideVec = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle)) * 2f;

				float amount = .5f;
				if (playerTag.Id== 1) {
					col = Color.blue;
					col1.r = amount;
					col1.g = amount;
				} else if (playerTag.Id == 2) {
					col = Color.yellow;
					col1.r = amount;
				} else if (playerTag.Id == 3) {
					col = Color.red;
					col1.b = amount;
					col1.g = amount;
				} else if (playerTag.Id == 4) {
					col = Color.magenta;
					col1.g = amount;
				}

				enemyScript.Ini (gameObject, playerTag.Id, playerTag.Team, false);
				//enemyScript.Ini(i, sideVec, gameObject,col,this);
				break;
			}
		}

		enemy_count++;
		enemyScript.GetComponentInChildren<SpriteRenderer> ().color = col1;

		Physics2D.IgnoreCollision (GetComponent<CapsuleCollider2D>(),enemyScript.GetComponent<CircleCollider2D>());
		Base_WeaponManager wepMan = GetComponent<Base_WeaponManager> ();//.weapons [0];
		float damageRed = Mathf.Min(.5f,1f - (enemy_count * .1f));
		wepMan.DamageRed = damageRed;
		NumberSpawner.Instance.CreateNumber (transform.position, "-" + Mathf.Min (50f, enemy_count * 10f) + "% Damage", NUMBER_COL.DEBUFF, 0.1f, 60f, 1f);
	}

	void Update()
	{
		//RotateEnemies ();
		CheckDisband ();
		if (enemy_count == 0) {
			Base_WeaponManager wepMan = GetComponent<Base_WeaponManager>();
			if (wepMan.DamageRed < 1f) {
				wepMan.DamageRed += .001f; // around 8 seconds from 50% reduction.
				if (wepMan.DamageRed > 1f)
					wepMan.DamageRed = 1f;
				if (wepMan.DamageRed == 1f)
					NumberSpawner.Instance.CreateNumber (transform.position, Mathf.Round(wepMan.DamageRed)*100f + "% damage", NUMBER_COL.DEBUFF, .075f, 60f, 1f);
			}
		}
		lastAxis = Input.GetAxisRaw ("rot_" + (playerTag.Id).ToString());
	}

	void RotateEnemies()
	{
		if (Input.GetAxisRaw ("rot_" + (playerTag.Id).ToString()) != 0f && lastAxis == 0f) {
			for (int i = 0; i < enemy_count; i++) {
				if (enemies_active [i] != null) {
					EnemyScript enemy_script = enemies_active [i].GetComponent<EnemyScript> ();
					enemy_script.side = (int)(((float)enemy_script.side + Input.GetAxisRaw ("rot_" + (playerTag.Id).ToString())) % enemies_active.Length);
					float angle = (360f / enemies_active.Length) * enemy_script.side;
					enemy_script.sideVec = new Vector3 (Mathf.Cos (angle), Mathf.Sin (angle)) * 2f;
				}

			}
			GameObject temp = enemies_active [0];
			for (int i = 0; i < enemies_active.Length; i++) {
				enemies_active [i] = enemies_active[(i+1)%enemies_active.Length];

			}
			enemies_active [enemies_active.Length-1] = temp;

		}
	}

	public void ClearEnemies()
	{
		for (int i = 0; i < enemies_active.Length; i++) {
			Destroy (enemies_active [i]);
		}
		enemy_count = 0;
	}

	public void CheckDisband()
	{
		if (Input.GetButtonDown ("xButton_" + (playerTag.Id).ToString())) {
			Disband ();
		}
	}

	public void Disband()
	{
		for (int i = 0; i < enemies_active.Length; i++) {
			if (enemies_active [i] != null) {
				Base_Mob enemy = enemies_active [i].GetComponent<Base_Mob> ();
				enemy.Release ();
				enemies_active [i] = null;
			}
		}
		enemy_count = 0;
	}

	public void Reband(GameObject enemy)
	{
		if (enemy_count < enemyMax) {
			enemies_active [enemy_count] = enemy;
			enemy_count++;
		}
	}

	public bool CheckForPanko()
	{
		for (int i = 0; i < enemies_active.Length; i++) {		// PINKO PANKO disables pickups
			if (enemies_active [i] == null)
				continue;
			if (enemies_active [i].GetComponent<Mob_Pinko>() != null)
				return true;
		}
		return false;
	}
}

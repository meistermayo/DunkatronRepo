using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour {
	public float health, health_max;
	public float armor, resistance;
	public int id;
	public int lives=5;
	public Text healthText;
//	Coroutine invincible_routine;
	public bool invincibilty;
	public GameObject diePrefab;
	public AudioSource death;
	public Slider slider;
	public int stunStack;
	public Text stunText;

	void Awake()
	{
		//healthText = GetComponentInChildren<Text> ();
		slider = GetComponentInChildren<Slider> ();
		slider.maxValue = health_max;
		slider.value = health;
		id = GetComponent<MovementScript> ().player_num;
	}

	void Update()
	{
		healthText.transform.localScale = new Vector3 (.01f*transform.localScale.x,.01f,.01f);
		healthText.text = health.ToString ();
		slider.value = health;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		MovementScript movementScript = GetComponent<MovementScript> ();
		bool hit = false;
		int otherId=1;
		Color hitCol = new Color (1f, .5f, 0f);

		if (!invincibilty) {
			if (other.tag == "Bullet") {
				BulletScript bulletScript = other.GetComponent<BulletScript> ();
				if (bulletScript.id == id)
					return;
				if (movementScript.team != 0 && bulletScript.team == movementScript.team)
					return;
				health -= bulletScript.damage;
				LaserBulletScript lbs = bulletScript.GetComponent<LaserBulletScript> ();
				if (lbs == null)
					Destroy (other.gameObject);
				if (!movementScript.isJuggernaut)
					StartCoroutine (InvincibilityRoutine(30f));
				hit = true;
				otherId = bulletScript.id;
				//GetComponent<PlayerEnemyHandler> ().CreateNumber (transform.position, bulletScript.damage.ToString(), hitCol, .1f, 120f, 3f);
				LucyFreezeBulletScript lucyFreeze = other.GetComponent<LucyFreezeBulletScript> ();
				if (lucyFreeze != null) {
					movementScript.move_mult = Mathf.Max (lucyFreeze.slowMax, movementScript.move_mult - lucyFreeze.slowAmount);
				}

			}

			if (other.tag == "Enemy") {
				EnemyScript enemyScript = other.GetComponent<EnemyScript> ();
				if (enemyScript.id == id)
					return;
				if (movementScript.team != 0 && enemyScript.team == movementScript.team)
					return;
				float damage;
				if (enemyScript.type == 0) {
					health -= 25f;
					damage = 25f;
				} else {
					health -= 5f;
					damage = 5f;
				}
				if (gameObject.activeSelf) {StartCoroutine (InvincibilityRoutine (30f));}
				hit = true;
				otherId = enemyScript.id;
				//GetComponent<PlayerEnemyHandler> ().CreateNumber (transform.position, damage.ToString(), hitCol, .1f, 120f, 3f);
			}

			if (hit)
			{
				GetComponentInChildren<AudioManager> ().Play (1);
				GetComponentInChildren<ParticleController> ().Play (3);
				GetComponent<Rigidbody2D>().MovePosition (transform.position + (Vector3.ClampMagnitude(-transform.position+other.transform.position*10f,.25f)));
			}

			if (health <= 0f) {
				GameManager gameManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ();
				if (GameManager.gameMode == GAMEMODE.PVP) {
					ChildQueueController.player_profiles [id - 1].deaths++;
					ChildQueueController.player_profiles [otherId - 1].kills++;
				}

				GameObject[] playerTemp = GameObject.FindGameObjectsWithTag ("Player"); // Crea Number

				GameObject killer = null;
				for (int i = 0; i < playerTemp.Length; i++) {
					if (playerTemp [i] == null)
						continue;
					if (playerTemp [i].GetComponent<MovementScript> ().player_num == otherId)
					{	
						killer = playerTemp [i];
						break;
					}
				}

				if (killer != null) {
					//killer.GetComponent<PlayerEnemyHandler>().CreateNumber(killer.transform.position,"+10% Damage", Color.red, .05f, 60f, 1f);
				}


				//Buff
				WeaponManager[] temp = FindObjectsOfType<WeaponManager> ();

				for (int i = 0; i < temp.Length; i++) {
					if (temp[i].player_num == otherId)
						temp [i].weapons [0].personalDamageMult += .1f;
				}
				if (GameManager.gameMode == GAMEMODE.PVP)
					gameManager.playerKills [otherId - 1]++;
				else if (GameManager.gameMode == GAMEMODE.PVE) {
					gameManager.enemyMult = Mathf.Floor (gameManager.enemyMult / .5f);
					gameManager.enemyMult *= .5f;
					//gameManager.enemyMult = 1f;
				}
				lives--;

				if (GameManager.gameMode == GAMEMODE.JUGGERNAUT)
				{
					lives = Mathf.Max (lives, 1);
					gameManager.juggernautPoints [otherId-1]++;
				}

				GetComponent<PlayerEnemyHandler> ().Disband ();
				Instantiate (diePrefab, transform.position, Quaternion.identity);
				death.Play ();

				//Juggernaut
				if (GameManager.gameMode == GAMEMODE.JUGGERNAUT)
				{
					if (GetComponent<MovementScript> ().isJuggernaut) {
						killer.GetComponent<MovementScript> ().BecomeJuggernaut();
						GetComponent<MovementScript> ().UnbecomeJuggernaut ();
					}
				}

				if (lives > 0) {
					gameManager.StartCoroutine (gameManager.RespawnPlayer (gameObject));
				} else {
					ChildQueueController.player_profiles [id - 1].losses++;
				}
					stunStack = 0;
					stunText.text = "";
					GetComponentInChildren<DanielShooterScript> ().Reset ();
					GetComponent<WeaponManager> ().input_active = true;
					gameObject.SetActive (false);
			}
		}
	}

	public IEnumerator InvincibilityRoutine(float frames)
	{
		invincibilty = true;
		while (frames > 0) {
			yield return new WaitForEndOfFrame ();
			GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;
			frames--;
		}
		invincibilty = false;
	}

	public void ResetSlider()
	{
		slider.maxValue = health_max;
		slider.value = health;
	}

	public IEnumerator StunSelf()
	{
		MovementScript movScr = GetComponent<MovementScript> ();
		WeaponManager wepMan = GetComponent<WeaponManager> ();
		movScr.move_mult = 0f;
		wepMan.input_active = false;
		yield return new WaitForSeconds (2f);
		wepMan.input_active = true;
		movScr.move_mult = 1f;
		stunText.text = "";
	}

	
}

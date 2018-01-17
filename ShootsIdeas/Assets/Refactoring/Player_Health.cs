using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerUI))]
public class Player_Health : Base_Health {
	[SerializeField] int lives=5;
	[SerializeField] SpriteRenderer playerSprite;

	public int Lives{get { return lives; }}

	PlayerUI playerUI;
	public float regen;
	int regenTimer;

	void Awake()
	{
		playerUI = GetComponent<PlayerUI> ();
		playerUI.ResetHealthUI (health);
		playerUI.UpdateHealthUI (health,armor);
	}

	void Update()
	{
	}

	void FixedUpdate()
	{
		RegenTimer ();
	}

	protected void RegenTimer(){
		regenTimer--;
		if (regenTimer <= 0) {
			if (regen > 0f)
				Heal (regen);
			regenTimer = 60;
		}
	}
	protected virtual void RedSprite()
	{
		Texture2D damageTex = new Texture2D (playerSprite.sprite.texture.width, playerSprite.sprite.texture.height);
		Sprite damageSprite = Sprite.Create (damageTex, playerSprite.sprite.textureRect, playerSprite.sprite.textureRectOffset);
		for (int x = 0; x < damageTex.width; x++) {
			for (int y = 0; y < damageTex.height; y++) {
				if (playerSprite.sprite.texture.GetPixel (x, y).a != 0) {
					damageSprite.texture.SetPixel (x, y, Color.red);
				}
			}
		}
		playerSprite.sprite = damageSprite;
		gameObject.AddComponent<SpriteRenderer> ().sprite = 
			damageSprite;
		GetComponentInChildren<Animator> ().speed = 0f;
	}

	public override void TakeDamage(float damage, int otherId, int otherTeam)
	{
		//RedSprite ();

		if (health <= 0f)
			return;
		if (playerTag.Id == otherId)
			return;
		if (otherTeam != 0 && playerTag.Team == otherTeam)
			return;
		
		if (invincibilty)
			return;

		if (armor > 0f) {
			if (armor > .99f)
				armor = .90f;
			else
				armor -= damage * .005f;
			if (armor < 0f)
				armor = 0f;
			damage *= 1f - armor;
		}

		health -= damage;
		playerUI.UpdateHealthUI (health,armor);
	
		//StartCoroutine (InvincibilityRoutine(30f));
	
		//FX
		NumberSpawner.Instance.CreateNumber (transform.position, Mathf.Round(damage).ToString(),numCol, .1f, 120f, 3f);
		GetComponentInChildren<AudioManager> ().Play (1);
		GetComponentInChildren<ParticleController> ().Play (3);
	
		if (health <= 0f)
			Die (otherId);
	}

	
	public override void Die(int otherId)
	{
		// This behaviour contains player-specific behaviours -- needs to be refactored
		lives--;

		GetComponent<PlayerEnemyHandler> ().Disband (); // DIE
		Instantiate (diePrefab, transform.position, Quaternion.identity);
		GlobalAudioManager.Instance.PlayDeath();
		GetComponent<Base_PowerupManager> ().Reset ();
		StopAllCoroutines ();
		stunStack = 0; // NEW FUNCTION?

		Base_Bullet[] bullets = GetComponentsInChildren<Base_Bullet> (); // Remove all bullets
		for (int i = 0; i < bullets.Length; i++) {
			Destroy (bullets [i].gameObject);
		}

		GameObject killer = GameManager.Instance.GetPlayer(otherId); // Spawn Number
		if (otherId != playerTag.Id && otherId > -1) {
			if (killer != null) {
				NumberSpawner.Instance.CreateNumber (killer.transform.position, "+10% Damage", NUMBER_COL.RED, .05f, 60f, 1f);
				killer.GetComponent<Base_WeaponManager> ().DamageMult += .1f;
			}

			if (GameManager.gameMode == GAMEMODE.PVP) { // GameMode Reactions
				//ChildQueueController.player_profiles [playerTag.Id-1].deaths++;
				//ChildQueueController.player_profiles [otherId-1].kills++;
				//GameManager.Instance.playerKills [otherId-1]++;
			} else if (GameManager.gameMode == GAMEMODE.JUGGERNAUT) {
				/*
			lives = Mathf.Max (lives, 1);
			GameManager.Instance.juggernautPoints [otherId-1]++;
			if (movementScript.isJuggernaut) {
				killer.GetComponent<MovementScript> ().BecomeJuggernaut();
				movementScript.UnbecomeJuggernaut ();
			}
			*/
			} else if (GameManager.gameMode == GAMEMODE.PVE) {
				GameManager.Instance.enemyMult = Mathf.Floor (GameManager.Instance.enemyMult / .5f);
				GameManager.Instance.enemyMult *= .5f;
			}
		}

		if (lives > 0) {
			GameManager.Instance.StartCoroutine (GameManager.Instance.RespawnPlayer (gameObject));
		} else {
			
			//ChildQueueController.player_profiles [playerTag.Id].losses++;
		}

		gameObject.SetActive (false); // FINAL LINE
	}

	public void CreateDead()
	{
		
	}

	public override void Heal(float heal)
	{
		health += heal;
		if (health > health_max)
			health = health_max;
		playerUI.UpdateHealthUI (health, armor);
		NumberSpawner.Instance.CreateNumber (transform.position, Mathf.Round(heal).ToString(),Color.green, .1f, 120f, 3f);
	}

	public override void SetHealth(float _health, float _health_max, float _armor)
	{
		base.SetHealth (_health, _health_max, _armor);
		playerUI.ResetHealthUI (_health_max);
		playerUI.UpdateHealthUI (_health,_armor);
	}


	public override void AddStun()
	{
		if (stun)
			return;
		stunStack++;
		playerUI.UpdateStunUI (stunStack);
		if (stunStack >= 3) {
			StopAllCoroutines ();
			StartCoroutine (StunSelf ());
		} else {
			StopAllCoroutines ();
			StartCoroutine (ReduceStun ());
		}
	}

	public override IEnumerator StunSelf()
	{
		stun = true;
		yield return new WaitForSeconds (1f * Time.timeScale);
		stun = false;
		stunStack = 0;
		playerUI.UpdateStunUI (0);
	}

	public void SetLives_Hard(int lives)
	{
		this.lives = lives;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<ArmorPickup> () != null) {
			PickupRespawn pickup = other.GetComponent<PickupRespawn> ();
			if (!pickup.active)
				return;
			armor += other.GetComponent<ArmorPickup> ().Value; 
			if (armor > 1f)
				armor = 1f;
			pickup.StartRespawnTimer ();
			playerUI.UpdateHealthUI (health, armor);
		}
	}

	IEnumerator ReduceStun()
	{
		while (stunStack > 0) {
			yield return new WaitForSeconds (3f);
			stunStack--;
			playerUI.UpdateStunUI (stunStack);
		}
		StopAllCoroutines ();
	}

	public void ResetHarsh()
	{
		stunStack = 0;
		stun = false;
		playerUI.UpdateStunUI (stunStack);
	}
}

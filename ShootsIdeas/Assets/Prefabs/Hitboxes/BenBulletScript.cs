using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenBulletScript : BulletScript {
	public GameObject myPlayer;

	public override void OnTriggerEnter2D (Collider2D other)
	{
		
		if (other.tag == "Wall") {
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
		EnemyScript enemy = other.GetComponent<EnemyScript> ();
		if (enemy != null) {
			if (enemy.id == id)
				return;
			if (team != 0 && team == enemy.team)
				return;
		}
		
		MovementScript movement = other.GetComponent<MovementScript> ();
		if (movement != null) {
			if (movement.player_num == id)
				return;
			if (team != 0 && team == movement.team)
				return;
		}

		if (movement == null && enemy == null)
			return;
		Instantiate (explosionPrefab, transform.position, Quaternion.identity);
		if (myPlayer != null) {
			HealthScript hs = myPlayer.GetComponent<HealthScript> ();

			float healthPercent = hs.health / hs.health_max;
			float healing;
			if (healthPercent > .50f)
				healing = 1f;
			else {
				healing = damage;
				healing += Mathf.Round ((1f-(hs.health / (hs.health_max/2)))*20f);
			}

			NumberSpawner.Instance.CreateNumber (myPlayer.transform.position, healing.ToString (), new Color (0f, 1f, 0f), .1f, 120f, 2f);
			hs.health += healing;
			if (hs.health > hs.health_max)
				hs.health = hs.health_max;
		}
		Destroy (gameObject);
	}
}

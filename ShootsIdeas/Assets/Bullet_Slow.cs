using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Slow : Base_Bullet {
	[SerializeField] float slowAmount;
	public float SlowAmount {get {return slowAmount;}}

	public override void OnTriggerEnter2D (Collider2D other)
	{
		//RETURN CONDITIONS
		if (other.tag == "Wall") {
			if (deathParticlePrefab != null)
				Instantiate (deathParticlePrefab, transform.position, Quaternion.identity);
			if (destroyOnHit) {
				Destroy (gameObject);
				return;
			}
			return;
		}
		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth == null || otherHealth.GetPlayerTag ().Id == id || (otherHealth.GetPlayerTag().Team != 0 && otherHealth.GetPlayerTag().Team == team))
			return;

		if (canStun)
			otherHealth.AddStun ();

		otherHealth.GetComponent<Base_Movement> ().AddSlow (slowAmount);
		DamageTarget (otherHealth);
		// ELSE CASE??
	}
}

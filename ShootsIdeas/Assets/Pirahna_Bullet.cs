using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pirahna_Bullet : Base_Bullet {
	[SerializeField] float offset;
	[SerializeField] float rate;
	[SerializeField] float depth;
	float angle;
	public bool swim=true;

	void Update()
	{
		if (swim) {
			angle += rate;
			transform.rotation *= Quaternion.Euler (transform.forward * Mathf.Sin (offset + angle) * depth);
			mBody.velocity = transform.right * move_speed;
		}
	}

	public override void OnTriggerEnter2D (Collider2D other)
	{
		Base_Health otherHealth = other.GetComponent<Base_Health> ();

		if (otherHealth == null || otherHealth.GetPlayerTag ().Id == id || (otherHealth.GetPlayerTag().Team != 0 && otherHealth.GetPlayerTag().Team == team))
			return;
	
		if (swim) {
			Pirahna_Bullet[] bullets = FindObjectsOfType<Pirahna_Bullet> ();
			foreach (Pirahna_Bullet bullet in bullets) {
                bullet.ResetKillFlag(Base_Health.KILL_FLAGS.PIRAHNA);
				Rigidbody2D oBody = bullet.GetComponent<Rigidbody2D> ();
				oBody.velocity = Vector3.Normalize (other.transform.position - bullet.transform.position) * move_speed * 5f;
				bullet.transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Atan2 (oBody.velocity.y, oBody.velocity.x)));
				bullet.swim = false;
			}
            killFlag = Base_Health.KILL_FLAGS.NONE;
		}


		if (canStun)
			otherHealth.AddStun ();

		DamageTarget (otherHealth);



		//RETURN CONDITIONS
		/*
		if (other.tag == "Wall") {
			if (deathParticlePrefab != null)
				Instantiate (deathParticlePrefab, transform.position, Quaternion.identity);
			if (destroyOnHit) {
				Destroy (gameObject);
				return;
			}
			return;
		}
		*/
		// ELSE CASE??
	}
}

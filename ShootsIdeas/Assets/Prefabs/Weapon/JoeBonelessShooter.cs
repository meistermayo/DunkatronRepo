using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeBonelessShooter : Weapon{
	public float spreadAmount;

	public override void Attack ()
	{
		/*
		base.Attack ();


		if (currentBullet != null) {
			angle += Mathf.Atan2 (currentBullet.mBody.velocity.y, currentBullet.mBody.velocity.x) * Mathf.Rad2Deg;
		}

		for (int i = 0; i < 2; i++) {
			base.Attack ();
			angle += spreadAmount;
			if (currentBullet != null)
				currentBullet.mBody.velocity = new Vector2 (Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad)) * currentBullet.move_speed;
		}
		*/

		if (!can_attack)
			return;
		GetInput ();
		// Create hitbox, start cooldown coroutine
		transform.parent.GetComponentInChildren<ParticleController> ().Play (0);
		transform.parent.GetComponentInChildren<AudioManager> ().Play (audioClip);

		can_attack = false;


		for (int i = 0; i < 3; i++) {
			GameObject temp = Instantiate (hitbox, transform.position, Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Rad2Deg * Mathf.Atan2 (-v, h))));
			currentBullet = temp.GetComponent<BulletScript> ();
			float angle = Mathf.Atan2 (currentBullet.mBody.velocity.y, currentBullet.mBody.velocity.x) * Mathf.Rad2Deg +( spreadAmount * (i-1));
			currentBullet.team = GetComponentInParent<MovementScript> ().team;
			CircleCollider2D col = temp.GetComponent<CircleCollider2D> ();
			Physics2D.IgnoreCollision (col, GetComponent<CapsuleCollider2D> ());
			currentBullet.id = player_num;
			currentBullet.damage = Mathf.Ceil (currentBullet.damage * (personalDamageMult * damageMult) * damageRed);
			if (damageMult > 1f)
				currentBullet.PowerupParticles ();
			//Rigidbody2D bulletBody = temp.GetComponent<Rigidbody2D> ();
			currentBullet.mBody.velocity = new Vector2 (Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad)) * currentBullet.move_speed;
			if (i == 1) {
				currentBullet.GetComponent<JoeBonelessBulletScript> ().penetrates = true;
				currentBullet.transform.localScale *= 1.5f;
			}
		}
		//Debug.Log ("V: " + v + "\nH: " + h);
		//bulletBody.velocity = Vector2.ClampMagnitude(new Vector2 (h,v) * bulletScript.move_speed,bulletScript.move_speed);
		StartCoroutine (Cooldown());
	}
}

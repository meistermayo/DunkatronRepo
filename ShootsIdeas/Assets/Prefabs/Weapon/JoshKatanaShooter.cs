using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoshKatanaShooter : Weapon {
	public override void Attack ()
	{

		if (!can_attack)
			return;
		GetInput ();
		// Create hitbox, start cooldown coroutine
		transform.parent.GetComponentInChildren<AudioManager> ().Play (audioClip);

		can_attack = false;
		GameObject temp = Instantiate ( hitbox, transform.position, Quaternion.Euler( new Vector3(0f,0f,Mathf.Rad2Deg*Mathf.Atan2(-v,h)) ) );
		currentBullet = temp.GetComponent<BulletScript> ();
		currentBullet.team = GetComponentInParent<MovementScript> ().team;
		CapsuleCollider2D col = temp.GetComponent<CapsuleCollider2D> ();
		Physics2D.IgnoreCollision (col,GetComponent<CapsuleCollider2D>());
		currentBullet.id = player_num;
		currentBullet.damage = Mathf.Ceil(currentBullet.damage*(personalDamageMult * damageMult)*damageRed);
		if (damageMult > 1f)
			currentBullet.PowerupParticles ();
		//Rigidbody2D bulletBody = temp.GetComponent<Rigidbody2D> ();
		currentBullet.GetComponent<JoshKatanaBulletScript>().follow = transform;
		StartCoroutine (Cooldown());
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeBulletScript : BulletScript {
	public Weapon parentWeapon;
	public bool hitSomething;

	public override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Bullet") { // BULLET
			BulletScript bscript = other.GetComponent<BulletScript> ();
			if ((bscript.team != team || team == 0) && bscript.id != id) {
				Destroy (other.gameObject);
				Destroy (gameObject);
				hitSomething = true;
				return;
			}
		}
		if (other.tag == "Wall") { // WALL
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			Destroy (gameObject);
			return;
		}
			
		if (other.tag == "Player") { // PLAYER
			MovementScript movScript = other.GetComponent<MovementScript> ();
			if (movScript.player_num != id && (team == 0 || team != movScript.team
			)) {
				hitSomething = true;
				return;
			}

		}

		EnemyScript enemy = other.GetComponent<EnemyScript> (); // ENEMY
		if (enemy == null)
			return;
		if (enemy.id == id)
			return;
		if (team != 0 && team == enemy.team)
			return;
		
		Instantiate (explosionPrefab, transform.position, Quaternion.identity);
		//float r = Random.Range (0f,1f);
		//float crit = (r < crit_chance) ? 1f+crit_damage : 1f;
		enemy.hp -= damage;// * crit;
		enemy.CreateNumber(enemy.transform.position, damage.ToString(),new Color(1f,.9f,.9f),.1f,120f,2f);
		enemy.particleSystems[1].Play();
		enemy.audioManager.Play (0);
		enemy.StartCoroutine (enemy.FlashRoutine ());
		hitSomething = true;
		Destroy (gameObject);
	}

	void OnDestroy()
	{
		if (hitSomething) {
			if (parentWeapon != null) {
				parentWeapon.can_attack = true;
				parentWeapon.StopAllCoroutines ();
			}
		}
	}
}

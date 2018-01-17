using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BecExplosionBulletScript : BulletScript {
	public override void Awake ()
	{

		int r = Random.Range(0,100);
		if (r < crit_chance) {
			damage *= crit_damage;
			damage = Mathf.Ceil (damage);
		}

		Destroy (gameObject, .016f);
	}

	public override void OnTriggerEnter2D (Collider2D other)
	{

		EnemyScript enemy = other.GetComponent<EnemyScript> ();
		if (enemy == null)
			return;
		if (enemy.id == id)
			return;
		if (team != 0 && team == enemy.team)
			return;

		//float r = Random.Range (0f,1f);
		//float crit = (r < crit_chance) ? 1f+crit_damage : 1f;
		enemy.hp -= damage;// * crit;
		enemy.CreateNumber(enemy.transform.position, damage.ToString(),new Color(1f,.9f,.9f),.1f,120f,2f);
		enemy.particleSystems[1].Play();
		enemy.audioManager.Play (0);
		enemy.StartCoroutine (enemy.FlashRoutine ());
		Destroy (gameObject);
	}
}

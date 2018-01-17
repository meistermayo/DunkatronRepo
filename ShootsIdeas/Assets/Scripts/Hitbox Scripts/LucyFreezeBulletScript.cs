using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LucyFreezeBulletScript : BulletScript {
	public float slowAmount;
	public float slowMax;

	public override void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log ("DAMAGE: " + damage);
		if (other.tag == "Wall") {
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}
		EnemyScript enemy = other.GetComponent<EnemyScript> ();
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
		enemy.move_mult = Mathf.Max (slowMax, enemy.move_mult - slowAmount);
		Destroy (gameObject);

	}

}

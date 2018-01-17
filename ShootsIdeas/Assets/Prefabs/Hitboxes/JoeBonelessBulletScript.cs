using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoeBonelessBulletScript : BulletScript{
	public bool penetrates;

	void Update()
	{
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Atan2 (mBody.velocity.y, mBody.velocity.x) * Mathf.Rad2Deg));
	}

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
		float saveHP = enemy.hp;

		enemy.hp -= damage;// * crit;
		enemy.CreateNumber(enemy.transform.position, damage.ToString(),new Color(1f,.9f,.9f),.1f,120f,2f);
		enemy.particleSystems[1].Play();
		enemy.audioManager.Play (0);
		enemy.StartCoroutine (enemy.FlashRoutine ());

		if (!penetrates)
			Destroy (gameObject);
		else {	
			if (saveHP < damage) {
				damage -= saveHP;
				damage *= .33f;
				damage = Mathf.Round (damage);
				if (damage <= 0f)
					Destroy (gameObject);
			} else {
				Destroy (gameObject);
			}
		}
	}
}

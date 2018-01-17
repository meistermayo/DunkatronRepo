using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsaiahShotgunBulletScript : BulletScript {
	public float duration;
	public GameObject shrapnelPrefab;
	public float spreadAmount;

	void Update()
	{
		duration --;
		if (duration <= 0) {
			for (int i = 0; i < 5; i++) {
				GameObject temp = Instantiate (shrapnelPrefab, transform.position, Quaternion.identity);
				IsaiahShrapnelBulletScript bulletScript = temp.GetComponent<IsaiahShrapnelBulletScript> ();
				bulletScript.id = id;
				bulletScript.mBody.velocity = mBody.velocity;

				float angle = Mathf.Atan2 (bulletScript.mBody.velocity.y, bulletScript.mBody.velocity.x) * Mathf.Rad2Deg;
				angle += Random.Range (-spreadAmount / 2f, spreadAmount / 2f);
				bulletScript.mBody.velocity = new Vector2 (Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad)) * bulletScript.move_speed;
			}
			Destroy (gameObject);
		}
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
	
		if (saveHP < damage) {
			damage -= saveHP;
			damage = Mathf.Round (damage);
			if (damage <= 0f)
				Destroy (gameObject);
		} else {
			Destroy (gameObject);
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamGunBulletScript : BulletScript {

	void Update()
	{
		MovementScript[] temp = FindObjectsOfType<MovementScript> ();
		int minIndex = 0;
		float minDist = 1000f;

		for (int i = 0; i < temp.Length; i++) {
			if (temp [i].player_num != id) {
				if (temp [i].team != team || team == 0) {
					if (Vector3.Distance (transform.position, temp [i].transform.position) < minDist) {
						minDist = Vector3.Distance (transform.position, temp [i].transform.position);
						minIndex = i;
					}
				}
			}
		}

		if (temp.Length > 0)
			mBody.velocity = Vector3.ClampMagnitude (-(transform.position - temp [minIndex].transform.position), move_speed);
		else
			Destroy (gameObject);
	}


	public override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Bullet") {
			if (other.GetComponent<BulletScript> ().id != id)
			if (other.GetComponent<BulletScript> ().team != team || team == 0) {
				Instantiate (explosionPrefab, transform.position, Quaternion.identity);
				Destroy (gameObject);
			}
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
		Destroy (gameObject);

	}
}

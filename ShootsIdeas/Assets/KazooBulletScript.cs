using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KazooBulletScript : BulletScript {
	public float growthRate;
	bool alreadyStunned = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//capCol.size += Vector2.one * growthRate/10f;
		transform.localScale += Vector3.one * growthRate;
	}

	public override void OnTriggerEnter2D (Collider2D other)
	{
		MovementScript movScr = other.GetComponent<MovementScript> ();
		if (movScr != null) {
			if (movScr.player_num == id)
				return;
		}

		if (other.tag == "Wall") {
			Instantiate (explosionPrefab, transform.position, Quaternion.identity);
			Destroy (gameObject);
		}


		if (!alreadyStunned) {
			HealthScript healthScr = other.GetComponent<HealthScript> ();

			if (healthScr != null) {
				if (healthScr.stunStack >= 3)
					return;
				alreadyStunned = true;
				healthScr.stunStack++;
				//Debug.Log ("stun+++: " + healthScr.stunStack);
				healthScr.stunText.gameObject.SetActive (true);
				healthScr.stunText.text += "* ";
				if (healthScr.stunStack > 2) {
					//Debug.Log ("STUN!");
					healthScr.stunText.text = "STUN!";
					healthScr.StartCoroutine (healthScr.StunSelf ());
				}
				//Destroy (gameObject);
			}
		}
		/*
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

*/
	}
}

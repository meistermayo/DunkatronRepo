﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoshKatanaBulletScript : BulletScript {
	public Transform follow;

	public override void Awake ()
	{	
		transform.rotation = Quaternion.Euler (new Vector3(0f,0f,transform.localEulerAngles.z-90f));
		int r = Random.Range(0,100);
		if (r < crit_chance) {
			damage *= crit_damage;
			damage = Mathf.Ceil (damage);
		}
		Destroy (gameObject, life);
	}

	void Update()
	{
		transform.position = follow.position;
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, transform.localEulerAngles.z + 5f));
	}

	public override void OnTriggerEnter2D (Collider2D other)
	{
		Debug.Log ("DAMAGE: " + damage);

		EnemyScript enemy = other.GetComponent<EnemyScript> ();
		if (enemy == null)
			return;
		if (enemy.id == id)
			return;
		if (team != 0 && team == enemy.team)
			return;

		//Instantiate (explosionPrefab, transform.position, Quaternion.identity);
		//float r = Random.Range (0f,1f);
		//float crit = (r < crit_chance) ? 1f+crit_damage : 1f;
		enemy.hp -= damage;// * crit;
		enemy.CreateNumber(enemy.transform.position, damage.ToString(),new Color(1f,.9f,.9f),.1f,120f,2f);
		enemy.particleSystems[1].Play();
		enemy.audioManager.Play (0);
		enemy.StartCoroutine (enemy.FlashRoutine ());
		GetComponentInChildren<Collider2D> ().enabled = false;
				// TODO : Bullet deflect
	}
}

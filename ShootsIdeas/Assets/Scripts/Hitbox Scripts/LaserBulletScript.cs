using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBulletScript : BulletScript{
	List<GameObject> thingsHit;
	void Start()
	{
		thingsHit = new List<GameObject> ();
	
	}

	void Update()
	{
		transform.localScale = new Vector3 (transform.localScale.x, transform.localScale.y-.05f, transform.localScale.z);
	}

	public void RotateParticles()
	{
		ParticleSystem[] parts = GetComponentsInChildren<ParticleSystem> ();
		for (int i = 0; i < parts.Length; i++) {
			ParticleSystem.MainModule fuckUnity = parts [i].main;
			fuckUnity.startRotation = gameObject.transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
		}
	}
	public override void OnTriggerEnter2D (Collider2D other)
	{
		if (thingsHit.Contains (other.gameObject))
			return;

		thingsHit.Add (other.gameObject);

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
	}
}

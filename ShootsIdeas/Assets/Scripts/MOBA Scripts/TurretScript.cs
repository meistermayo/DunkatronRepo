using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TurretScript : MonoBehaviour {
	/* Behaviours: 
	 * Target Enemies
	 * Shoot Enemies
	 * Die
	 * Get Attacked by Enemies
	*/

	GameObject target;

	public GameObject bullet;
	public GameObject aggroText;

	bool isShooting;
	public int team;

	public float attackDuration;

	public float damage;
	public float range;

	void Start()
	{
		Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), GetComponentInChildren<CircleCollider2D> ());
	}

	void Update()
	{
		// Move bullet
		// Check For Enem
	}

	void FixedUpdate()
	{
		
		Debug.Log (isShooting);
		if (target != null)
			Debug.Log ("target: " + target.name);
		if (!isShooting) {
			if (target == null) {				
				
				Collider2D[] contacts = Physics2D.OverlapCircleAll (transform.position, 4f);

				Vector3 minDist = Vector3.one * 9f;
				for (int i = 0; i < contacts.Length; i++) {
					Debug.Log (contacts [i].gameObject.name);
					if (Vector3.Distance (contacts [i].transform.position, transform.position) > range)
						continue;
					if (contacts [i].tag != "Player" && contacts [i].tag != "Enemy")
						continue;
					if (contacts [i].transform.position.magnitude < minDist.magnitude) {
						target = contacts [i].gameObject;
					}
				}

			} else if (Vector3.Distance (target.transform.position, transform.position) > range) {
				target = null;
			}
			
			if (target != null) {
				StartCoroutine (AttackTarget (target));
			}
		}
	}
	/*
	void OnTriggerEnter2D(Collider2D other)
	{
		// take note of target, attack him
		if (target == null) {
			aggroText.SetActive (false);
			if (!isShooting) {	
				target = other.gameObject;
				StartCoroutine (AttackTarget(target));
				target = null;


			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == target) { // if one leaves, find the nearest and attack it.
			target = null;
			Collider2D[] contacts = Physics2D.OverlapCircleAll (transform.position,range);
			Vector3 minDist = Vector3.one * 8f;
			for (int i = 0; i < contacts.Length; i++) {
				if (contacts [i].transform.position.magnitude < minDist.magnitude) {
					target = contacts [i].gameObject;
					StartCoroutine (AttackTarget (target));
				}
			}
		}
	}
*/
	IEnumerator AttackTarget(GameObject target) // passing in a target so we don't lose the reference.
	{
		isShooting = true;
		if (target.tag == "Player")
			aggroText.SetActive (true);
		
		bullet.SetActive (true);
		for (int i=0; i < attackDuration; i++)
		{
			if (target != null)
			bullet.transform.position = transform.position + (-transform.position + target.transform.position) * (i / attackDuration);
			yield return new WaitForSeconds (.016f); // one frame
		}
		bullet.SetActive (false);

		// figure out how to attack the enemy
		if (target.tag == "Player") {
			HealthScript healthScript = target.GetComponent<HealthScript> ();
			healthScript.health -= damage;
		} else if (target.tag == "Enemy") {
			EnemyScript enemyScript = target.GetComponent<EnemyScript> ();
			enemyScript.hp -= damage;
		}
		isShooting = false;
		aggroText.SetActive (false);
	}

	// selecting a target
	/*something enters radius
	 * old target is gone, looking fora  new one
	 * 
	 * removing a target
	 * target dies
	 * target leaves range
	 * */
}

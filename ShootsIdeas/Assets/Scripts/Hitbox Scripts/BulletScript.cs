using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

	public float move_speed;
	public float damage;
	public float crit_chance, crit_damage;
	public float life;
	public int id;
	public Rigidbody2D mBody;
	public GameObject explosionPrefab;
	public int team;

	public virtual void Awake()
	{
		mBody = GetComponent<Rigidbody2D> ();
		mBody.velocity = transform.right * move_speed;

		int r = Random.Range(0,100);
		if (r < crit_chance) {
			damage *= crit_damage;
			damage = Mathf.Ceil (damage);
		}
		
		Destroy (gameObject, life * (1f/ Time.timeScale));
	}

	public virtual void OnTriggerEnter2D(Collider2D other)
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
		Destroy (gameObject);
	}

	public void PowerupParticles()
	{
		transform.GetChild (0).gameObject.SetActive (true);
	}

}

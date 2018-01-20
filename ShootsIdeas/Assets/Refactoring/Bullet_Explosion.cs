using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Bullet_Explosion : Base_Bullet
{
	[SerializeField] float explosionActiveDuration;
	List<GameObject> excludeObjects;
	CircleCollider2D collider;

	void Start()
	{
		if (excludeObjects == null)
			excludeObjects = new List<GameObject> ();
		Camera.main.transform.position += Vector3.down*damage*.005f;
		collider = GetComponent<CircleCollider2D> ();
		StartCoroutine (EndExplosion ());
	}

	public void ExcludeGameObjects(GameObject _gameObject)
	{
		if (excludeObjects == null)
			excludeObjects = new List<GameObject> ();
		excludeObjects.Add (_gameObject);
	}

	public void ExcludeGameObjects(GameObject[] gameObjects)
	{
		if (excludeObjects == null)
			excludeObjects = new List<GameObject> ();
		foreach (GameObject _gameObject in gameObjects) {
			excludeObjects.Add (_gameObject);
		}
	}


	public override void OnTriggerEnter2D (Collider2D other)
	{
		//RETURN CONDITIONS
		if (excludeObjects.Contains (other.gameObject)) {
			return;
		}

		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth == null || (otherHealth.GetPlayerTag().Team != 0 && otherHealth.GetPlayerTag().Team == team))
			return;

		if (canStun)
			otherHealth.AddStun ();
		
		DamageTarget (otherHealth );
		// ELSE CASE??

	}

	protected IEnumerator EndExplosion()
	{
		yield return new WaitForSeconds (explosionActiveDuration);
		Destroy (collider);
	}


	protected override void DamageTarget (Base_Health otherHealth)
	{
		if (!canDamage)
			return;
		//DAMAGE
		float maxDist = collider.radius * transform.localScale.x;
		float dist = Vector3.Distance (transform.position, otherHealth.transform.position);
		float value = (maxDist - dist) / maxDist;

		if (value <= 0f)
			return;
		{
			Rigidbody2D otherBody = otherHealth.GetComponent<Rigidbody2D> ();

			Vector2 diff = (maxDist - dist/maxDist)*(damage * (otherBody.position - mBody.position).normalized)*.25f;
			otherBody.AddForce (diff, ForceMode2D.Impulse);
		}

		int _id = id;
		if (otherHealth.GetPlayerTag ().Id == id)
			_id = -1;
		
		otherHealth.TakeDamage (damage * (value), _id, team);
		excludeObjects.Add (otherHealth.gameObject);
	}
}


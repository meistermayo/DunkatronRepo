using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Explode : Base_Bullet{
	[SerializeField] GameObject explosionPrefab;
	[SerializeField] bool explodeOnHit;
	[SerializeField] bool explodeOnDeath;

	public override void OnTriggerEnter2D (Collider2D other)
	{
		//RETURN CONDITIONS
		if (other.tag == "Wall") {
			if (explodeOnHit) {
				Explode ();
				return;
			}
			return;
		}

		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth == null || otherHealth.GetPlayerTag ().Id == id || (otherHealth.GetPlayerTag().Team != 0 && otherHealth.GetPlayerTag().Team == team))
			return;

		if (canStun)
			otherHealth.AddStun ();
		DamageTarget (otherHealth);
		// ELSE CASE??

		if (explodeOnHit) {
			Explode ();
			return;
		}
	}

	protected override IEnumerator Life (float seconds)
	{
		return base.Life (seconds);
		if (explodeOnDeath) {
			Explode ();
		}
	}

	public void Explode()
	{
		if (GetComponentInChildren<IndependantChildController> () != null)
			GetComponentInChildren<IndependantChildController> ().Detach (1f);
		{
			GameObject temp = Instantiate (explosionPrefab, transform.position, Quaternion.identity) as GameObject;
			Bullet_Explosion explosion = temp.GetComponent<Bullet_Explosion> ();
			explosion.SetInfo (id, team);
		}
		Destroy (gameObject);
	}
}

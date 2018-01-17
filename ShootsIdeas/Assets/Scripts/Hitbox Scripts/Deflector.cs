using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour {
	public int id;

	void Start()
	{
		id = GetComponentInParent<JoshKatanaBulletScript> ().id;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Bullet") {
			if (other.GetComponent<BulletScript> ().id != id) {
				other.GetComponent<BulletScript> ().id = id;
				other.GetComponent<Rigidbody2D> ().velocity *= -1f;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletVelocityAffector : MonoBehaviour {
	Queue<Base_Bullet> bullets;
	[SerializeField] float amount;

	// Use this for initialization
	void Start () {
		bullets = new Queue<Base_Bullet> (10);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		Base_Bullet bullet = other.GetComponent<Base_Bullet> ();
		if (bullet != null) {
			if (bullets.Contains (bullet))
				return;
			bullet.GetComponent<Rigidbody2D> ().velocity *= amount;
			bullets.Enqueue (bullet);
		}
	}
}

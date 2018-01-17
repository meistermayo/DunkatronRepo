using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunBullet : Base_Bullet {
	[SerializeField] int stunNumber;

	public override void OnTriggerEnter2D (Collider2D other)
	{
		if (other.GetComponent<Base_Health> () != null) {
			other.GetComponent<Base_Health> ().AddStun ();
		}
		base.OnTriggerEnter2D (other);
	}
}

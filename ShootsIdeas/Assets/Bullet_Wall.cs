using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Wall : Base_Bullet{
	public override void OnTriggerEnter2D (Collider2D other)
	{
		base.OnTriggerEnter2D (other);
		if (other.GetComponent<Base_Bullet> () != null) {
			Destroy (other.gameObject);
		}
	}
}

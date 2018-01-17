using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana_Bullet : Base_Bullet{
	[SerializeField] float offset;
	[SerializeField] float turnRate;
	List<int> enemies;

	public void Start ()
	{
		enemies = new List<int> ();
		transform.rotation *= Quaternion.Euler (Vector3.forward * offset);
	}

	void Update()
	{
		transform.rotation *= Quaternion.Euler (Vector3.forward * turnRate);
	}

	public override void OnTriggerEnter2D (Collider2D other)
	{
		foreach (int enemy in enemies) {
			if (other.gameObject.GetInstanceID () == enemy)
				return;
		}
		enemies.Add (other.gameObject.GetInstanceID());
		base.OnTriggerEnter2D (other);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletReflector : MonoBehaviour {
	[SerializeField] bool bank;
	[SerializeField] bool vertical;
	Queue<int> enemies;

	void Awake()
	{
		enemies = new Queue<int> (10);
	}

	void OnTriggerEnter2D(Collider2D other)
	{

		Base_Bullet bullet = other.GetComponent<Base_Bullet> ();
		if (bullet == null)
			return;
		if (enemies.Contains (bullet.gameObject.GetInstanceID()))
			return;

		if (bank) {
			if (vertical)
				bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (bullet.GetComponent<Rigidbody2D>().velocity.x,-bullet.GetComponent<Rigidbody2D>().velocity.y);
			else
				bullet.GetComponent<Rigidbody2D> ().velocity = new Vector2 (-bullet.GetComponent<Rigidbody2D>().velocity.x,bullet.GetComponent<Rigidbody2D>().velocity.y);
			
		}
		else
			bullet.transform.rotation *= Quaternion.Euler (new Vector3 (0f,0f,180f));
		
		Base_Bullet parent = GetComponentInParent<Base_Bullet> ();

		if (parent != null)
			bullet.SetInfo (parent.Id, parent.Team);
		
		enemies.Enqueue (other.gameObject.GetInstanceID ());
	}
}

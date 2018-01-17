using UnityEngine;
using System.Collections;

public class OneWayWall : MonoBehaviour
{
	[Range(-1,1)][SerializeField] int horizontalLock;
	[Range(-1,1)][SerializeField] int verticalLock;

	void OnTriggerEnter2D(Collider2D other)
	{
		Base_Bullet bullet = other.GetComponent<Base_Bullet> ();
		if (other.GetComponent<Bullet_Explosion> () != null)
			return;
		if (other.GetComponent<Katana_Bullet> () != null)
			return;
		if (bullet != null) {
			Rigidbody2D body = other.GetComponent<Rigidbody2D> ();

			if (horizontalLock != 0) {
				if (Mathf.Sign (body.velocity.x) == horizontalLock)
					Destroy (other.gameObject);
			}else if (verticalLock!= 0) {
					if (Mathf.Sign (body.velocity.y) == verticalLock)
					Destroy (other.gameObject);
			}
		}
	}
}


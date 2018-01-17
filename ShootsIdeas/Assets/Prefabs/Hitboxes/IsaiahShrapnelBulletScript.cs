using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsaiahShrapnelBulletScript : BulletScript {

	void Update()
	{
		transform.rotation = Quaternion.Euler (new Vector3(0f, 0f, Mathf.Atan2(mBody.velocity.y, mBody.velocity.x)*Mathf.Rad2Deg));
	}
}

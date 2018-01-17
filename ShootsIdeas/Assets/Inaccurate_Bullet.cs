using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inaccurate_Bullet : Base_Bullet {
	[SerializeField] float inaccuracy;

	public override void SetInfo (int _id, int _team)
	{
		transform.rotation *= Quaternion.Euler(Vector3.forward * Random.Range(-inaccuracy,inaccuracy));
		base.SetInfo (_id, _team);
	}
}

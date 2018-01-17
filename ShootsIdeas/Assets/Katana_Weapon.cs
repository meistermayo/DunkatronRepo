using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katana_Weapon : Base_Weapon {
	public override void Attack (float h, float v)
	{
		base.Attack (h, v);
		if (currentBullet != null)
			currentBullet.transform.parent = transform;
	}
}

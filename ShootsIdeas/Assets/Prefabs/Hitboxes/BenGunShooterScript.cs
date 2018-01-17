using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BenGunShooterScript : Weapon {

	public override void Attack ()
	{
		damageMult = 1f;
		base.Attack ();
		if (currentBullet != null)
			currentBullet.GetComponent<BenBulletScript> ().myPlayer = transform.parent.gameObject;
	}
}

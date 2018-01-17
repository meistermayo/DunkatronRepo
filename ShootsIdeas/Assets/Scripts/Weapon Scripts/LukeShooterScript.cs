using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LukeShooterScript : Weapon{

	public override void Attack ()
	{
		base.Attack ();
		if (currentBullet != null)
			currentBullet.GetComponent<LukeBulletScript> ().parentWeapon = this;
	}

}
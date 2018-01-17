using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TedMachineGunWeapon : Weapon {
	public float spreadAmount;

	public override void Attack ()
	{
		base.Attack ();
		if (currentBullet != null) {
			float angle = Mathf.Atan2 (currentBullet.mBody.velocity.y, currentBullet.mBody.velocity.x) * Mathf.Rad2Deg;
			angle += Random.Range (-spreadAmount / 2f, spreadAmount / 2f);
			currentBullet.mBody.velocity = new Vector2 (Mathf.Cos (angle * Mathf.Deg2Rad), Mathf.Sin (angle * Mathf.Deg2Rad)) * currentBullet.move_speed;
		}
	}

}
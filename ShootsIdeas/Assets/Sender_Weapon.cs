using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender_Weapon : Base_Weapon {

	public override void Attack (float h, float v)
	{
		base.Attack (h, v);
		if (currentBullet != null)	
			currentBullet.GetComponent<Bullet_Sender> ().SetSender (this);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spread_Weapon : Base_Weapon {
	[SerializeField] GameObject secondaryBulletPrefab;
	public GameObject SecondHitbox{ get { return secondaryBulletPrefab; } }
	[SerializeField] float spread;
	public float Spread{get {return spread;}}
	[SerializeField] int extraBulletCount;
	public int ExtraBulletCount{ get { return extraBulletCount; } }
	public override void Attack (float h, float v)
	{
		if (ammo <= 0)
			return;
		if (!can_attack)
			return;
		{ // Bullet Stuff
			GameObject saveHitbox = hitbox;
			hitbox = secondaryBulletPrefab;
			float angle = Mathf.Atan2 (v,h) * Mathf.Rad2Deg;
			float newH, newV;

			for (int i = 0; i < extraBulletCount; i++) {
				angle += spread;

				newH = Mathf.Cos (angle * Mathf.Deg2Rad);
				newV = Mathf.Sin (angle * Mathf.Deg2Rad);
				CreateBullet (newH, newV);
				angle -= spread * 2f;

				newH = Mathf.Cos (angle * Mathf.Deg2Rad);
				newV = Mathf.Sin (angle * Mathf.Deg2Rad);
				CreateBullet (newH, newV);
			}
			hitbox = saveHitbox;
		}
		base.Attack (h, v);
	}
}

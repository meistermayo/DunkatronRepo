using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamGunShooter : Weapon{
	public float baseCooldown;
	public float maxCooldown;
	void Start()
	{
		//maxCooldown = cooldown * 100f;
		baseCooldown = cooldown;
	}

	public override void Attack ()
	{
		cooldown = Mathf.Min(cooldown + .2f,maxCooldown);
		base.Attack ();
	}

	public override void Update()
	{
		if (can_attack) {
			cooldown = Mathf.Max (baseCooldown, cooldown - .5f);
		}
	}

}

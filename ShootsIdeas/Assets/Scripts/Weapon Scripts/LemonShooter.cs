using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemonShooter : Weapon {

	public int stack = 5;
	float stackTimer = 20f;

	public override void Attack ()
	{
		if (stack == 0)
			return;
		if (can_attack)
			stack--;
		base.Attack ();
	}
	/*
	public override void Update ()
	{
		base.Update();
	}
*/
	public void FixedUpdate()
	{
		StackTimer ();
	}

	public void StackTimer()
	{
		if (stackTimer <= 0f) {
			if (stack < 5) {
				stack++;
				stackTimer = 20f * attackSpeedRed;
			}
		} else
			stackTimer--;
	}
}

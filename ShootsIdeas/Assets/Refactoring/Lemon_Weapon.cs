using UnityEngine;
using System.Collections;

public class Lemon_Weapon : Base_Weapon
{
	int stack;
	[SerializeField] int stackMax;
	[SerializeField] float stackCooldown;

	protected override void Awake ()
	{
		base.Awake ();
		stack = stackMax;
		StartCoroutine (StackCooldown ());
	}


	public IEnumerator StackCooldown()
	{
		while (true) {
			yield return new WaitForSeconds (stackCooldown);
			stack++;
			if (stack > stackMax)
				stack = stackMax;
		}
	}

	public override void Attack (float h, float v)
	{
		if (stack <= 0)
			return;
		base.Attack (h, v);
	}

	public override IEnumerator Cooldown ()
	{
		stack--;

		float _cooldown = (cooldown * wepMan.AttackSpeedRed);
		yield return new WaitForSeconds (_cooldown*(1f/Time.timeScale));
		can_attack = true;
	}

	public override void ResetCooldown ()
	{
		base.ResetCooldown ();
		StartCoroutine (StackCooldown ());
		stack = stackMax;
	}
}


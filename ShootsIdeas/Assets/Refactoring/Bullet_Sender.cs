using UnityEngine;
using System.Collections;

public class Bullet_Sender : Base_Bullet
{
	protected Base_Weapon sender;

	public void SetSender(Base_Weapon weapon)
	{
		sender = weapon;
	}

	public virtual void CallSender()
	{
		sender.ResetCooldown ();
	}


	public override void OnTriggerEnter2D (Collider2D other)
	{
		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth!= null) {
			if (otherHealth.GetPlayerTag().Id != id)
			if (otherHealth.GetPlayerTag().Team == 0 || otherHealth.GetPlayerTag().Team != team)
			if (canDamage)
				CallSender ();
		}
		base.OnTriggerEnter2D (other);
	}
}


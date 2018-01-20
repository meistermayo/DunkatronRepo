using UnityEngine;
using System.Collections;

public class Bullet_Leech : Bullet_Sender
{
	[SerializeField] float healAmount;
	public float HealAmount { get {return healAmount;}}
	public override void CallSender()
	{
			Base_Health otherHealth = sender.GetComponent<Base_Health> ();
			if (otherHealth == null) {
				otherHealth = sender.GetComponentInParent<Base_Health> ();
			}
			if (otherHealth != null)
				otherHealth.Heal (healAmount);
		
	}
}


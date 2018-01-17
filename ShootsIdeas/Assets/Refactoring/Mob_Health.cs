using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mob_Health : Base_Health
{
	Text hpText;

	void Start()
	{
		hpText = GetComponentInChildren<Text> ();
	}

	public override void TakeDamage (float damage, int otherId, int otherTeam)
	{
		Debug.Log (invincibilty);
		if (invincibilty)
			return;
		base.TakeDamage (damage, otherId, otherTeam);
		hpText.text = Mathf.Round (health).ToString();
	}

	public override void Die (int otherId)
	{
		GetComponent<Base_Mob> ().RemoveFromParent ();
		base.Die (otherId);
	}

}


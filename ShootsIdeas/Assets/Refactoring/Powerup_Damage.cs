using UnityEngine;
using System.Collections;

public class Powerup_Damage : Powerup
{
	public override void Buff ()
	{
		GetComponent<Base_WeaponManager> ().DamageMult += buffValue;
		GetComponentInChildren<ParticleController>().transform.Find ("DamageBuffSpark").GetComponent<ParticleSystem> ().Play ();
	}

	public override void DeBuff()
	{
		GetComponent<Base_WeaponManager> ().DamageMult -= buffValue;
		GetComponentInChildren<ParticleController>().transform.Find ("DamageBuffSpark").GetComponent<ParticleSystem> ().Stop ();
	}
}


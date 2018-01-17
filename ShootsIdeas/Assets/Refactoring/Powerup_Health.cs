using UnityEngine;
using System.Collections;

public class Powerup_Health : Powerup
{
	public override void Buff ()
	{
		GetComponent<Player_Health> ().regen += buffValue;
		GetComponentInChildren<ParticleController>().transform.Find ("RegenSpark").GetComponent<ParticleSystem> ().Play ();
	}

	public override void DeBuff()
	{
		GetComponent<Player_Health> ().regen -= buffValue;
		GetComponentInChildren<ParticleController>().transform.Find ("RegenSpark").GetComponent<ParticleSystem> ().Stop ();
	}
}


using UnityEngine;
using System.Collections;

public class Powerup_Movement : Powerup
{
	public override void Buff ()
	{
		GetComponent<Base_Player_Movement> ().AddMovement(buffValue);
		GetComponentInChildren<ParticleController>().transform.Find ("MovementSpark").GetComponent<ParticleSystem> ().Play ();
	}

	public override void DeBuff()
	{
		GetComponent<Base_Player_Movement> ().ResetMovementValues();
		GetComponentInChildren<ParticleController>().transform.Find ("MovementSpark").GetComponent<ParticleSystem> ().Stop ();
	}
}


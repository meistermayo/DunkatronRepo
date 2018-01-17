using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum POWERUP_TYPE{
	DAMAGE,
	SPEED,
	HEALTH
}
public class Base_PowerupManager : MonoBehaviour {
	//ParticleController particleController;

	void Awake()
	{
		//particleController = GetComponentInChildren<ParticleController> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (GetComponent<PlayerController> ().IsJuggernaut)
			return;
		if (GetComponent<PlayerEnemyHandler> ().CheckForPanko ())
			return;
		PickupRespawn pickupRespawn = other.GetComponent<PickupRespawn> ();
		if (pickupRespawn == null)
			return;
		if (!pickupRespawn.active)
			return;
		
		Powerup otherPowerup = other.GetComponent<Powerup> ();
		if (otherPowerup == null)
			return;
		{
			Powerup currentPowerup = GetComponent<Powerup> ();
			if (currentPowerup != null) {
				if (currentPowerup.CanStack) {
					if (!otherPowerup.CanStack)
						return;
				} else
					return;
			}
		}
		Powerup thisPowerup = null;

		switch (otherPowerup.type) {
		case POWERUP_TYPE.DAMAGE:
			thisPowerup = gameObject.AddComponent<Powerup_Damage> ();
			break;
		case POWERUP_TYPE.SPEED:
			thisPowerup = gameObject.AddComponent<Powerup_Movement> ();
			break;
		case POWERUP_TYPE.HEALTH:
			thisPowerup = gameObject.AddComponent<Powerup_Health> ();
			break;
		}
		if (thisPowerup == null)
			return;
		thisPowerup.Copy (otherPowerup);
		StartCoroutine (UsePowerup(thisPowerup));
		pickupRespawn.StartRespawnTimer ();
	}

	public void Reset()
	{
		StopAllCoroutines ();
		Powerup[] stopPowerups = GetComponents<Powerup> ();
		foreach (Powerup stopPowerup in stopPowerups) {
			stopPowerup.DeBuff ();
			Destroy (stopPowerup);
		}
	}

	public IEnumerator UsePowerup(Powerup thisPowerup)
	{
		thisPowerup.Buff ();
		yield return new WaitForSeconds (thisPowerup.Duration);
		if (thisPowerup != null)
			thisPowerup.DeBuff ();
		Destroy (thisPowerup);
	}
}
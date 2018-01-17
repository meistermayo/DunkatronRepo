using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndependantChildController : MonoBehaviour {
	public void Detach()
	{
		GetComponent<ParticleSystem> ().Stop ();
		transform.parent = null;
	}

	public void Detach(float dieInSeconds)
	{
		GetComponent<ParticleSystem> ().Stop ();
		transform.parent = null;
		Destroy (gameObject, dieInSeconds);
	}
}

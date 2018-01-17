using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleKiller : MonoBehaviour {

	ParticleSystem part;
	public AudioSource noise;

	void Awake()
	{
		part = GetComponentInChildren<ParticleSystem> ();
		if (noise != null)
			noise.Play ();
		//GameObject.FindGameObjectWithTag ("GameController").GetComponent<AudioSource> ().Play ();
	}

	void Update()
	{
		if (part.isStopped)
			Destroy (gameObject);
	}

}

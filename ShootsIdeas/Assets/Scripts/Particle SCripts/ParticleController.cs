using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour {

	public ParticleSystem shootSparks;
	public ParticleSystem movementPowerupSparks;
	public ParticleSystem damagePowerupSparks;

	public ParticleSystem hurtSparks;
	public ParticleSystem juggSparks;
	public ParticleSystem regenSparks;

	MovementScript movScript;
	float h,v;

	void Awake()
	{
		movScript = GetComponentInParent<MovementScript> ();
	}

	void Update()
	{
		//h = Input.GetAxis ("h2_" + movScript.player_num);
		//v = -Input.GetAxis ("v2_" + movScript.player_num);
		
		//shootSparks.gameObject.transform.rotation = Quaternion.Euler (new Vector3 (0f,0f,Mathf.Atan2(v,h)*Mathf.Rad2Deg));
	}

	public void Play(int p)
	{
		if (p == 0)
			shootSparks.Play ();
		else if (p == 1)
			damagePowerupSparks.Play ();
		else if (p == 2)
			movementPowerupSparks.Play ();
		else if (p == 3)
			hurtSparks.Play ();
		else if (p == 4)
			juggSparks.Play ();
		else if (p == 5)
			regenSparks.Play ();
	}

	public void Stop(int p)
	{
		if (p == 0)
			shootSparks.Stop ();
		else if (p == 1)
			damagePowerupSparks.Stop ();
		else if (p == 2)
			movementPowerupSparks.Stop ();
		else if (p == 3)
			hurtSparks.Stop ();
		else if (p == 4)
			juggSparks.Stop();
		else if (p == 5)
			regenSparks.Stop();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	AudioSource[] clips;

	void Awake()
	{
		clips = GetComponents<AudioSource> ();
	}

	public void Play(int a)
	{
		if (clips.Length == 0)
			return;
		/*
		AudioSource[] _as = FindObjectsOfType<AudioSource> ();
		for (int i = 0; i < _as.Length; i++) {
			if (_as[i].clip.name != "Death")
			if (_as[i].clip.name != "GameMusic1_0")
			if (_as[i].clip.name != "MenuMusic")
			if (_as[i].clip.name != "JugMusic")
			if (_as[i].clip.name != "RobotronRemix")
			if (_as[i].clip.name != "RobotWave")
				_as [i].Stop ();
		}
		*/
		if (a < 0 || a >= clips.Length)
			a = 0;
		clips [a].Play ();
	}
		
	public void Stop(int a)
	{
		clips [a].Stop ();
	}


}

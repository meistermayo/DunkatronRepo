using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoopMusic : MonoBehaviour {
	AudioSource[] musics;
	// Use this for initialization
	void Start () {
		musics = GetComponents<AudioSource> ();
		if (GameManager.robotronLike)
			musics [1].Play ();
		else
			musics [0].Play ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

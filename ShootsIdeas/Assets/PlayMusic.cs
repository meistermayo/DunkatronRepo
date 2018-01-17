using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour {

	[SerializeField] AudioClip music;

	void Start()
	{
		GlobalAudioManager.Instance.PlayMusic (music);
	}
}

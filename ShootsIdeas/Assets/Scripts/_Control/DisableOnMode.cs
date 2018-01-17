using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnMode : MonoBehaviour {
	public bool isEnabled;
	public GAMEMODE type;
	// Use this for initialization
	void Start () {
			if (GameManager.gameMode == type)
				gameObject.SetActive (isEnabled);
			else
				gameObject.SetActive (!isEnabled);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuWallMessenger : MonoBehaviour {

	RoomController_Refactored myRoom;
	public bool isPlay;
	// Use this for initialization
	void Awake () {
		myRoom = GetComponentInParent<RoomController_Refactored> ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			if (isPlay)
				myRoom.SetReady ();
			else
				myRoom.SetExit ();
		}
	}
}

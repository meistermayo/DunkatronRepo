using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SecondaryCapture : CapturePoint {
	public POWERUP_TYPE powerup_type;
	public float secondsWait;

	public override void Start()
	{
		base.Start ();
		SetColors (0);
	}

	public override void PointCaptured()
	{
		/*
		for (int i = 0; i < 4; i++) {
			if (GameManager.Instance.GetPlayer(i) == null)
				continue;
			
			MovementScript movScr = GameManager.Instance.GetPlayer(i).GetComponent<MovementScript> ();
			if (captureId == 0) {
				if (movScr.player_num == capturePId) {
					PowerupManager powMan = movScr.GetComponent<PowerupManager> ();
					StartCoroutine (powMan.PowerupDuration (powerup_type));
					powMan.GetComponentInChildren<AudioManager> ().Play ((int)powerup_type + 2);

				}
			} else if (movScr.team == captureId) {
				PowerupManager powMan = movScr.GetComponent<PowerupManager> ();
				StartCoroutine (powMan.PowerupDuration (powerup_type));
				powMan.GetComponentInChildren<AudioManager> ().Play ((int)powerup_type + 2);

			}
		}
		captureAmount = 0;
		captureId = -1;
		StartCoroutine (ResetPoint ());
		*/
	}


	public IEnumerator ResetPoint()
	{
		yield return new WaitForSeconds (secondsWait);
		captureAmount = 0f;
		captureId = 0;
		SetColors (0);
	}
}

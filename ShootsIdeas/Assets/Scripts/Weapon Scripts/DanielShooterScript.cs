using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanielShooterScript : Weapon{
	public float startdelay;
	public GameObject laserSightGO;
	public bool coroutineHasStarted = false;
	public int audioClipAim;
	float saveH, saveV;

	public override void Attack ()
	{
		if (can_attack && !coroutineHasStarted) {
			transform.parent.GetComponentInChildren<AudioManager> ().Play (audioClipAim);
			coroutineHasStarted = true;
			StartCoroutine (AttackRoutine ());
		}
	}
		
	IEnumerator AttackRoutine()
	{
		MovementScript movMan = GetComponentInParent<MovementScript> ();
		WeaponManager wepMan = GetComponentInParent<WeaponManager> ();
		laserSightGO.SetActive (true);
		movMan.move_mult = .5f;
		for (float f = 0; f < startdelay*attackSpeedRed; f += 1f) {
			Debug.Log ("RUNNING: " + f);
			if (wepMan.iWeapon != 0) {
				if (h != 0f || v != 0f) {
					transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f, Mathf.Atan2 (-v, h) * Mathf.Rad2Deg));
					saveH = h;
					saveV = v;
				}
			} 
			transform.localScale = new Vector3 (transform.localScale.x,transform.localScale.y,transform.localScale.z);
			yield return new WaitForSeconds (.016f);
		}
		movMan.move_mult = 1f;
		updateInput = (wepMan.iWeapon != 0);
		h = saveH;
		v = saveV;
		base.Attack ();
		updateInput = true;
		if (currentBullet != null) {
			currentBullet.GetComponent<LaserBulletScript> ().RotateParticles ();
		}
		Debug.Log ("Ended");
		laserSightGO.SetActive (false);
		coroutineHasStarted = false;
	}

	//Meme
	public void Reset()
	{
		laserSightGO.SetActive (false);
		coroutineHasStarted  = false;
		StopCoroutine ("AttackRoutine");
		can_attack = true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sniper_Weapon : Base_Weapon {
	[SerializeField] float moveSpeedReduction;
	[SerializeField] float chargeTime;
	[SerializeField] AudioClip laserSound;
	SpriteRenderer laserSprite;
	Base_Player_Movement movementScript;
	float rH,rV;

	protected override void Awake ()
	{
		base.Awake ();
		movementScript = GetComponentInParent<Base_Player_Movement> ();
		laserSprite = GetComponentInChildren<SpriteRenderer> ();
	}

	public override void Attack (float h, float v)
	{
		rH = h;
		rV = v;
		transform.rotation = Quaternion.Euler (new Vector3 (0f, 0f,Mathf.Atan2 (-v, h) * Mathf.Rad2Deg));

		if (!can_attack)
			return;
		if (ammo <= 0)
			return;
		ammo--;
		can_attack = false;

		StartCoroutine (Charge (h,v));
	}

	public IEnumerator Charge(float h, float v)
	{
		GlobalAudioManager.Instance.PlaySound (laserSound);
		laserSprite.enabled = true;
		movementScript.AddMovement (-moveSpeedReduction);
		movementScript.ResetMovementValues ();
		yield return new WaitForSeconds (chargeTime);
		//PlayFX();
		CreateBullet (rH,rV);
		currentBullet.transform.parent = transform;
		laserSprite.enabled = false;
		StartCoroutine (Cooldown());
	}

	public override void ResetCooldown ()
	{
		base.ResetCooldown ();
		StopCoroutine ("Charge");
		laserSprite.enabled = false;
	}

	public override void RefreshGun ()
	{
		base.RefreshGun ();
		laserSprite.enabled = false;
	}
}

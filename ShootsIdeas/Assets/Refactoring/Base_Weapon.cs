using UnityEngine;
using System.Collections;

[System.Serializable]
[RequireComponent(typeof(FXPlayer))]
public class Base_Weapon : PlayerScript
{
	[SerializeField] protected GameObject hitbox;
	public GameObject Hitbox { get { return hitbox; } }
	[SerializeField] protected float cooldown;
	public float GetCooldown {get {return cooldown;}}
	[SerializeField] protected int maxAmmo;
	[SerializeField] protected bool unlimitedAmmo;
	protected int ammo;
	public int Ammo { get { return ammo; } }
	public int MaxAmmo { get { return maxAmmo; } }
	public bool HasUnlimitedAmmo { get {return unlimitedAmmo;}}
	public bool IsFull {get {return ammo >= maxAmmo;}}

	protected float damageMult = 1f;

	protected bool can_attack = true;
	protected bool updateInput = true;

	protected Base_Bullet currentBullet; // ???


	protected Base_WeaponManager wepMan;

	protected virtual void Awake()
	{
		if (unlimitedAmmo)
			ammo = 1;
		else
			ammo = maxAmmo;
		wepMan = GetComponentInParent<Base_WeaponManager> ();
		if (wepMan == null)
			wepMan = GetComponent<Base_WeaponManager> ();
	}

	public virtual void Update()
	{
	}

	public virtual void Attack(float h, float v)
	{
		if (!can_attack)
			return;
		if (ammo <= 0)
			return;
		if (!unlimitedAmmo)
			ammo--;
		
		can_attack = false;

		PlayFX();
		CreateBullet (h,v);
		StartCoroutine (Cooldown());
	}
		
	public virtual IEnumerator Cooldown()
	{
		float _cooldown = (cooldown );
		if (wepMan != null)
			cooldown *= wepMan.AttackSpeedRed;
		yield return new WaitForSeconds (_cooldown);
		can_attack = true;
	}

	void PlayFX()
	{
		
		if (damageMult == 1f)
			fxPlayer.Play (FX_TYPE.FIRE_BULLET); 
		else
			fxPlayer.Play (FX_TYPE.FIRE_BULLET_BUFFED);

	}

	protected GameObject CreateBullet(float h, float v)
	{
		GameObject BulletObject = InstantiateBullet (h,v);
		MakeBulletFriendly (BulletObject);
		BuffBullet ();
		return BulletObject;
	}

	protected GameObject InstantiateBullet(float h, float v)
	{
		GameObject BulletObject = Instantiate ( hitbox, transform.position  + new Vector3(h,-v,0f)*.25f, Quaternion.Euler( new Vector3(0f,0f,Mathf.Rad2Deg*Mathf.Atan2(-v,h)) ) );
		currentBullet = BulletObject.GetComponent<Base_Bullet> ();
		currentBullet.
		SetInfo (
			GetPlayerTag().Id,
			GetPlayerTag().Team);
		return BulletObject;
	}

	void MakeBulletFriendly(GameObject BulletObject)
	{
		Collider2D col1 = BulletObject.GetComponent<Collider2D> ();
		Collider2D col2 = GetComponent<Collider2D> ();
		if (col1 != null && col2 != null)
			Physics2D.IgnoreCollision (col1,col2);
	}

	void BuffBullet()
	{
		if (wepMan == null)
			return;
		if (damageMult > 1f)
			currentBullet.PowerupParticles ();
		currentBullet.
		MultiplyDamage(damageMult*wepMan.DamageMult*wepMan.DamageRed);
	}

	public virtual void ResetCooldown()
	{
		StopCoroutine ("Cooldown");
		can_attack = true;
	}

	public virtual void RefreshGun()
	{
		ResetCooldown ();
		ammo = maxAmmo;

	}

	public void AddAmmo(int ammo)
	{
		this.ammo += ammo;
		if (this.ammo > maxAmmo)
			this.ammo = maxAmmo;
	}
}


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Base_WeaponManager : PlayerScript
{
	[SerializeField] public Base_Weapon primaryWeapon,secondaryWeapon,currentWeapon;
	[SerializeField] bool input_active=true;
	[SerializeField] int iWeapon=0;
	[SerializeField] int priWepInd, secWepInd=0;
	[SerializeField] PlayerUI playerUI;

	[SerializeField] protected float damageMult = 1f;
	public float DamageMult{
		get{ return damageMult; }
		set {damageMult = value;}
	}
	[SerializeField] protected float damageRed = 1f;
	public float DamageRed{
		get{ return damageRed; }
		set {damageRed = value;}
	}
	[SerializeField] protected float attackSpeedRed = 1f;
	public float AttackSpeedRed{
		get{ return attackSpeedRed; }
		set {attackSpeedRed =  value;}
	}

	void Awake()
	{
		playerUI = GetComponent<PlayerUI> ();
		primaryWeapon = GetComponent<Base_Weapon> ();
		secondaryWeapon = GetComponentInChildren<Base_Weapon> ();
		currentWeapon = primaryWeapon;

		if (playerUI != null) {
			playerUI.ShowAmmo (!currentWeapon.HasUnlimitedAmmo);
			playerUI.UpdateAmmoCounter (currentWeapon.Ammo, currentWeapon.MaxAmmo);
		}

	}

	public void RotateWeapons()
	{
		if (currentWeapon == primaryWeapon) {
			if (secondaryWeapon != null)
				currentWeapon = secondaryWeapon;
		} else
			currentWeapon = primaryWeapon;

		UpdateAmmoUI (currentWeapon.Ammo, currentWeapon.MaxAmmo,!currentWeapon.HasUnlimitedAmmo);
	}


	public void Attack(float h,float v)
	{
		currentWeapon.Attack (h, v);
		UpdateAmmoUI (currentWeapon.Ammo,currentWeapon.MaxAmmo);
	}

	public void ResetWeapons()
	{
		primaryWeapon.ResetCooldown ();
		if (secondaryWeapon != null)
			secondaryWeapon.ResetCooldown ();
	}

	public void RefreshWeapons()
	{
		primaryWeapon.RefreshGun ();
		if (secondaryWeapon != null)
			secondaryWeapon.RefreshGun();
		UpdateAmmoUI (currentWeapon.Ammo, currentWeapon.MaxAmmo);
	}

	public void EquipSecondWeapon()
	{
		secondaryWeapon = GetComponentInChildren<Base_Weapon> ();
	}

	void OnTriggerStay2D(Collider2D other)
	{
		PickupRespawn pickup = other.GetComponent<PickupRespawn> ();
		if (pickup == null)
			return;
		if (!pickup.active)
			return;
		pickup.SetAButton (true);
		if (other.GetComponent<AmmoPickup> () != null) {
			PickUpAmmo (pickup);
		} else if (other.GetComponent<PickupGun> () != null) {
			if (Input.GetButtonDown ("aButton_" + playerTag.Id.ToString ())) {
				PickUpWeapon (pickup);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		PickupRespawn pickup = other.GetComponent<PickupRespawn> ();
		if (pickup == null)
			return;
		if (!pickup.active)
			return;
		pickup.SetAButton (false);
	}

	void PickUpAmmo(PickupRespawn pickup)
	{
		int extraAmmo = pickup.GetComponent<AmmoPickup> ().Value; 

		Base_Weapon ammoWeapon = currentWeapon; // if lemon -- refill secondary : if weapon is full, return, else fill.
		if (currentWeapon.HasUnlimitedAmmo)
			ammoWeapon = (currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon);

		if (ammoWeapon.IsFull)
			return;

		ammoWeapon.AddAmmo (extraAmmo);

		if (playerUI != null)
			playerUI.UpdateAmmoCounter (currentWeapon.Ammo, currentWeapon.MaxAmmo);
		
		pickup.StartRespawnTimer ();
	}

	void PickUpWeapon(PickupRespawn pickup)
	{

		if (currentWeapon == primaryWeapon)
			primaryWeapon = null;
		else 
			secondaryWeapon = null;
		
		Destroy (currentWeapon);
		currentWeapon = null;
		{
			GameObject temp = Instantiate (pickup.GetComponent<PickupGun> ().gun, transform);
			currentWeapon = temp.GetComponent<Base_Weapon> ();
		}

		if (primaryWeapon == null)
			primaryWeapon = currentWeapon;
		else 
			secondaryWeapon = currentWeapon;

		currentWeapon.GetComponent<PlayerTag>().SetId(GetPlayerTag().Id);
		currentWeapon.GetComponent<PlayerTag>().SetTeam(GetPlayerTag().Team);

		UpdateAmmoUI (currentWeapon.Ammo, currentWeapon.MaxAmmo,!currentWeapon.HasUnlimitedAmmo);

		pickup.StartRespawnTimer ();
	}

	public void UpdateAmmoUI(int ammo, int maxAmmo)
	{
		if (playerUI != null) {
			playerUI.ShowAmmo (!currentWeapon.HasUnlimitedAmmo);
			playerUI.UpdateAmmoCounter (ammo,maxAmmo);
		}
	}

	public void UpdateAmmoUI(int ammo, int maxAmmo, bool unlimited)
	{
		if (playerUI != null) {
			playerUI.ShowAmmo (unlimited);
			playerUI.UpdateAmmoCounter (ammo,maxAmmo);
		}
	}
	
}


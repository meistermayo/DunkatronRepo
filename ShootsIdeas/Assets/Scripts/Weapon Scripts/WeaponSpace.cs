using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponSpace{
	
	public class Wep{
		public string name;
		public float damage;
		public float crit_chance, crit_damage;
		public float speed;
		public float cooldown;
		public int ammo, ammo_capacity;

		public Wep(string name, float damage, float crit_chance, float crit_damage, float speed, float cooldown, int ammo){
			this.name = name;
			this.damage = damage;
			this.crit_chance = crit_chance;
			this.crit_damage = crit_damage;
			this.speed = speed;
			this.cooldown = cooldown;
			this.ammo = ammo;
			this.ammo_capacity = ammo;
		}

		public virtual void Fire()
		{
			//base fire
		}
	}

	/*public class Swep : Wep{
		public override void Fire ()
		{
			base.Fire ();
			// do whatever else you want here
		}
	}*/

	public class WeaponIni{
		public static List<Wep> WepList;

		public void IniWeapons()
		{
			WepList.Add(new Wep("Lemon Shooter", 25f, 40f, 1.5f, 1f, 60f, 5));
		}
	}

}

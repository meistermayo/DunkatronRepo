using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum DPSCALC_MODE
{
	DPS = 0,
	TOTAL,
	ALL
}
public class DpsCalculator : MonoBehaviour {
	[SerializeField] Text dpsText;
//	[SerializeField] float damage, crit_chance, crit_damage, attack_speed;
	[SerializeField] GameObject[] weapons;
	[SerializeField] DPSCALC_MODE mode;
	Base_Weapon[] weaponScripts;
	GameObject[] bulletObjects;
	Base_Bullet[] bulletScripts;

	int index;
	string[] names;
	string[] dps_strings;
	float[] dps_floats;

	[SerializeField] bool SortDpsButton;
	[SerializeField] bool ReHash;

	void Awake()
	{
		weaponScripts = new Base_Weapon[weapons.Length];
		bulletObjects = new GameObject[weapons.Length];
		bulletScripts = new Base_Bullet[weapons.Length];

		names = new string[weapons.Length];
		dps_strings = new string[weapons.Length];
		dps_floats = new float[weapons.Length];

		for (int i = 0; i < weapons.Length; i++) {
			weaponScripts[i] = weapons [i].GetComponent<Base_Weapon> ();
			bulletObjects[i] = weaponScripts [i].Hitbox;
			bulletScripts[i] = bulletObjects [i].GetComponent<Base_Bullet> ();
			names [i] = weapons [i].name;
		}

		dpsText.text = CalculateAllDPS();
	}

	void Update()
	{
		if (SortDpsButton) {
			SortDpsButton = false;
			SortStrings ();
		}
		if (ReHash) {
			ReHash = false;
			dpsText.text = CalculateAllDPS();
		}
	}

	string CalculateDPS(int i)
	{
		float base_total = 0f;
		string output = "DPS: <color=#fffffffff>";
		string grade = "";
		if (mode == DPSCALC_MODE.TOTAL)
			dps_floats [i] = bulletScripts [i].Damage * (1f + (bulletScripts [i].Crit_Chance / 100f * (bulletScripts [i].Crit_Damage - 1f))) * Mathf.Round ((float)weaponScripts [i].MaxAmmo);
		else if (mode == DPSCALC_MODE.DPS)
			dps_floats [i] = bulletScripts [i].Damage * (1f + (bulletScripts [i].Crit_Chance / 100f * (bulletScripts [i].Crit_Damage - 1f))) * (1f / weaponScripts [i].GetCooldown);
		else {
			dps_floats [i] = bulletScripts [i].Damage * (1f + (bulletScripts [i].Crit_Chance / 100f * (bulletScripts [i].Crit_Damage - 1f))) * (1f / weaponScripts [i].GetCooldown);
			dps_floats [i] += bulletScripts [i].Damage * (1f + (bulletScripts [i].Crit_Chance / 100f * (bulletScripts [i].Crit_Damage - 1f))) * Mathf.Round ((float)weaponScripts [i].MaxAmmo);
			if (bulletScripts[i].Move_Speed != 0f)
				dps_floats [i] *= bulletScripts [i].Move_Speed/2f;
			base_total = dps_floats [i];
			dps_floats [i] /= 15222.8f;

			dps_floats [i] += GradeSlow (i);
			dps_floats [i] += GradeSpread (i);
			dps_floats [i] += GradeInfinite (i);
			dps_floats [i] += GradeBlock (i);
			dps_floats [i] += GradeReflect (i);
			dps_floats [i] += GradeTranscendant(i);
			dps_floats [i] += GradeHeal (i);
			dps_floats [i] += GradeStun (i);
			dps_floats [i] += GradeChain (i);
			dps_floats [i] += GradeBlue (i);

			if (dps_floats [i] >= .9f)
				grade = "A";
			else if (dps_floats [i] >= .8f)
				grade = "B";
			else if (dps_floats [i] >= .7f)
				grade = "C";
			else if (dps_floats [i] >= .51f)
				grade = "D";
			else if (dps_floats [i] < .51f)
				grade = "F";
		}

		if (mode == DPSCALC_MODE.ALL)
			output += base_total + "///" + dps_floats[i] + " - (" + grade + ")" + "</color> <";
		else
			output += dps_floats[i].ToString() + "</color> <";

		if (mode == DPSCALC_MODE.DPS)
			output += "((" + bulletScripts[i].Damage + "(<color=#ff0000ff>" + bulletScripts[i].Crit_Chance + "</color>*" + bulletScripts[i].Crit_Damage + ")))*<color=#00ffffff>" + Mathf.Round(1f/weaponScripts[i].GetCooldown) + "</color> >";
		return output;
	}

	#region grades
	float GradeSlow(int i)
	{
		Bullet_Slow bullet = bulletObjects [i].GetComponent<Bullet_Slow> ();
		if (bullet == null)
			return 0f;
		else
			return bullet.SlowAmount;//*(1f/weaponScripts[i].GetCooldown);
	}

	float GradeSpread(int i)
	{
		Spread_Weapon bullet = bulletObjects [i].GetComponent<Spread_Weapon> ();
		if (bullet == null)
			return 0f;
		else {
			float returnValue = 0f;
			Base_Bullet bulletSecondWeapon = bullet.SecondHitbox.GetComponent<Base_Bullet>();
			returnValue = bulletSecondWeapon.Damage * (1f + (bulletSecondWeapon.Crit_Chance / 100f * (bulletSecondWeapon.Crit_Damage - 1f))) * (1f / weaponScripts [i].GetCooldown);
			returnValue += bulletSecondWeapon.Damage * (1f + (bulletSecondWeapon.Crit_Chance / 100f * (bulletSecondWeapon.Crit_Damage - 1f))) * Mathf.Round ((float)weaponScripts [i].MaxAmmo);
			returnValue /= 2000f;
				
			returnValue *= bullet.ExtraBulletCount;
			returnValue *= 270f/bullet.Spread;

			return returnValue;
		}
	}

	float GradeInfinite(int i)
	{
		if (weaponScripts [i].HasUnlimitedAmmo)
			return .5f;
		else
			return 0f;
	}

	float GradeBlock(int i)
	{
		Bullet_Wall bullet = bulletObjects [i].GetComponent<Bullet_Wall> ();
		if (bullet == null)
			return 0f;
		else
			return .4f;
	}

	float GradeReflect(int i)
	{
		Katana_Bullet bullet = bulletObjects [i].GetComponent<Katana_Bullet> ();
		if (bullet != null)
			return .20f;
		else
			return 0f;
	}

	float GradeTranscendant(int i)
	{
		Katana_Bullet bullet = bulletObjects [i].GetComponent<Katana_Bullet> ();
		Pirahna_Bullet bullet1 = bulletObjects [i].GetComponent<Pirahna_Bullet> ();
		if (bullet != null)
			return .20f;
		else if (bullet1 != null)
			return .4f;
		else
			return 0f;
	}

	float GradeHeal(int i)
	{
		Bullet_Leech bullet = bulletObjects [i].GetComponent<Bullet_Leech> ();
		if (bullet != null)
			return bullet.HealAmount / 50f;
		else
			return 0f;
	}

	float GradeStun(int i)
	{
		if (bulletScripts[i].CanStun)
			return .5f;
		else
			return 0f;
	}

	float GradeChain(int i)
	{
		Bullet_Chain bullet = bulletObjects [i].GetComponent<Bullet_Chain> ();
		if (bullet != null) {
			float returnValue = 0f;
			Base_Bullet bulletSecondWeapon = bullet.NextBullet.GetComponent<Base_Bullet>();
			returnValue = bulletSecondWeapon.Damage * (1f + (bulletSecondWeapon.Crit_Chance / 100f * (bulletSecondWeapon.Crit_Damage - 1f))) * (1f / weaponScripts [i].GetCooldown);
			returnValue += bulletSecondWeapon.Damage * (1f + (bulletSecondWeapon.Crit_Chance / 100f * (bulletSecondWeapon.Crit_Damage - 1f))) * Mathf.Round ((float)weaponScripts [i].MaxAmmo);
			returnValue /= 2000f;

			returnValue *= bullet.NumberOfBullets;

			return returnValue;
		}
		else
			return 0f;
	}

	float GradeBlue(int i)
	{
		Refresh_Bullet bullet = bulletObjects [i].GetComponent<Refresh_Bullet> ();
		if (bullet != null)
			return .75f;
		else
			return 0f;
	}
	#endregion

	string CalculateAllDPS()
	{
		string output = "";
		for (int i=0; i<names.Length; i++)
		{
			dps_strings [i] = "";
			dps_strings [i]  += names [i];
			dps_strings [i]  += ": " + CalculateDPS (i);
			dps_strings [i]  += "\n";
			output += dps_strings [i];
		}
		return output;
	}

	void ConstructStringsFromSort()
	{
		dpsText.text = "";
		for (int i = 0; i < names.Length; i++) {
			dpsText.text += dps_strings [i];
		}
	}

	void SortStrings()
	{
		for (int i = 1; i < dps_floats.Length; i++) {
			for (int j = i; j > 0; j--) {
				if (dps_floats [j] > dps_floats [j - 1]) {
					SwapDpsStrings (j, j - 1);
				} else
					break;
			}
		}
		ConstructStringsFromSort ();
	}

	void SwapDpsStrings(int i1, int i2)
	{
		float floatTemp = dps_floats [i1];
		string stringTemp = dps_strings [i1];

		dps_floats [i1] = dps_floats [i2];
		dps_strings [i1] = dps_strings [i2];

		dps_floats [i2] = floatTemp;
		dps_strings [i2] = stringTemp;
	}
}

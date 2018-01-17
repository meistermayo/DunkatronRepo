using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
	[SerializeField] Text healthText;
	[SerializeField] Slider healthSlider;
	[SerializeField] Slider armorSlider;
	[SerializeField] Slider healthSlider_FX;
	[SerializeField] Text stunText;
	[SerializeField] Text ammoText;
	[SerializeField] float health_FX_rate;

	void Update()
	{
		healthText.transform.localScale = new Vector3 (.01f * transform.localScale.x, .01f, .01f);
		UpdateHealthFX ();
	}

	public void UpdateHealthUI(float healthValue, float armorValue)
	{
		healthText.text = Mathf.Round(healthValue).ToString ();
		healthSlider.value = healthValue;
		armorSlider.maxValue = healthSlider.maxValue;
		armorSlider.value = armorValue * healthValue;
	}

	public void UpdateStunUI(int stun)
	{
		if (stun < 5) {
			stunText.text = "";
			for (int i = 0; i < stun; i++) {
				stunText.text += "*";
			}
		} else {
			stunText.text = "STUN";
		}
	}

	public void ResetHealthUI(float value)
	{
		healthText.text = Mathf.Round(value).ToString ();
		healthSlider.maxValue = value;
		healthSlider.value = value;
		healthSlider_FX.maxValue = value;
		healthSlider_FX.value = value;
	}

	void UpdateHealthFX()
	{
		if (healthSlider_FX.value > healthSlider.value) {
			healthSlider_FX.value -= health_FX_rate;
			if (healthSlider_FX.value < healthSlider.value)
				healthSlider_FX.value = healthSlider_FX.value;
		}
	}
	public void UpdateAmmoCounter(int ammo, int maxAmmo)
	{
		if (maxAmmo == 0)
			return;
		if (ammoText.enabled)
			ammoText.text = ammo.ToString ();	
		ammoText.color = Color.Lerp (Color.red, Color.white, (float)ammo/maxAmmo);
		ammoText.fontSize = 30 - (15 * ammo / maxAmmo);
	}

	public void ShowAmmo(bool show)
	{
		ammoText.enabled = show;
	}

}


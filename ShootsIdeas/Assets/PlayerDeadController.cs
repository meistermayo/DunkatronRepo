using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerDeadOptionType
{
	MOB = 0,
	POWERUP
}

[System.Serializable] 
public class PlayerDeadSpawnOption{
	[SerializeField] public string name;
	[SerializeField] public Sprite displaySprite;
	[SerializeField] public float cost;
	[SerializeField] public GameObject prefab;
	[SerializeField] public PlayerDeadOptionType type;
	[SerializeField] public float baseValue;
}

public class PlayerDeadController : PlayerScript {
	[SerializeField] float moveSpeed;
	[SerializeField] float chargeRate;
	[SerializeField] PlayerDeadSpawnOption[] options;
	[SerializeField] Text costText;

	int currentOption = 0;
	float h1,v1,h2,v2,lh;
	float bumper, lastBumper;
	bool bumperPressed;
	bool hDown;
	bool aPressed;

	float power;
	SpriteRenderer spriteRenderer;

	void Start()
	{
		playerTag = GetComponent<PlayerTag> ();
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		ChangeOption ();
	}

	void Update()
	{
		Charge ();

		GetInput ();
		Move ();
		ChangeOption ();
		SelectOption ();
		UpdateText ();
	}

	void UpdateText()
	{
		costText.text = options [currentOption].cost.ToString ();
		costText.text += "\n\n\n";
		costText.text += Mathf.Round (power).ToString ();
	}

	void Charge()
	{
		power += chargeRate * Time.deltaTime;
	}

	void Move ()
	{
		transform.position += new Vector3 (h1, v1, 0f) * moveSpeed;
	}

	void GetInput()
	{
		h1 = Input.GetAxis ("h1_" + playerTag.Id.ToString ());
		v1 = Input.GetAxis ("v1_" + playerTag.Id.ToString ());
		lh = h2;
		h2 = Mathf.Round(Input.GetAxis ("h2_" + playerTag.Id.ToString ()));
		hDown = (lh == 0f && h2 != 0f);
		v2 = Input.GetAxis ("v2_" + playerTag.Id.ToString ());

		lastBumper = bumper;
		bumper = Mathf.Round(Input.GetAxis ("switch_" + playerTag.Id.ToString ()));
		bumperPressed = (lastBumper == 0f && bumper != 0f);

		aPressed = Input.GetButtonDown ("aButton_" + playerTag.Id.ToString ());
	}

	void ChangeOption()
	{
		int changeDir = 0;
		if (hDown)
			changeDir = (int)h2;
		if (bumperPressed)
			changeDir = (int)bumper;
		currentOption += changeDir;
		if (currentOption >= options.Length)
			currentOption = 0;
		if (currentOption < 0)
			currentOption = options.Length - 1;
		spriteRenderer.sprite = options [currentOption].displaySprite;
	}

	void SelectOption()
	{
		if (aPressed)
		{
			if (power > options [currentOption].cost) {
				power -= options [currentOption].cost;
				InstantiateObject ();
			}
		}
	}

	public void InstantiateObject()
	{
		GameObject objectToInstantiate = Instantiate (options [currentOption].prefab, transform.position, Quaternion.identity);

		if (options [currentOption].type == PlayerDeadOptionType.MOB) {
			Base_Mob mob = objectToInstantiate.GetComponent<Base_Mob> ();
			mob.Ini (null, playerTag.Id, playerTag.Team, true);
		} else if (options [currentOption].type == PlayerDeadOptionType.POWERUP) {
			PickupRespawn pickup = objectToInstantiate.GetComponent<PickupRespawn> ();
			pickup.duration = -1f;

			PickupGun gun = objectToInstantiate.GetComponent<PickupGun> ();
			if (gun != null) {
				return;
			}

			Powerup powerup = objectToInstantiate.GetComponent<Powerup> ();
			if (powerup != null) {
				powerup.SetValue (options[currentOption].baseValue);
				return;
			}

			AmmoPickup ammo = objectToInstantiate.GetComponent<AmmoPickup> ();
			if (ammo != null) {
				ammo.SetValue((int)options [currentOption].baseValue);
				return;
			}

		}
	}
}

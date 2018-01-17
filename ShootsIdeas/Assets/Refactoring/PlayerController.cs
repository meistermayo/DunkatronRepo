using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Base_Player_Movement))]
[RequireComponent(typeof(Base_WeaponManager))]
[RequireComponent(typeof(Player_Health))]
[RequireComponent(typeof(PlayerAnimator))]
[RequireComponent(typeof(PlayerTag))]
[RequireComponent(typeof(PlayerEnemyHandler))]
public class PlayerController : PlayerScript
{
	[SerializeField] AudioClip switchNoise;
	//Input
	bool canSwitch=true;
	bool input_active = true;
	float h1,v1,h2,v2;
	float bumperAxis;
	bool bumperDown;
	bool disbandPressed;

	Base_Player_Movement movementScript;
	Base_WeaponManager weaponScript;
	Player_Health healthScript;
	PlayerAnimator playerAnimator;
	PlayerEnemyHandler playerEnemyHandler;

	bool isJuggernaut;
	public bool IsJuggernaut{ get { return isJuggernaut; } }

	int nameIndex;
	public int NameIndex{ get { return nameIndex; } }

	void Awake()
	{
		playerTag = GetComponent<PlayerTag> ();
		movementScript = GetComponent<Base_Player_Movement> ();
		weaponScript = GetComponent<Base_WeaponManager> ();
		healthScript = GetComponent<Player_Health> ();
		playerAnimator = GetComponent<PlayerAnimator> ();
		playerEnemyHandler = GetComponent<PlayerEnemyHandler> ();
	}
		
	public void RefreshCanSwitch()
	{
		canSwitch = true;
	}

	void Update()
	{
		if (!input_active) // no input, no actions
			return;
		GetInput ();

		if (healthScript.Stun) { // input, no actions
			GetComponent<Rigidbody2D>().velocity=Vector2.zero;
			return;
		}
		Move ();
		Shoot ();
		SwitchWeapon ();
	}

	void GetInput()
	{
		h1 = Input.GetAxisRaw ("h1_" + playerTag.Id);
		v1 = Input.GetAxisRaw ("v1_" + playerTag.Id);
		h2 = Mathf.Round(Input.GetAxisRaw ("h2_" + playerTag.Id));
		v2 = Mathf.Round(Input.GetAxisRaw ("v2_" + playerTag.Id));
		float lastBumper = bumperAxis;
		bumperAxis = Input.GetAxisRaw ("switch_" + playerTag.Id);
		bumperDown = (bumperAxis != 0f && lastBumper == 0f);
	}

	void Move()
	{
		movementScript.Move (h1, v1);
		if (h1 != 0f || v1 != 0f) {
			playerAnimator.Animate (h1, v1);
		}
	}

	void Shoot()
	{
		if (h2 != 0f || v2 != 0f)
			weaponScript.Attack (h2,v2);
	}

	void SwitchWeapon()
	{
		if (canSwitch)
		if (bumperDown) {
			GlobalAudioManager.Instance.PlaySound (switchNoise);
			weaponScript.RotateWeapons ();
			StartCoroutine (SwitchWeaponTimer ());
		}
	}

	public void SetInputActive(bool active)
	{
		input_active = active;
	}

	public void BecomeJuggernaut()
	{
		//TODO
	}

	public void IniPlayer(int id,int team,string avatar, GameObject weapon)
	{
		GetPlayerTag ().SetId(id);
		playerTag.SetTeam( team);

		if (avatar != null && avatar != "")
		GetComponentInChildren<Animator> ().runtimeAnimatorController = Resources.Load(avatar) as RuntimeAnimatorController;

		GameObject temp = Instantiate (weapon, transform) as GameObject;
		temp.GetComponent<PlayerTag>().SetId (GetPlayerTag().Id);

		weaponScript.secondaryWeapon = temp.GetComponent<Base_Weapon>();
	}


	public void IniPlayer(string avatar, GameObject weapon)
	{
		if (avatar != null && avatar != "")
			GetComponentInChildren<Animator> ().runtimeAnimatorController = Resources.Load(avatar) as RuntimeAnimatorController;

		GameObject temp = Instantiate (weapon, transform) as GameObject;
		temp.GetComponent<PlayerTag>().SetId (GetPlayerTag().Id);

		weaponScript.secondaryWeapon = temp.GetComponent<Base_Weapon>();
	}

	IEnumerator SwitchWeaponTimer()
	{
		canSwitch = false;
		yield return new WaitForSeconds (1f);
		canSwitch = true;
	}
}
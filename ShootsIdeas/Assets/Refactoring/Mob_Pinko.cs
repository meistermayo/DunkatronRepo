using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Base_Weapon))]
public class Mob_Pinko : Base_Mob
{
	[SerializeField] protected GameObject brainMobPrefab;
	protected Base_Weapon brainWeapon;
	protected GameObject targetObject;

	protected override void Start ()
	{
		base.Start ();
		brainWeapon = GetComponent<Base_Weapon> ();
	}
	public override void ReleasedBehavior ()
	{
		body.velocity = Vector2.zero;
		if (targetObject == null) {
			targetObject = targetter.GetNearest ();
			if (targetObject == null)
				return;
		}
		{
			RaycastHit2D hit = Physics2D.Raycast (transform.position, targetObject.transform.position - transform.position, 100f);
			if (hit != null) {
				if (hit.collider.tag == "Wall") {
					targetObject = null;
				}
			}
		}
		if (targetObject != null) {
				Vector3 shootVec = Vector3.Normalize(targetObject.transform.position - transform.position);
				brainWeapon.Attack (shootVec.x,-shootVec.y);
		}
	}

	void OnTriggerEnter2D (Collider2D other)
	{
		PickupRespawn pickup = other.GetComponent<PickupRespawn> ();
		if (pickup != null && pickup.active) {
			GameObject temp = Instantiate (brainMobPrefab, transform.position, Quaternion.identity) as GameObject;
			Mob_Skeel skeel = temp.GetComponent<Mob_Skeel> ();
			skeel.Ini (null, playerTag.Id, playerTag.Team, true);
			pickup.StartRespawnTimer ();
		}
	}
	/*
	protected override void Chase ()
	{
		if (targetObject.GetComponent<PickupRespawn> () != null) {
			if (!targetObject.GetComponent<PickupRespawn> ().active) {
				state = STATE.FOLLOW;
				return;
			}
		}
		base.Chase ();
	}

	protected override void OnTriggerStay2D (Collider2D other)
	{
		if (other.tag == "EnemyPickup" || other.tag == "Powerup") {
			PickupRespawn otherPickup = other.GetComponent<PickupRespawn> (); // Return statement
			if (!otherPickup.active)
				return;
			otherPickup.StartRespawnTimer ();

			OLD_BaseMob mob_script;

			{
				GameObject mob = Instantiate (brainMobPrefab, transform.position, Quaternion.identity);  // Get mob
				mob_script = mob.GetComponent<OLD_BaseMob> ();
			}

			mob_script.SetHealth (50f, 50f); // set mob stuff
			mob_script.SetLoose ();
			mob_script.GetComponent<PlayerTag> ().SetTeam (playerTag.Team);
			mob_script.GetComponent<PlayerTag> ().SetId(playerTag.Id);
			mob_script.SetColor (playerTag.Id);
			mob_script.IgnoreCollision ();
		}
	}
	*/
}


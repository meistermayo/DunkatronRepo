using UnityEngine;
using System.Collections;

public class Mob_Skeel : Base_Mob
{
	[SerializeField] protected GameObject skeelPrefab;
	[SerializeField] protected int skeelAmount;
	GameObject chaseObject;
	/*
	protected override void Start ()
	{
		base.Start ();
	}
*/

	public override void Release ()
	{
		if (parentObject != null) {
			parentObject.transform.position = transform.position;
		}
			base.Release ();
	}
	/*
	protected override void GetInput()
	{
		h = Input.GetAxisRaw ("h2_" + playerTag.Id);
		v = Input.GetAxisRaw ("v2_" + playerTag.Id);
	}
*/
	public override void ReleasedBehavior ()
	{
		if (chaseObject == null) {
			chaseObject = targetter.GetNearest ();
			if (chaseObject == null) {
				body.velocity = Vector2.zero;
				return;
			}
		}

		body.velocity = Vector3.Normalize (chaseObject.transform.position - transform.position) * 
			movementScript.Move_Speed;
	}

	void OnTriggerStay2D(Collider2D other)
	{
		if (!canAttack)
			return;
		Base_Health health = other.GetComponent<Base_Health> ();
		if (health != null) {
			if (health.GetPlayerTag ().Id != playerTag.Id && (health.GetPlayerTag ().Team == 0 || health.GetPlayerTag ().Team != playerTag.Team)) {
				if (health.Health - damage <= 0f) {
					if (other.GetComponent<Player_Health> () != null) {
						for (int i = 0; i < skeelAmount; i++) {
							GameObject skeel = Instantiate (skeelPrefab, other.transform.position, Quaternion.identity);
							skeel.GetComponent<Mob_Skeel> ().Ini (null,playerTag.Id,playerTag.Team,true);
						}
					}
				}
				health.TakeDamage (damage, playerTag.Id, playerTag.Team);
				StartCoroutine (AttackCooldown ());
			}
		}
	}

}


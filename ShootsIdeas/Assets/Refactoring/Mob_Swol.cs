using UnityEngine;
using System.Collections;

public class Mob_Swol : Base_Mob
{
	public override void ReleasedBehavior ()
	{
		body.velocity = Vector2.zero;
	}

	[SerializeField] protected GameObject splashDamagePrefab;
	[SerializeField] float bombDuration;
	public override void Release ()
	{
		base.Release ();
		GetComponent<Mob_Health> ().SetInvincibilty(true);
		Destroy (gameObject, healthScript.Health / healthScript.Health_Max * bombDuration);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (released) {
			Base_Health health = other.GetComponent<Base_Health> ();
			if (health!= null) {
				if (health.GetPlayerTag().Id != playerTag.Id || (health.GetPlayerTag().Team != 0 && health.GetPlayerTag().Team != playerTag.Team))
				{
					GameObject temp = Instantiate (splashDamagePrefab, transform.position, Quaternion.identity) as GameObject;
					temp.GetComponent<Base_Bullet> ().SetInfo (playerTag.Id, playerTag.Team);
					Destroy (gameObject);
				}
			}
		} else {
			Base_Bullet bullet = other.GetComponent<Base_Bullet> ();
			if (other.GetComponent<Bullet_Explosion> () != null)
				return;
			if (other.GetComponent<Katana_Bullet> () != null)
				return;
			if (bullet != null) {
				if (bullet.Id!= playerTag.Id && (bullet.Team == 0 || bullet.Team != playerTag.Team)) {
					Destroy (other.gameObject);
				}
			} else if (other.GetComponent<Base_Mob> () != null) {
				if (!canAttack)
					return;
				Base_Health health = other.GetComponent<Base_Health> ();
				if (health != null) {
					if (health.GetPlayerTag ().Id != playerTag.Id && (health.GetPlayerTag ().Team == 0 || health.GetPlayerTag ().Team != playerTag.Team)) {
						StartCoroutine (AttackCooldown ());
						health.TakeDamage (damage, playerTag.Id, playerTag.Team);
					}
				}
			}
		}
	}

}


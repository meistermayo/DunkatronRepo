using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldEnemyTargetter : MonoBehaviour {
	EnemyScript parentEnemy;

	// Use this for initialization
	void Awake()
	{
		parentEnemy = GetComponentInParent<EnemyScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		int team = parentEnemy.team;

		if (parentEnemy.type == 0) { // SKEEL
			MovementScript otherMov = other.GetComponent<MovementScript> ();
			if (otherMov == null) // if doesnt have a mov script, dont use a null one
			return;
			if (team != 0 && otherMov.team == team)
				return;
			if (otherMov.player_num != parentEnemy.id) { // if not our player, follow.
				parentEnemy.state = STATE.CHASE;
				parentEnemy.targetObject = other.gameObject;
			}

		} else if (parentEnemy.type == 1) { // SWOL
			EnemyScript otherEne = other.GetComponent<EnemyScript> (); // CHASESE NEMY
			if (otherEne != null) {
				if (team != 0 && otherEne.team == team)
					return;
				if (otherEne.id != parentEnemy.id) {
					parentEnemy.state = STATE.CHASE;
					parentEnemy.targetObject = other.gameObject;
				}
			} else {
				BulletScript otherBul = other.GetComponent<BulletScript> (); // CHASES BULLET
				if (otherBul == null)
					return;
				if (team != 0 && otherBul.team == team)
					return;
				if (otherBul.id != parentEnemy.id) {
					parentEnemy.state = STATE.CHASE;
					parentEnemy.targetObject = other.gameObject;
				}
			}
			
		} else if (parentEnemy.type == 2) {	//PINKO PANKO
			PickupRespawn otherPickup = other.GetComponent<PickupRespawn>();

			if (otherPickup == null)
				return;
			
			if (!otherPickup.active)
				return;

			parentEnemy.state = STATE.CHASE;
			parentEnemy.targetObject = other.gameObject;


		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		int team = parentEnemy.team;
		if (other.gameObject == parentEnemy.targetObject)
		{
			parentEnemy.state = STATE.FOLLOW;
			parentEnemy.targetObject = null;
			return;
		}
		MovementScript otherMov = other.GetComponent<MovementScript> ();
		if (otherMov == null) // if doesnt have a mov script, dont use a null one
			return;

		if (team != 0 && otherMov.team == team)
			return;
		if (otherMov.player_num != parentEnemy.id) { // if not our player, follow.
			parentEnemy.state = STATE.FOLLOW;
			parentEnemy.targetObject = other.gameObject;
		}
	}

}

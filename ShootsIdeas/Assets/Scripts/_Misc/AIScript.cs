using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour {

	MovementScript movementScript;
	WeaponManager weaponManager;
	PlayerEnemyHandler playerEnemyHandler;
	Rigidbody2D mBody;
	int id;

	GameObject targetPickup;
	GameObject targetPlayer;

	// Use this for initialization
	void Start () {
		movementScript = GetComponent<MovementScript> ();
		weaponManager = GetComponent<WeaponManager> ();
		playerEnemyHandler = GetComponent<PlayerEnemyHandler> ();
		mBody = GetComponent<Rigidbody2D> ();
		id = movementScript.player_num;
	}
	
	// Update is called once per frame
	void Update () {
		if (playerEnemyHandler.enemy_count < 3) 
		{
			PickupEnemies ();
		} 
		else 
		{
			ChasePlayer();
		}

		// TargetPlayer
		if (targetPlayer == null) {
			GameObject[] temp = GameObject.FindGameObjectsWithTag ("Player");
			for (int i = 0; i < temp.Length; i++) {
				if (temp [i].GetComponent<MovementScript> ().player_num != id) {
					targetPlayer = temp [i];
					break;
				}
			}
		} else {
			float angle = Mathf.Atan2 (transform.position.y - targetPlayer.transform.position.y, -(transform.position.x - targetPlayer.transform.position.x)) * Mathf.Rad2Deg;
			float r = Random.Range (-10f, 10f);
			angle += r;
			if (Mathf.Abs (r) < 2.5f) {
				weaponManager.weapons [0].h = Mathf.Cos (angle * Mathf.Deg2Rad);
				weaponManager.weapons [0].v = Mathf.Sin (angle * Mathf.Deg2Rad);
				weaponManager.weapons [0].Attack ();
			}
		}

	}

	void PickupEnemies()
	{

		if (targetPickup == null) {
			GameObject[] temp = GameObject.FindGameObjectsWithTag ("EnemyPickup");
			targetPickup = temp [0];
			if (targetPickup == null)
				return;
			float minDist = Vector3.Distance (transform.position, temp [0].transform.position);
			for (int i = 0; i < temp.Length; i++) {
				float thisDist = Vector3.Distance (transform.position, temp [i].transform.position);
				if (thisDist < minDist) {
					targetPickup = temp [i];
					minDist = thisDist;
				}
			}
		} else {
			mBody.velocity = Vector2.ClampMagnitude(new Vector2(-transform.position.x + targetPickup.transform.position.x, -transform.position.y + targetPickup.transform.position.y)*100f,movementScript.move_speed);
		}

	}


	void ChasePlayer()
	{
		if (targetPlayer != null) {
			mBody.velocity = Vector2.ClampMagnitude(new Vector2(-transform.position.x + targetPlayer.transform.position.x, -transform.position.y + targetPlayer.transform.position.y)*100f,movementScript.move_speed);
		}
	}

}

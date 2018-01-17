using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : MonoBehaviour {
	EnemyScript enemyScript;
//	Coroutine shootRoutine;
	public float cooldownFrames;
	public GameObject bulletPrefab;
	bool can_shoot=true;

	void Awake()
	{
		enemyScript = GetComponent<EnemyScript> ();
	}

	void Update()
	{
		if (can_shoot) {
			if (enemyScript.state == STATE.CHASE) {
				StartCoroutine (ShootRoutine(cooldownFrames));
			}
		}
	}

	IEnumerator ShootRoutine(float frames)
	{
		can_shoot = false;
		if (enemyScript.targetObject == null)
			yield break;

		GameObject bullet = Instantiate ( bulletPrefab, transform.position, 
			Quaternion.Euler(new Vector3(0f,0f,Mathf.Rad2Deg*Mathf.Atan2(
				-(transform.position.y - enemyScript.targetObject.transform.position.y), 
				-(transform.position.x - enemyScript.targetObject.transform.position.x))))
			) as GameObject;
		
		BulletScript bulletScript = bullet.GetComponent<BulletScript> ();

		bulletScript.id = enemyScript.id;
		bulletScript.team = enemyScript.team;
		//bulletScript.damage = Mathf.Ceil(bulletScript.damage*damageMult);
		while (frames > 0) {
			yield return new WaitForEndOfFrame ();
			frames--;
		}

		can_shoot = true;
	}
}

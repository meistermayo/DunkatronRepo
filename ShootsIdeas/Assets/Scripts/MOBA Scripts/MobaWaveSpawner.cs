using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobaWaveSpawner : MonoBehaviour {

	int waveNum = 1;
	public GameObject MeleeMinionPrefab, RangedMinionPrefab, TankMinionPrefab;
	public GameObject waveGoal;
	public int team;
	void Start()
	{
		StartCoroutine (SpawnWave ());
	}

	IEnumerator SpawnWave()
	{
		yield return new WaitForSeconds (3f);
		while (true) {
			for (int i = 0; i < 3; i++) {
				SpawnMinion (MeleeMinionPrefab);
			}
			for (int i = 0; i < 2; i++) {
				SpawnMinion (RangedMinionPrefab);
			}
			if (waveNum % 2 == 0)
				SpawnMinion (TankMinionPrefab);
			waveNum++;
			yield return new WaitForSeconds (30f);
		}
	}

	void SpawnMinion(GameObject minionPrefab)
	{

		GameObject minion = Instantiate (minionPrefab, transform.position, Quaternion.identity) as GameObject;
		EnemyScript enemyScript = minion.GetComponent<EnemyScript> ();
		enemyScript.team = team;
		if (team == 1)
		enemyScript.outlineSprite.color = Color.blue;
		enemyScript.targetObject = waveGoal;
	}
}

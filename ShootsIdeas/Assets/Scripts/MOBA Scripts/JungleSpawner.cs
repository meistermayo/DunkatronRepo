using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleSpawner : MonoBehaviour {

	public GameObject[] prefabMinions;
	public Vector3[] spawnPos;
	public float initialTimer;
	public float respawnTimer;

	List<GameObject> activeMinions;

	// Use this for initialization
	void Start () {
		activeMinions = new List<GameObject> ();
		StartCoroutine (SpawnMinions ());
	}

	void Update()
	{
		for (int i = 0; i < activeMinions.Count; i++) {
			if (activeMinions [i] == null)
				activeMinions.RemoveAt (i);
		}

	}

	IEnumerator SpawnMinions()
	{
		yield return new WaitForSeconds (initialTimer);
		while (true) {
			if (activeMinions.Count == 0) {
				yield return new WaitForSeconds (respawnTimer);
				for (int i = 0; i < prefabMinions.Length; i++) {
					GameObject minion = Instantiate (prefabMinions [i], spawnPos [i], Quaternion.identity) as GameObject;
					activeMinions.Add (minion);
				}
			} else
				yield return new WaitForSeconds (.016f*3f);
		}
	}
}

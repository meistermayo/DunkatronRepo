using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider2D))]
public class Targetter : MonoBehaviour
{
	protected List<GameObject> targets;

	void Awake()
	{
		targets = new List<GameObject> ();
	}


	public GameObject GetNearest()
	{
		float minDist=-1f;
		float dist;
		GameObject returnObject=null;

		for (int i=0; i<targets.Count; i++){
			if (targets [i] == null)
				continue;
			dist = Vector3.SqrMagnitude (targets[i].transform.position - transform.position);
			if (minDist < 0f) {
				minDist = dist;
				returnObject = targets[i];
			}
			else if (dist < minDist) {
				minDist = dist;
				returnObject = targets[i];
			}
		}

		return returnObject;
	}

	public virtual void AddTarget(GameObject _gameObject)
	{
		if (!targets.Contains(_gameObject))
			targets.Add (_gameObject);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		AddTarget (other.gameObject);
	}


}


using UnityEngine;
using System.Collections;

public class EnemyTargetter : Targetter
{
	PlayerTag playerTag;

	void Start()
	{
		playerTag = GetComponentInParent<PlayerTag> ();
	}

	public override void AddTarget (GameObject _gameObject)
	{
		Base_Health otherHealth = _gameObject.GetComponent<Base_Health> ();
		if (otherHealth != null) {
			if (otherHealth.GetPlayerTag ().Id != playerTag.Id && (otherHealth.GetPlayerTag ().Team == 0 || otherHealth.GetPlayerTag ().Team != playerTag.Team))
				if (!targets.Contains(_gameObject))
					targets.Add (_gameObject);
		}

	}

}


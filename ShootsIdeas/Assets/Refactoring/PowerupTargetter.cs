using UnityEngine;
using System.Collections;

public class PowerupTargetter : Targetter
{

	public override void AddTarget (GameObject _gameObject)
	{
		if (_gameObject.GetComponent<PickupRespawn>() != null)
			targets.Add (_gameObject);
	}

}


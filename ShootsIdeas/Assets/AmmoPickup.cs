using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour {
	[SerializeField] int value;
	public int Value{get {return value;}}

	public void SetValue(int value)
	{
		this.value = value;
	}
}

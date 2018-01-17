using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickup : MonoBehaviour {
	[Range(0f,1f)][SerializeField] float value;
	public float Value { get { return value; } }
}

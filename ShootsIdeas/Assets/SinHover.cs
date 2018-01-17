using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinHover : MonoBehaviour {
	Vector3 originPos;
	[SerializeField] float rate;
	[SerializeField] float depth;
	float angle;

	void Start()
	{
		originPos = transform.position;
	}

	void FixedUpdate()
	{
		transform.position = originPos + Vector3.up * Mathf.Sin (angle * rate) * depth;
		angle++;
	}
}

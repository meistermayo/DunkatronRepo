using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleOverTime : MonoBehaviour {
	[SerializeField] float rate;
	[SerializeField] float max;
	bool increase = true;
	// Update is called once per frame
	void FixedUpdate() {
		if (increase)
			transform.localScale *= rate;
		if (transform.localScale.magnitude > rate) {
			transform.localScale = transform.localScale.normalized * rate;
			increase = false;
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberController : MonoBehaviour {
	Text myText;
	float speed, duration=1f;
	float gravity;
	float hspd;

	// Use this for initialization
	void Start () {
		myText = GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += speed * Vector3.up + Vector3.right * hspd;
		transform.localScale -= Vector3.one * (1f/(duration)) * 2f;
		if (transform.localScale.x <= 0f)
			Destroy (gameObject);
		duration--;
		if (duration <= 0f)
			Destroy (gameObject);
		speed -= gravity;
	}

	// Sets up color
	public void IniColor(string value,  Color color, float speed, float duration, float scale)
	{
		if (myText == null)
			myText = GetComponentInChildren<Text> ();
		myText.text = value;
		myText.color = color;
		hspd = speed * .1f;
		speed += Random.Range (-hspd, hspd);
		hspd = Random.Range (-hspd,hspd);
		this.speed = speed;
		this.duration = duration;
		gravity = speed * .02f;
		transform.localScale *= scale;
	}
}

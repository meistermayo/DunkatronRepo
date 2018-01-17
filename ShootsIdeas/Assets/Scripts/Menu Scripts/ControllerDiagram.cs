using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerDiagram : MonoBehaviour {

	public int id;
	float h,v;
	public float stickDist;
	public GameObject lStick,rStick,aButton,bButton,yButton,xButton;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		lStick.transform.position = transform.position + new Vector3 ( Input.GetAxis("h1_"+id) , Input.GetAxis("v1_"+id) ) * stickDist;
		rStick.transform.position = transform.position + new Vector3 ( Input.GetAxis("h2_"+id) , -Input.GetAxis("v2_"+id) ) * stickDist;
		aButton.SetActive ( Input.GetButton("aButton_"+id) );
		bButton.SetActive ( Input.GetButton("bButton_"+id) );
		xButton.SetActive ( Input.GetButton("xButton_"+id) );
		yButton.SetActive ( Input.GetButton("yButton_"+id) );
	}
}

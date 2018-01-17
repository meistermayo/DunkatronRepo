using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject[] players;
	public float moveSpeed;
	Vector3 targPos;
	float smoothSpeed;
	bool runShake;
	float sinShake;
	[SerializeField] float depth, depthDecay, rate;
	float _depth;

	void Awake()
	{
		//players = GameObject.FindGameObjectsWithTag ("Player");
	}

	void Update()
	{
		if (players.Length == 0) {
			transform.position = Vector3.Lerp (transform.position, Vector3.forward * -10f, moveSpeed);
			Camera.main.orthographicSize = Mathf.Lerp (Camera.main.orthographicSize, 5f, moveSpeed);
			return;
		}
		
		int player_count= 0;
		float x=0f, y=0f;

		for (int i = 0; i < players.Length; i++) {
			if (players [i] != null && players [i].activeSelf) {
				player_count++;
				x += players [i].transform.position.x;
				y += players [i].transform.position.y;
			}
		}

		if (player_count == 0) {
			targPos = Vector2.zero;
		} else {
			
			x = x / player_count;
			y = y / player_count;

			targPos = new Vector3 (x, y, transform.position.z);
			transform.position = Vector3.Lerp (transform.position, targPos, moveSpeed);
			Zoom ();
		}
		RunShake ();
	}

	private void Zoom() /// RIPPED
	{
		float requiredSize = FindRequiredSize(); //same thing as MOve but with zoom
		Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, requiredSize, ref smoothSpeed, .1f); //size = smoothdamp of ( size, targsize, zoomspeed, smoothing.
	}

	 
	private float FindRequiredSize() //Returns a float! /// RIPPED
	{
		//\/???
		Vector3 desiredLocalPos = transform.InverseTransformPoint(targPos);

		float size = 0f;
		int playerCount=0;

		for (int i = 0; i < players.Length; i++)
		{
			if (players [i] == null)
				continue;
			if (!players[i].gameObject.activeSelf)
				continue;
			playerCount++;
			//											\/????
			Vector3 targetLocalPos = transform.InverseTransformPoint(players[i].transform.position);

			Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

			size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / Camera.main.aspect);
		}

		if (playerCount == 0)
			size = 12f;
		else {
			size += 2f;

			size = Mathf.Max (size, .1f);
		}
		if (size < 5f)
			size = 5f;
		return size;
	}

	public void TurnOnShake()
	{
		runShake = true;
		_depth = depth;
	}

	void RunShake()
	{
		if (!runShake)
			return;
		
		sinShake++;
		_depth -= depthDecay;
		transform.position = transform.position + Vector3.up * Mathf.Sin(sinShake * rate) * _depth;
		if (_depth <= 0f)
			runShake = false;
	}
}

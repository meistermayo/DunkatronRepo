using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CapturePoint : MonoBehaviour {

	public float captureAmount=0f;
	public int captureId = -1;
	public int capturePId = -1;
	public float captureSpeed = .11f;
	public Slider captureBar;
	List<int> competitors;
	Collider2D[] overlapColliders;
	SpriteRenderer sr;
	public GameManager gameManager;

	public virtual void Start()
	{
		competitors = new List<int> ();
		sr = GetComponent<SpriteRenderer> ();
		captureBar = GetComponentInChildren<Slider> ();
		gameManager = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager>();
	}

	void FixedUpdate()
	{
		if (captureId == -1)
			return;
		if (competitors.Count == 0) {
			captureAmount -= captureSpeed*.70f;
			if (captureAmount < 0f) {
				SetColors (0);
				captureAmount = 0f;
			}
			captureBar.value = captureAmount;
			return;
		}

		bool uncontested = CheckForContested ();
		if (uncontested) {
			UpdateSlider ();
		}

	}

	bool CheckForContested()
	{
		int competitorCheck = competitors [0];
		for (int i = 0; i < competitors.Count; i++) {
			if (competitorCheck != competitors [i]) {
				return false;
			}
		}
		return true;
	}

	void UpdateSlider()
	{			
		if (competitors [0] == captureId) {
			captureAmount += captureSpeed;
		} else {
			captureAmount -= captureSpeed*1.07f;
		}
		if (captureAmount < 0f) {
			captureAmount = 0f;
			SetColors (competitors [0]);
		}
		if (captureAmount > 100f) {
			captureAmount = 100f;
			PointCaptured ();
		}
		captureBar.value = captureAmount;
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player") {
			MovementScript otherMov = other.GetComponent<MovementScript> ();
			capturePId = otherMov.player_num;
			if (!competitors.Contains (otherMov.team)) {
				competitors.Add (otherMov.team);
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "Player") {
			MovementScript otherMov = other.GetComponent<MovementScript> ();
			if (competitors.Contains (otherMov.team)) {
				competitors.Remove (otherMov.team);
			}
		}
	}

	public virtual void PointCaptured()
	{
		GameManager.cpScore [captureId]++;
		if (GameManager.cpScore [captureId] < 3) 
		{
			StartCoroutine (gameManager.SetNewCapturePoint ());
			GameObject[] temp = GameObject.FindGameObjectsWithTag ("CapturePoint");
			for (int i = 0; i < temp.Length; i++) {
				SecondaryCapture secCap = temp [i].GetComponent<SecondaryCapture> ();
				if (secCap != null) {
					secCap.captureId = 0;
					secCap.captureAmount = 0f;
					secCap.SetColors (0);
				}
			}
		}
		UnsetSelf ();
	}

	void UnsetSelf()
	{
		captureId = -1;
		captureAmount = 0f;
		SetColors (-1);
	}

	public void SetColors(int team)
	{
		ColorBlock colorBlock = new ColorBlock();
		captureId = team;
		if (team == -1) {
			sr.color = Color.black;
			colorBlock.normalColor = Color.black;
		}
		if (team == 0) {
			sr.color = Color.gray;
			colorBlock.normalColor = Color.gray;
		} else if (team == 1) {
			sr.color = Color.blue;
			colorBlock.normalColor = Color.blue;
		} else if (team == 2) {
			sr.color = Color.yellow;
			colorBlock.normalColor = Color.yellow;
		} else if (team == 3) {
			sr.color = Color.red;
			colorBlock.normalColor = Color.red;
		} else if (team == 4) {
			sr.color = Color.magenta;
			colorBlock.normalColor = Color.magenta;
		} else {
			sr.color = Color.white;
			colorBlock.normalColor = Color.white;
		}

		captureBar.colors = colorBlock;
	}

}

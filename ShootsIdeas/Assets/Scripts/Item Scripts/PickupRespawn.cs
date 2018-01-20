using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRespawn : MonoBehaviour {
	Collider2D mCollider;
	public bool active = true;
	public float duration;
	[SerializeField] float initialDuration;
	ParticleSystem partSys;
	[SerializeField] AudioClip audio;
	[SerializeField] GameObject aButton;

	void Awake()
	{
		partSys = GetComponent<ParticleSystem> ();
		if (!active) {
			if (initialDuration == 0f)
				initialDuration = duration;
			StartIniRespawnTimer ();
		}
		mCollider = GetComponent<Collider2D> ();
	}

	IEnumerator Respawn(float frames, bool sound)
	{
		SetAButton (false);
		if (GetComponentInChildren<Light> () != null) {
			GetComponentInChildren<Light> ().enabled = false;
		}
		if (sound)
			GlobalAudioManager.Instance.PlaySound (audio);
		if (partSys != null)
			partSys.Stop ();
		active = false;
		GetComponent<SpriteRenderer> ().enabled = false;
		while (frames > 0) {
			yield return new WaitForEndOfFrame ();
			frames--;
		}

		GetComponent<SpriteRenderer> ().enabled = true;
		mCollider.enabled = true;
		active = true;
		if (partSys != null)
			partSys.Play ();
		if (GetComponentInChildren<Light> () != null) {
			GetComponentInChildren<Light> ().enabled = true;
		}
	}

	public void StartRespawnTimer()
	{
		if (duration < 0f)
			Destroy (gameObject);
		else
			StartCoroutine (Respawn (60f * duration,true));
	}

	public void StartIniRespawnTimer()
	{
		if (duration < 0f)
			Destroy (gameObject);
		else
			StartCoroutine (Respawn (60f * initialDuration,false));
	}

	public void SetAButton(bool _on)
	{
		if (aButton != null)
			aButton.SetActive (_on);
	}
}

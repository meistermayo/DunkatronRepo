using UnityEngine;
using System.Collections;

public enum FX_TYPE{
	FIRE_BULLET,
	FIRE_BULLET_BUFFED
}

[System.Serializable]
public class FX_Object{
	[SerializeField] AudioClip clip;
	[SerializeField] ParticleSystem particle;

	public void Play()
	{
		GlobalAudioManager.Instance.PlaySound (clip);
		ParticleSystem particleBurrow = particle;
		int safety = 0;
		while (particleBurrow != null) { // Plays every child particle instance
			safety++;
			particleBurrow.Play ();
			if (particleBurrow.transform.childCount > 0)
				particleBurrow = particleBurrow.GetComponentInChildren<ParticleSystem> ();
			else
				break;
			if (safety > 100) {
				Debug.Log ("Broke Safety");
				break;
			}
		}
	}
}

public class FXPlayer : MonoBehaviour
{
	[SerializeField] FX_Object[] fx_objects;

	public void Play(int i)
	{
		if (i >= 0 && i < fx_objects.Length) {
			fx_objects [i].Play ();
		}
	}

	public void Play(FX_TYPE fx_type)
	{
		int f = (int)fx_type;
		if (f >= 0 && f < fx_objects.Length) {
			fx_objects [f].Play ();
		}
	}
}


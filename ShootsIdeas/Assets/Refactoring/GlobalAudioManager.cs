using UnityEngine;
using System.Collections;

public enum MUSIC {
	TITLE = 0,
	MENU,
	JUGGERNAUT,
	ROBOTRON,
	ROBOWAVE
}
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioSource))]
public class GlobalAudioManager : MonoBehaviour
{
	private static GlobalAudioManager instance;
	public static GlobalAudioManager Instance{
		get { return instance; }
		set { }
	}

	AudioSource audioSource_Sound;
	AudioSource audioSource_Music;
	AudioSource audioSource_Death;

	[SerializeField] AudioClip[] audioClips_Music;
	//821% 72%

	void Singleton()
	{
		if (instance == null)
			instance = this;
		else
			DestroyImmediate (this);
		DontDestroyOnLoad (gameObject);
	}

	public void Awake()
	{
		Singleton ();
		AudioSource[] audioSources = GetComponents<AudioSource> ();
		if (audioSources.Length < 3) {
			for (int i = 0; i < 3 - audioSources.Length; i++) {
				gameObject.AddComponent<AudioSource> ();
				audioSources = GetComponents<AudioSource> ();
			}
		}
		audioSource_Sound = audioSources [0];
		audioSource_Music = audioSources [1];
		audioSource_Death = audioSources [2];
		audioSource_Music.loop = true;
		audioSource_Music.volume = 0.55f;
	}

	public void PlaySound(AudioClip sound)
	{
		audioSource_Sound.clip = sound;
		audioSource_Sound.Play ();
	}

	public void PlayMusic(MUSIC music)
	{
		int m = (int)music;
		if (m >= 0 && m < audioClips_Music.Length) {
			audioSource_Music.clip = audioClips_Music [m];
			audioSource_Music.Play ();
		}
	}

	public void PlayMusic(MAP map)
	{
		int m = (int)map;
		m += 1;
		if (m >= 0 && m < audioClips_Music.Length) {
			audioSource_Music.clip = audioClips_Music [m];
			audioSource_Music.Play ();
		}
	}

	public void PlayMusic(AudioClip music)
	{
		audioSource_Music.clip = music;
		audioSource_Music.Play ();
	}

	public void PlayDeath()
	{
		audioSource_Death.Play ();
	}
}


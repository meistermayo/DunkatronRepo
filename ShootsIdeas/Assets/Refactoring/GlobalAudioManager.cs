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
    public enum ROBOTCLIP
    {
        TITLE=0,

        SELECT_CHARACTER,
        CHAR_BLUE,
        CHAR_MEGO,
        CHAR_EDJ,
        CHAR_PRIN,
        CHAR_BITZ,
        CHAR_FLITCH,
        CHAR_ODORE,
        CHAR_UNALIS,
        CHAR_MOEG,
        CHAR_SENOR,
        CHAR_RANGLE,
        CHAR_SKULLDIER,
        CHAR_GAZE,
        CHAR_SLICEY,
        CHAR_MRFISH,
        CHAR_FROOP,
        CHAR_PIPPY,
        CHAR_GUG,

        SELECT_WEAPON,
        WEP_MACHINEGUN,
        WEP_PEASHOOTER,
        WEP_ROCKETLAUNCHER,
        WEP_SUPERSHOTGUN,
        WEP_KATANA,
        WEP_SNIPER,
        WEP_DRAGONCLAW,
        WEP_FREEZERAY,
        WEP_WALL,
        WEP_BLUARC,
        WEP_LEECH,
        WEP_PIRAHNA,
        WEP_KAZOO,

        PLAYER_READY,

        SELECT_MAP,
        MAP_TRAVELING,
        MAP_CLASSIC,
        MAP_SPIRAL,
        MAP_PIT,
        MAP_MARKET,
        MAP_CORRIDOR,

        MATCH_READY,
        MATCH_GO,

        KILL_DELETE,
        KILL_DHIT,
        KILL_SELF_1,
        KILL_SELF_2,
        KILL_SELF_3,
        KILL_SELF_4,
        KILL_KAZOO,
        KILL_LEECH,
        KILL_SKEEL,
        KILL_SWOL,
        KILL_SUPERSKEEL,
        KILL_PINKOPANKO,

        MATCH_WIN,

        SILLY1,
        SILLY2,

        KILL_PIRAHNA,
        KILL_REFLECT,

        SILLY3
    }
    AudioSource audioSource_Sound;
	AudioSource audioSource_Music;
	AudioSource audioSource_Death;
    [SerializeField] AudioSource audioSource_RobotVoice;

    [SerializeField] AudioClip[] audioClips_Music;
    [SerializeField] AudioClip[] audioClips_Robot;
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
        float r = Random.value;

        if (r <= 0.98f)
            PlayRobotVoice(ROBOTCLIP.SILLY3);
        else if (r <= 0.99f)
            PlayRobotVoice(ROBOTCLIP.SILLY1);
        else
            PlayRobotVoice(ROBOTCLIP.SILLY2);

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
       // audioSource_RobotVoice = GetComponentInChildren<AudioSource>();
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

    public void PlayRobotVoice(ROBOTCLIP clip)
    {
        int _clip = (int)clip;
        if (_clip > -1 && _clip < audioClips_Robot.Length)
        {
            audioSource_RobotVoice.clip = audioClips_Robot[_clip];
            audioSource_RobotVoice.Play();
        }
    }
}


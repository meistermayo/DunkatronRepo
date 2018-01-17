using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PlayerTag))]
public class PlayerScript : MonoBehaviour
{
	protected PlayerTag playerTag;
	protected FXPlayer fxPlayer;

	void Start ()
	{
		playerTag = GetComponent<PlayerTag> ();
		fxPlayer = GetComponent<FXPlayer> ();
	}

	public PlayerTag GetPlayerTag()
	{
		playerTag = GetComponent<PlayerTag> ();
		return playerTag;
	}

	public FXPlayer GetFXPlayer()
	{
		return fxPlayer;
	}
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NUMBER_COL
{
	PLAYER_DMG = 0,
	MOB_DMG,
	RED,
	DEBUFF
}
public class NumberSpawner : MonoBehaviour {
	#region Singleton Shenanigans
	private static NumberSpawner instance;
	public static NumberSpawner Instance
	{
		get { return instance; }
		set { }
	}

	void Singleton()
	{
		if (instance == null)
			instance = this;
		else
			DestroyImmediate (this);
		DontDestroyOnLoad (gameObject);
	}
	#endregion

	[SerializeField] GameObject numberPrefab;
	[SerializeField] Color[] numberColors;

	void Awake()
	{
		Singleton ();
	}

	public void CreateNumber(Vector3 pos, string value, NUMBER_COL color, float speed, float duration, float scale)
	{
		if (color == NUMBER_COL.MOB_DMG)
			scale *= .5f;
		GameObject temp = Instantiate (numberPrefab, pos, Quaternion.identity);
		NumberController num = temp.GetComponent<NumberController> ();
		num.IniColor (value, numberColors[(int)color], speed, duration, scale);
	}

	public void CreateNumber(Vector3 pos, string value, Color color, float speed, float duration, float scale)
	{
		GameObject temp = Instantiate (numberPrefab, pos, Quaternion.identity);
		NumberController num = temp.GetComponent<NumberController> ();
		num.IniColor (value, color, speed, duration, scale);
	}

	public Color GetColor(NUMBER_COL num_col)
	{
		return numberColors [(int)num_col];
	}
}
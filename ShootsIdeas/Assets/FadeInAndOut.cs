using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInAndOut : MonoBehaviour {
	[SerializeField] float rate;
	float dir=1f;
	Image[] images;
	SpriteRenderer[] sprites;
	Text[] texts;
	Color myCol;

	void Start()
	{
		images = CreateArray<Image> ();
		sprites = CreateArray<SpriteRenderer> ();
		texts = CreateArray<Text> ();
	}


	T[] CreateArray<T>()
	{
		T[] local = GetComponents<T> ();
		T[] children = GetComponentsInChildren<T> ();

		T[] returnArray;

		//double check lengths of arrays
		int localLength = local.Length;
		int childrenLength = children.Length;
		/*
		if (local [0] == null)
			localLength--;
		if (children [0] == null)
			childrenLength--;
		*/
		returnArray = new T[localLength+childrenLength];

		// concatenate
		for (int i = 0; i < localLength; i++) {
			returnArray [i] = local [i];
		}
		for (int i = localLength; i < localLength + childrenLength; i++) {
			returnArray [i] = children [i - localLength];
		}

		return returnArray;
	}

	void Update()
	{
		ChangeAlpha ();
	}

	void ChangeAlpha()
	{
		myCol = new Color (1f, 1f, 1f, myCol.a + rate * dir);
		if (myCol.a >= 1f) {
			dir = -1f;
			myCol = new Color (1f, 1f, 1f, 1f);
		} else if (myCol.a <= 0f) {
			dir = 1f;
			myCol = new Color (1f,1f,1f,0f);
		}

		for (int i = 0; i < images.Length; i++) {
			images [i].color = myCol;
		}
		for (int i = 0; i < sprites.Length; i++) {
			sprites [i].color = myCol;
		}

		for (int i = 0; i < texts.Length; i++) {
			texts [i].color = myCol;
		}
	}
}

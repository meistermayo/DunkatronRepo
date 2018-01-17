using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTransitions : MonoBehaviour {
	public bool inText;
	public Text titleText;
	GameManager gameManager;

	void Awake()
	{
		gameManager = GetComponent<GameManager> ();
	}

	public IEnumerator FadeIn()
	{
		gameManager = GetComponent<GameManager> ();
		gameManager.SetControls (false);
		inText = true;
		while (titleText.color.a < 1f) {
			yield return new WaitForEndOfFrame ();
			Color newC = titleText.color;
			newC.a += .1f;
			titleText.color = newC;
		}

		yield return new WaitForSeconds (1f);

		while (titleText.color.a > 0f) {
			yield return new WaitForEndOfFrame ();
			Color newC = titleText.color;
			newC.a -= .1f;
			titleText.color = newC;
		}

		titleText.text = "GO!";
		titleText.fontSize = 64;

		while (titleText.color.a < 1f) {
			titleText.fontSize += 32;
			yield return new WaitForEndOfFrame ();
			Color newC = titleText.color;
			newC.a += .2f;
			titleText.color = newC;
		}

		while (titleText.color.a > 0f) {
			titleText.fontSize += 32;
			yield return new WaitForEndOfFrame ();
			Color newC = titleText.color;
			newC.a -= .025f;
			titleText.color = newC;
		}

		inText = false;
		gameManager.SetControls (true);
	}

	public IEnumerator EndGame(string name)
	{
		titleText.text = name + " WINS!";
		titleText.fontSize = 256;

		while (titleText.color.a < 1f) {
			yield return new WaitForEndOfFrame ();
			Color newC = titleText.color;
			newC.a += .5f;
			titleText.color = newC;
		}
	}

}

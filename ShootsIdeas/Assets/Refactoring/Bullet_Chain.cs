using UnityEngine;
using System.Collections;

public class Bullet_Chain : Base_Bullet
{
	[SerializeField] GameObject nextBullet;
	[SerializeField] int numberOfBullets;
	public GameObject NextBullet{get{return nextBullet;}}
	public int NumberOfBullets{ get { return numberOfBullets; } }

	public override void Awake ()
	{
		if (GetComponent<AudioSource>() != null)
			GlobalAudioManager.Instance.PlaySound (GetComponent<AudioSource> ().clip);
		mBody = GetComponent<Rigidbody2D> ();
		SetCrit ();
		StartCoroutine (DestroyRoutine(life));
	}

	IEnumerator DestroyRoutine(float seconds)
	{
		yield return new WaitForSeconds (seconds);

		for (int i = 0; i < numberOfBullets; i++) {
			GameObject temp = Instantiate (nextBullet, transform.position, transform.rotation);
			temp.GetComponent<Base_Bullet> ().SetInfo (id, team);
		}

		Destroy (gameObject);
	}
}


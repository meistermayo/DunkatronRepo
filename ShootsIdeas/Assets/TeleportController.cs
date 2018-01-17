using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(SpriteRenderer))]
public class TeleportController : MonoBehaviour {
	[SerializeField] TeleportController exitNode;
	[SerializeField] Color deacColor;
	[SerializeField] float cooldown = 5f;
	ParticleSystem particleSystem;
	SpriteRenderer spriteRenderer;
	bool activated=true;
	Color defaultColor;


	void Start()
	{
		particleSystem = GetComponent<ParticleSystem> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		if (exitNode == null) {
			PermanantDeactivate ();
		}
	}

	public void Deactivate()
	{
		if (activated) {
			activated = false;
			particleSystem.Stop ();
			StopAllCoroutines ();
			defaultColor = spriteRenderer.color;
			spriteRenderer.color = deacColor;
			StartCoroutine (Reactivate (cooldown));
		}
	}

	void PermanantDeactivate()
	{
		activated = false;
		particleSystem.Stop ();
		StopAllCoroutines ();
		defaultColor = spriteRenderer.color;
		spriteRenderer.color = deacColor;
	}

	public void Activate()
	{
		if (!activated)
		{
			activated = true;
			particleSystem.Play ();
			spriteRenderer.color = defaultColor;
		}
	}

	IEnumerator Reactivate(float _cooldown)
	{
		yield return new WaitForSeconds (_cooldown);
		Activate ();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!activated)
			return;
		
		PlayerController player = other.GetComponent<PlayerController> ();

		if (player != null) {
			Teleport (player);
		}
	}

	public void Teleport(PlayerController playerToTeleport)
	{
		exitNode.Deactivate ();
		playerToTeleport.transform.position = exitNode.transform.position;
		if (playerToTeleport.GetComponent<PlayerEnemyHandler>().enemy_count > 0)
			if (playerToTeleport.GetComponent<PlayerEnemyHandler> ().CurrentEnemy != null)
				playerToTeleport.GetComponent<PlayerEnemyHandler> ().CurrentEnemy.transform.position = exitNode.transform.position;
		Deactivate ();
	}
}

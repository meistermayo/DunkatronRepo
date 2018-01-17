using UnityEngine;
using System.Collections;

public class Base_Health : PlayerScript
{

	[SerializeField] protected float health, health_max;
	[SerializeField] protected float armor, resistance;
	[SerializeField] protected bool invincibilty;
	[SerializeField] protected GameObject diePrefab;
	[SerializeField] protected NUMBER_COL numCol;

	protected bool stun;
	protected int stunStack;
	public bool Stun { get { return stun; } }

	public float Health {get {return health;}}
	public float Health_Max {get {return health_max;}}
	public bool Invincibility { get { return invincibilty; } }


	public virtual void TakeDamage(float damage, int otherId, int otherTeam)
	{
		if (health <= 0f)
			return;
		if (playerTag.Id == otherId)
			return;
		if (otherTeam != 0 && playerTag.Team == otherTeam)
			return;
		if (invincibilty)
			return;
		
		if (armor > 0f) {
			damage *= 1f - armor;
			if (armor >= 100f)
				armor = 99f;
			else
				armor -= damage * .01f;
			if (armor < 0f)
				armor = 0f;
		}

		health -= damage;

		//FX
		NumberSpawner.Instance.CreateNumber (transform.position, Mathf.Round(damage).ToString(),numCol, .1f, 120f, 3f);
		GetComponentInChildren<AudioManager> ().Play (1);
		GetComponentInChildren<ParticleController> ().Play (3);

		if (health <= 0f)
			Die (otherId);
	}


	public virtual void Die (int otherId)
	{
		Instantiate (diePrefab, transform.position, Quaternion.identity);
		Destroy (gameObject);
	}

	public IEnumerator InvincibilityRoutine(float frames)
	{
		SpriteRenderer childSpriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		invincibilty = true;
		while (frames > 0) {
			yield return new WaitForEndOfFrame ();
			childSpriteRenderer.enabled = !childSpriteRenderer.enabled;
			frames--;
		}
		invincibilty = false;
	}

	public virtual void Heal(float heal)
	{
		health += heal;
		if (health > health_max)
			health = health_max;
		NumberSpawner.Instance.CreateNumber (transform.position, Mathf.Round(heal).ToString(),Color.green, .1f, 120f, 3f);
	}


	public virtual void SetHealth(float _health, float _health_max, float _armor)
	{
		health = _health;
		health_max = _health_max;
		armor = _armor;
	}


	public virtual void AddStun()
	{
		stunStack++;
		if (stunStack > 3)
			StartCoroutine (StunSelf ());
	}

	public virtual IEnumerator StunSelf()
	{
		stun = true;
		yield return new WaitForSeconds (1f * Time.timeScale);
		stun = false;
		stunStack = 0;
	}

	public void SetInvincibilty(bool _invincibility)
	{
		this.invincibilty = _invincibility;
	}

}


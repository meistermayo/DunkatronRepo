using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Bullet : MonoBehaviour {
	[SerializeField] protected float move_speed;
	public float Move_Speed { get { return move_speed; } }
	[SerializeField] protected float damage;
	[SerializeField] protected float crit_chance;
	[SerializeField] protected float crit_damage;
	public float Damage { get { return damage; } }
	public float Crit_Chance { get { return crit_chance; } }
	public float Crit_Damage { get { return crit_damage; } }
	[SerializeField] protected float life;
	[SerializeField] protected bool canStun;
	public bool CanStun{ get { return canStun; } }
	[SerializeField] protected bool destroyOnHit=true;
	[SerializeField] protected GameObject deathParticlePrefab;
	[SerializeField] protected bool noShake;
	protected bool canDamage=true;

	protected int id,team;
	public int Id { get { return id; } }
	public int Team { get { return team; } }
	protected Rigidbody2D mBody;
	protected Animator animator;

    [SerializeField] protected Player_Health.KILL_FLAGS killFlag;

	public virtual void Awake()
	{
		animator = GetComponent<Animator> ();
		if (GetComponent<AudioSource>() != null)
			GlobalAudioManager.Instance.PlaySound (GetComponent<AudioSource> ().clip);
		mBody = GetComponent<Rigidbody2D> ();
		SetCrit ();
		StartCoroutine (Life(life));
	}

	public virtual void SetInfo(int _id, int _team)
	{
		mBody.velocity = transform.right * move_speed;
		if (!noShake)
			Camera.main.transform.position += (Vector3)((-mBody.velocity).normalized*damage*.005f);
		id = _id;
		team = _team;
	}

	protected void SetCrit()
	{
		int r = Random.Range(0,100);
		if (r < crit_chance) {
			damage *= crit_damage;
			damage = Mathf.Ceil (damage);
		}
	}

	public virtual void OnTriggerEnter2D(Collider2D other)
	{
		//RETURN CONDITIONS
		if (other.tag == "Wall") {
			if (deathParticlePrefab != null)
				Instantiate (deathParticlePrefab, transform.position, transform.rotation);
			if (destroyOnHit) {
				Destroy (gameObject);
				return;
			}
			return;
		}
		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth == null || otherHealth.GetPlayerTag ().Id == id || (otherHealth.GetPlayerTag().Team != 0 && otherHealth.GetPlayerTag().Team == team))
			return;

		if (canStun)
			otherHealth.AddStun ();
		
		DamageTarget (otherHealth);
		// ELSE CASE??

	}

	protected virtual void DamageTarget(Base_Health otherHealth)
	{
		if (!canDamage)
			return;
        //DAMAGE
        otherHealth.SetKillFlag(killFlag);
		otherHealth.TakeDamage (damage, id, team);
		/*
		Rigidbody2D otherBody = otherHealth.GetComponent<Rigidbody2D> ();
		float angle = transform.rotation.eulerAngles.z;
		otherBody.AddForce (new Vector2(Mathf.Cos (angle * Mathf.Deg2Rad),Mathf.Sin(angle * Mathf.Deg2Rad)) * damage * .25f, ForceMode2D.Impulse);
		*/
//FX
		if (deathParticlePrefab != null)
			Instantiate (deathParticlePrefab, transform.position, Quaternion.identity);

		//DESTRUCTION HANDLER
		if (destroyOnHit) {
			canDamage = false;
			Destroy (gameObject);
		}
	}

	public void PowerupParticles()
	{
		transform.GetChild (0).gameObject.SetActive (true); // MAYBE DO THIS DIFFERNTLY?
	}

	public void MultiplyDamage(float mult)
	{
		damage *= mult;
		damage = Mathf.Ceil (damage);
	}

	protected virtual IEnumerator Life(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		if (animator != null)
			animator.SetTrigger ("Die");
		yield return new WaitForSeconds (.15f);
		Destroy (gameObject);
	}

    public void ResetKillFlag(Base_Health.KILL_FLAGS killFlag)
    {
        this.killFlag = killFlag;
    }
}

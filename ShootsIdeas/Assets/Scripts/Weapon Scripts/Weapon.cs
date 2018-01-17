using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public  class Weapon : MonoBehaviour {
	
	public float cooldown;
	public GameObject hitbox;
	public float timer;
	public bool can_attack = true;
	public float h,v;
	public int player_num;
	public float damageMult = 1f;
	public float personalDamageMult = 1f;
	public float damageRed = 1f;
	public float attackSpeedRed = 1f;
	public BulletScript currentBullet;
	public int audioClip;
	public bool updateInput = true;

	public void Awake()
	{
		player_num = GetComponentInParent<MovementScript> ().player_num;
	}

	public virtual void Update()
	{
		//Gets input, attacks
		if(updateInput)
			GetInput ();
		//if (h != 0f || v != 0f)
		//	if (can_attack)
		//		Attack ();
	}

	public virtual void GetInput()
	{
		//Two axes input
		h = Input.GetAxisRaw ("h2_" + player_num);
		v = Input.GetAxisRaw ("v2_" + player_num);
		//Debug.Log ("h: " + h + "\nv: " + v);
	}

	public virtual void Attack()
	{
		if (!can_attack)
			return;
		if (updateInput)
			GetInput ();
		// Create hitbox, start cooldown coroutine
		transform.parent.GetComponentInChildren<ParticleController> ().Play (0);
		if (audioClip <= 0) {
			if (damageMult == 1f)
				transform.parent.GetComponentInChildren<AudioManager> ().Play (0);
			else
				transform.parent.GetComponentInChildren<AudioManager> ().Play (4);
		} else {
			transform.parent.GetComponentInChildren<AudioManager> ().Play (audioClip);
		}
			
		can_attack = false;
		GameObject temp = Instantiate ( hitbox, transform.position, Quaternion.Euler( new Vector3(0f,0f,Mathf.Rad2Deg*Mathf.Atan2(-v,h)) ) );
		currentBullet = temp.GetComponent<BulletScript> ();
		currentBullet.team = GetComponentInParent<MovementScript> ().team;
		CircleCollider2D col = temp.GetComponent<CircleCollider2D> ();
		Physics2D.IgnoreCollision (col,GetComponent<CapsuleCollider2D>());
		currentBullet.id = player_num;
		currentBullet.damage = Mathf.Ceil(currentBullet.damage*(personalDamageMult * damageMult)*damageRed);
		if (damageMult > 1f)
			currentBullet.PowerupParticles ();
//		Rigidbody2D bulletBody = temp.GetComponent<Rigidbody2D> ();
		//Debug.Log ("V: " + v + "\nH: " + h);
		//bulletBody.velocity = Vector2.ClampMagnitude(new Vector2 (h,v) * currentBullet.move_speed,currentBullet.move_speed);
		StartCoroutine (Cooldown());
	}

	//equip
	public virtual void Equip(bool on)
	{
		// Stat changes on use
	}


	//cooldown
	public virtual IEnumerator Cooldown()
	{
		float _cooldown = (cooldown * attackSpeedRed);
		for (int i = 0; i < _cooldown; i++)
			yield return new WaitForFixedUpdate ();//WaitForSeconds (_cooldown*.016f*Time.timeScale);
		can_attack = true;
	}

}

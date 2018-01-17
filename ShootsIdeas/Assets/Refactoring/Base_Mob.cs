using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Base_Health))]
[RequireComponent(typeof(Base_Movement))]
[RequireComponent(typeof(PlayerTag))]
public abstract class Base_Mob : MonoBehaviour {
	[SerializeField] protected float damage;
	[SerializeField] protected float cooldown;
	[SerializeField] protected float maxDist;
	[SerializeField] protected float lerpTime;

	protected GameObject parentObject;
	protected bool canAttack=true;

	protected Base_Health healthScript;
	protected Base_Movement movementScript;
	protected Targetter targetter;
	protected PlayerTag playerTag;
 	protected Rigidbody2D body;

	protected bool released;

	public virtual void MainBehavior ()
	{
		//if (h != 0f && v != 0f) 
		{
			if (parentObject != null)
				body.MovePosition(Vector2.Lerp (transform.position,parentObject.transform.position + new Vector3(h,-v) * maxDist,lerpTime));
		}
	}

	public abstract void ReleasedBehavior();

	protected float h,v;

	protected virtual void Start()
	{
		playerTag = GetComponent<PlayerTag> ();
		movementScript = GetComponent<Base_Movement> ();
		healthScript = GetComponent<Base_Health> ();
		body = GetComponent<Rigidbody2D> ();
		targetter = GetComponentInChildren<Targetter> ();
	}

	protected virtual void Update()
	{
		GetInput ();
		if (released) {
			ReleasedBehavior ();
		} else {
			MainBehavior ();
		}
	}

	public virtual void Release()
	{
		parentObject = null;
		released = true;
		//anything else you want.
	}

	protected IEnumerator AttackCooldown()
	{
		canAttack = false;
		yield return new WaitForSeconds (cooldown);
		canAttack = true;
	}

	protected virtual void GetInput()
	{
		h = Input.GetAxisRaw ("h1_" + playerTag.Id);
		v = -Input.GetAxisRaw ("v1_" + playerTag.Id);
	}

	public virtual void Ini(GameObject parent, int id, int team, bool _released)
	{
		playerTag = GetComponent<PlayerTag> ();
		movementScript = GetComponent<Base_Movement> ();
		healthScript = GetComponent<Base_Health> ();
		targetter = GetComponentInChildren<Targetter> ();

		if (playerTag == null)
			playerTag = GetComponent < PlayerTag> ();
		playerTag.SetId (id);
		playerTag.SetTeam (team);

		if (_released)
			Release ();
		else
			parentObject = parent;
	}

	public void RemoveFromParent()
	{
		if (parentObject == null)
			return;
		PlayerEnemyHandler playerEnemyHandler = parentObject.GetComponent<PlayerEnemyHandler> ();
		if (playerEnemyHandler != null) {
			playerEnemyHandler.enemy_count--;
		}
	}
}

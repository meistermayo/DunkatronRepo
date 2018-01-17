using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AI;

public enum STATE{FOLLOW,CHASE}
public enum CHASE_TYPE{PLAYER, HAZARD, HUMAN}
public enum MOB_TYPE{SKEEL,SWOLZ,PINKO}

public class OLD_BaseMob : Base_Health
{
	[SerializeField] protected float move_speed;
	[SerializeField] protected float damage;
	[SerializeField] protected float onMobMultiplier;
	[SerializeField] protected float onPlayerMultiplier;
	[SerializeField] protected float scoreValue;
	[SerializeField] protected MOB_TYPE type;
	public MOB_TYPE Type { get {return type;}}
	//[SerializeField] protected GameObject explosionObject; // REFACTOR???
	[SerializeField][Range(0,Mathf.Infinity)] protected int pathRoutineBuffer;

	protected STATE state;
	protected CHASE_TYPE chase_type;
	protected ParticleSystem[] particleSystems;
	protected GameObject playerObject;
	protected GameObject targetObject;

	protected Vector3 sideVec;
	protected float[][] map;
	protected float[] path;
	protected float hit_timer=1f;
	protected float move_mult = 1f;
	protected float move_clamp = 1f;
	protected int side;
	protected bool can_hit=true;

	protected SpriteRenderer outlineSprite;
	protected AudioManager audioManager;
	protected PlayerEnemyHandler playerEnemyHandler;
	protected EnemyPathfinding pathfinding;
	protected Rigidbody2D mBody;
	protected SpriteRenderer sr;
	protected Animator mAnim;

	void Awake()
	{
		mAnim = GetComponentInChildren<Animator> ();
		sr = GetComponentInChildren<SpriteRenderer> ();
		audioManager = GetComponentInChildren<AudioManager> ();
		mBody = GetComponent<Rigidbody2D> ();
		pathfinding = GetComponent<EnemyPathfinding> ();
		particleSystems = GetComponentsInChildren<ParticleSystem> ();

		pathfinding.seeker = gameObject.transform;
		StartCoroutine (PathRoutine ());
	}

	public void SetPlayerObject(GameObject player)
	{
		playerObject = player;
		playerEnemyHandler = player.GetComponent<PlayerEnemyHandler> ();
	}

	void Update()
	{
		CheckMoveMult (); // !! REFACTOR

		if (state == STATE.FOLLOW) {
			if (playerObject == null) 
			{
				state = STATE.CHASE;
				if (targetObject == null)
					FindTarget ();
			}
			Follow ();
		}
		else
			Chase ();
		

	}

	void FindTarget()
	{
		for (int i = 0; i < 4; i++) {
			if (GameManager.Instance.GetPlayer (i) == null)
				continue;

			Player_Health playerHealth = GameManager.Instance.GetPlayer(i).GetComponent<Player_Health> ();
			if (playerHealth.GetPlayerTag().Id != playerTag.Id) {
				targetObject = GameManager.Instance.GetPlayer (i);
				break;
			}

		}
	}

	void Animate(Vector3 moveVec)
	{
		if (moveVec.y < 0f)
			mAnim.SetFloat ("dir", -1.0f);
		else if (moveVec.y > 0f)
			mAnim.SetFloat ("dir", 1.0f);
		else
			mAnim.SetFloat ("dir", 0.0f);

		if (moveVec.x > 0f)
			sr.flipX = true;
		else if (moveVec.x < 0f)
			sr.flipX = false;
	}

	// follows player
	void Follow()
	{
		if (playerObject == null)
			return;
		if (playerObject.activeSelf == false)
			return;
		pathfinding.target = playerObject.transform;
	}

	// follows target object
	protected virtual void Chase()
	{
		if (targetObject == null) {
			state = STATE.FOLLOW;
			return;
		}
		if (targetObject.activeSelf == false) {
			state = STATE.FOLLOW;
			return;
		} // ?

		pathfinding.target = targetObject.transform;
	}

	IEnumerator PathRoutine()
	{
		while (true) {
			PathMove ();
			yield return new WaitForSeconds(Random.Range(0f,1f)*Time.deltaTime);
		}
	}

	void PathMove() // Needs an advanced look REFACTOR
	{ 
		List<Node> path = pathfinding.targetPath;
		if (targetObject != null) {
			RaycastHit2D hit = Physics2D.Linecast (transform.position, targetObject.transform.position, 8);		// BASE CASE??

			if (hit == null) {
				Vector3 moveVec = Vector3.ClampMagnitude ((targetObject.transform.position - transform.position) * move_speed, move_speed*move_mult*move_clamp);
				mBody.MovePosition (transform.position + moveVec);
				Animate (moveVec);
				return;

			}
		}

		if (path != null)
		if (path.Count > 1) {
			float dx, dy;

			dx = -path [0].worldPosition.x + path [1].worldPosition.x;
			dy = -path [0].worldPosition.y + path [1].worldPosition.y;

			mBody.velocity = Vector2.zero;
			Vector3 moveVec = Vector3.ClampMagnitude (new Vector3 (dx, dy, 0f) * move_speed, move_speed * move_mult * move_clamp);
			mBody.MovePosition (transform.position + moveVec);
			Animate (moveVec);
		} else {

			if (targetObject != null) {
				Vector3 moveVec = Vector3.ClampMagnitude ((targetObject.transform.position - transform.position) * move_speed, move_speed * move_mult * move_clamp);
				mBody.MovePosition ( transform.position + moveVec);
				Animate (moveVec);
			}

		}
	}


	protected virtual void OnTriggerStay2D(Collider2D other)
	{
		if (can_hit){
			MeleeAttack(other);
		}
	}

	protected virtual void MeleeAttack(Component other)
	{
		Base_Health otherHealth = other.GetComponent<Base_Health> ();
		if (otherHealth == null)
			return;
		if (playerTag.Team != 0 && otherHealth.GetPlayerTag().Team == playerTag.Team)
			return;
		float multipliedDamage = damage;
		if (other.GetComponent<OLD_BaseMob> () != null)
			multipliedDamage *= onMobMultiplier;
		else if (other.GetComponent<Player_Health> () != null)
			multipliedDamage *= onPlayerMultiplier;
		
		if (otherHealth.GetPlayerTag().Id != playerTag.Id) {
			otherHealth.TakeDamage (multipliedDamage, playerTag.Id, playerTag.Team);
		}
		StartCoroutine (HitRoutine ());
	}

	IEnumerator HitRoutine()
	{
		can_hit = false;
		yield return new WaitForSeconds (hit_timer);
		can_hit = true;
	}

	public IEnumerator FlashRoutine()
	{
		SpriteRenderer sr = GetComponentInChildren<SpriteRenderer> ();
		Sprite sprite = sr.sprite;
		sr.sprite = GetComponentInChildren<SpriteRenderer> ().sprite;
		Debug.Log (sr.sprite.name);
		yield return new WaitForSeconds (.1f);
		sr.sprite = sprite;
	}

	void CheckMoveMult()
	{
		// NEEDS TO BE INHERITED REFACTOR
		if (move_mult < 1f) {
			move_mult += .001f;
			if (move_mult > 1f)
				move_mult = 1f;
		}
	}
		
	public override void TakeDamage (float damage, int otherId, int team)
	{
		base.TakeDamage (damage, otherId, team);
		NumberSpawner.Instance.CreateNumber (transform.position, damage.ToString(), NUMBER_COL.MOB_DMG, .1f, 120f, 2f);
		particleSystems [1].Play ();
		audioManager.Play (0);
		StartCoroutine (FlashRoutine ());
	}

	public override void Die(int otherId)
	{
		if (playerEnemyHandler != null)
			playerEnemyHandler.enemy_count--;
		
		Instantiate (diePrefab, transform.position, Quaternion.identity);

		if (GameManager.gameMode == GAMEMODE.PVE) {
			GameManager.score += scoreValue;
		}
			
		// TODO? ??
		Destroy (gameObject);
	}

	public void SetLoose()
	{
		playerObject = null;
		state = STATE.CHASE;
	}

	public void SetColor(Color col)
	{
		outlineSprite = sr.GetComponentInChildren<SpriteRenderer> ();
		outlineSprite.color = col;
		GetComponentInChildren<SpriteRenderer> ().color = col; 
	}

	public void SetColor (int id)
	{
		Color col = Color.white;
		if (id == 1)
			col = Color.blue;
		else if (id == 2)
			col = Color.yellow;
		else if (id == 3)
			col = Color.red;
		else if (id == 4)
			col = Color.magenta;

		SetColor (col);
	}

	public void IgnoreCollision()
	{
		
		for (int i = 0; i < 4; i++) { // ignore collsion with player
			if (GameManager.Instance.GetPlayer (i) == null)
				continue;
			GameObject player = GameManager.Instance.GetPlayer (i);
			if (player.GetComponent<PlayerScript>().GetPlayerTag().Id == GetPlayerTag().Id) {
				Physics2D.IgnoreCollision (player.GetComponent<CapsuleCollider2D> (), GetComponent<CircleCollider2D> ());
			}
		}
	}



	public void Ini(int side, Vector2 sideVec, GameObject playerObject, Color col,PlayerEnemyHandler playerEnemyHandler)
	{
		this.side = side;
		this.sideVec = sideVec;
		this.playerObject = playerObject;
		outlineSprite = GetComponentInChildren<SpriteRenderer> ().GetComponentInChildren<SpriteRenderer>();
		this.outlineSprite.color = col;
		this.playerEnemyHandler = playerEnemyHandler;
	}
}


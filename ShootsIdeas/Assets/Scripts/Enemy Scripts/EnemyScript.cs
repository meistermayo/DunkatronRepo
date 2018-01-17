using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyScript : MonoBehaviour {

	public float hp_max = 100f, hp = 100f;
	public Text hp_text;

	public STATE state;
	public int id, side;
	public Vector3 sideVec;
	public float move_speed;
	public GameObject playerObject;
	public GameObject targetObject;
	public float[][] map;
	public float[] path;
	public PlayerEnemyHandler playerEnemyHandler;
	public CHASE_TYPE chase_type;
	EnemyPathfinding pathfinding;
	Rigidbody2D mBody;
//	Grid grid;
	public int type;
	public GameObject brainProjectilePrefab;
	public bool can_hit=true;
	public float hit_timer=1f;
	public SpriteRenderer outlineSprite;
	public ParticleSystem[] particleSystems;
	public AudioManager audioManager;
	public GameObject explosionObject;
	SpriteRenderer sr;
	Animator mAnim;
	public float move_mult = 1f;
	public int team;
	public GameObject number;
	public float move_clamp = 1f;
	void Awake()
	{
		mAnim = GetComponent<Animator> ();
		sr = GetComponent<SpriteRenderer> ();
		audioManager = GetComponentInChildren<AudioManager> ();
		mBody = GetComponent<Rigidbody2D> ();
		//grid = GetComponent<Grid> ();
		pathfinding = GetComponent<EnemyPathfinding> ();
		pathfinding.seeker = gameObject.transform;
		StartCoroutine (PathRoutine ());
		particleSystems = GetComponentsInChildren<ParticleSystem> ();
	}


	void Update()
	{
		CheckMoveMult ();

		hp_text.text = Mathf.Ceil(hp).ToString ();
		if (hp <= 0f) {
			if (playerEnemyHandler != null)
				playerEnemyHandler.enemy_count--;
			//playerEnemyHandler.enemies_active [side] = null;
			Instantiate (explosionObject, transform.position, Quaternion.identity);
			if (GameManager.gameMode == GAMEMODE.PVE) {
				float baseValue = 100f;
				if (type == 0)
					baseValue = 50f;
				else if (type == 1)
					baseValue = 150f;
				//baseValue *= GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameManager> ().enemyMult;
				GameManager.score += baseValue;
			}
			Destroy (gameObject);
		}

		if (playerObject == null) {
			state = STATE.CHASE;
			if (targetObject == null) {
				GameObject[] temp = GameObject.FindGameObjectsWithTag ("Player");
				for (int i = 0; i < temp.Length; i++) {
					MovementScript movScr = temp [i].GetComponent<MovementScript> ();
					if (movScr.player_num != id) {
						targetObject = temp [i];
						break;
					}
				}
			}
		}
		
		if (state == STATE.FOLLOW)
		{
			Follow ();
		}
		else
		{
			Chase ();
		}

	}

	void Animate(Vector3 moveVec)
	{
		
		//moveVec.x = Mathf.Sign(moveVec.x);
	//	moveVec.y = Mathf.Sign (moveVec.y);

		Debug.Log (moveVec);

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
		if (playerObject.activeSelf == false)
			return;
		pathfinding.target = playerObject.transform;
		//transform.position = Vector3.Lerp (transform.position,playerObject.transform.position + sideVec,move_speed);
	}

	// follows target object
	void Chase()
	{
		
		if (targetObject == null) {
			state = STATE.FOLLOW;
			return;
		}

		if (targetObject.activeSelf == false) {
			state = STATE.FOLLOW;
			return;
		}
		if (type == 2) {
			if (targetObject.GetComponent<PickupRespawn> () != null) {
				if (!targetObject.GetComponent<PickupRespawn> ().active) {
					state = STATE.FOLLOW;
					return;
				}
			}
		}
		pathfinding.target = targetObject.transform;
		//transform.position = Vector3.MoveTowards (transform.position,targetObject.transform.position,move_speed);
	}

	IEnumerator PathRoutine()
	{
		while (true) {
			PathMove ();
			for (int i=0; i<1; i++)
				yield return new WaitForEndOfFrame ();
		}
	}

	void PathMove()
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
			/*
			if (!grid.NodeFromWorldPoint (transform.position + new Vector3 (dx, 0f, 0f)).walkable)
				dx = 0f;
			if (!grid.NodeFromWorldPoint (transform.position + new Vector3 (0f, dy, 0f)).walkable)
				dy = 0f;
			*/
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


	void OnTriggerStay2D(Collider2D other)
	{
		if (type == 2) { // PINKO PANKO
			if (other.tag == "EnemyPickup" || other.tag == "Powerup") {
				PickupRespawn otherPickup = other.GetComponent<PickupRespawn> ();
				if (!otherPickup.active)
					return;
				otherPickup.StartRespawnTimer ();

				GameObject temp = Instantiate (brainProjectilePrefab, transform.position, Quaternion.identity);
				EnemyScript projectileScript = temp.GetComponent<EnemyScript> ();

				temp.GetComponentInChildren<SpriteRenderer> ().color = GetComponent<SpriteRenderer> ().color; // SETUP
				projectileScript.id = id;
				projectileScript.hp = 50f;
				projectileScript.hp_max = 50f;

				projectileScript.playerObject = null; // SET TO CHASE.
				projectileScript.team =team;


				Color col = Color.white;
				if (id == 1)
					col = Color.blue;
				else if (id == 2)
					col = Color.yellow;
				else if (id == 3)
					col = Color.red;
				else if (id == 4)
					col = Color.magenta;
				
				projectileScript.outlineSprite.color = col;

				GameObject[] temp2 = GameObject.FindGameObjectsWithTag("Player"); // Ignore player colilsion
				for (int i=0; i<temp2.Length;i++)
				{
					MovementScript playerMov = temp2 [i].GetComponent<MovementScript> ();
					if (playerMov.player_num == id)
					{
						Physics2D.IgnoreCollision (temp2[i].GetComponent<CapsuleCollider2D>(),projectileScript.GetComponent<CircleCollider2D>());
					}
				}

			}
		} else if (can_hit){ // Everything Else
			EnemyScript enemy = other.GetComponent<EnemyScript> ();
			if (enemy == null)
				return;
			if (team != 0 && enemy.team == team)
				return;
			if (enemy.id != id) {
				float damage;

				if (type == 0) {
					damage = 10f;
					enemy.hp -= 10f;
				} else {
					damage = 75f;
					enemy.hp -= 75f; // TANKS DO DAMAGE VS MOBS
				}

				enemy.CreateNumber (enemy.transform.position, damage.ToString(), new Color(1f,.9f,.9f), .1f, 120f,				 2f);
				enemy.StartCoroutine(enemy.FlashRoutine());
				enemy.particleSystems[1].Play();
				enemy.audioManager.Play (0);
			}
			StartCoroutine (HitRoutine ());
		}
	}

	IEnumerator HitRoutine()
	{
		can_hit = false;
		yield return new WaitForSeconds (hit_timer);
		can_hit = true;
	}

	public IEnumerator FlashRoutine()
	{
		SpriteRenderer sr = GetComponent<SpriteRenderer> ();
		Sprite sprite = sr.sprite;
		sr.sprite = GetComponentInChildren<SpriteRenderer> ().sprite;
		Debug.Log (sr.sprite.name);
		yield return new WaitForSeconds (.1f);
		sr.sprite = sprite;
	}


	public void CreateNumber(Vector3 pos, string value, Color color, float speed, float duration, float scale)
	{
		GameObject temp = Instantiate (number, pos, Quaternion.identity);
		NumberController num = temp.GetComponent<NumberController> ();
		num.IniColor (value, color, speed, duration, scale);
	}


	void CheckMoveMult()
	{
		if (move_mult < 1f) {
			move_mult += .001f;
			if (move_mult > 1f)
				move_mult = 1f;
		}
	}
}

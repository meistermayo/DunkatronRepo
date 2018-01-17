using UnityEngine;
using System.Collections;

public class Base_Movement : MonoBehaviour
{
	[SerializeField] protected float move_mult;
	[SerializeField] protected float move_speed;
	[SerializeField] protected float slowMax = .25f;
	public float Move_Speed { get { return move_speed*move_mult; } }

	public void AddSlow (float amount){
		move_mult = Mathf.Max (slowMax, move_mult - amount);
	}

}


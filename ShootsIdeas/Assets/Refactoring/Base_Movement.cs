using UnityEngine;
using System.Collections;

public class Base_Movement : MonoBehaviour
{
	[SerializeField] protected float move_mult;
	[SerializeField] protected float move_speed;
	protected float move_mult_internal=1f;
	[SerializeField] protected float slowMax = .25f;
	public float Move_Speed { get { return move_speed*(move_mult)*(move_mult_internal); } }

	public void AddSlow (float amount){
		move_mult = Mathf.Max (slowMax, move_mult - amount);
	}

	public void SetMoveMultInternal(float amount)
	{
		move_mult_internal = amount;
	}
}


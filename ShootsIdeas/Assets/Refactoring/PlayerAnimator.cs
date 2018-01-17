using UnityEngine;
using System.Collections;

public class PlayerAnimator : MonoBehaviour
{
	SpriteRenderer spriteRenderer;
	Animator animator;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer> ();
		animator = GetComponentInChildren<Animator> ();
	}

	public void Animate(float h, float v)
	{
		if (h != 0f)
		{
			spriteRenderer.flipX = (h < 0f);
		}
		animator.SetFloat("dir",v);
	}
}


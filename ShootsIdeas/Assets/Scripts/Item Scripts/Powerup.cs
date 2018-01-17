using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Powerup : MonoBehaviour{
	[SerializeField] public POWERUP_TYPE type;
	[SerializeField] protected float buffValue;
	[SerializeField] protected float defaultValue;
	[SerializeField] protected float duration;
	[SerializeField] protected bool canStack;
	public bool CanStack{get {return canStack;}}
	public float Duration{ get { return duration; } }

	public abstract void Buff ();
	public abstract void DeBuff ();

	public void Copy(Powerup other)
	{

		this.canStack = other.canStack;
		this.buffValue = other.buffValue;
		this.defaultValue = other.defaultValue;
		this.duration = other.duration;
	}

	public void SetValue(float value)
	{
		buffValue = value;
	}
}

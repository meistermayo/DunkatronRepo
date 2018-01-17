using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refresh_Bullet : Bullet_Sender {


	public override void CallSender()
	{
		sender.ResetCooldown ();
	}

}

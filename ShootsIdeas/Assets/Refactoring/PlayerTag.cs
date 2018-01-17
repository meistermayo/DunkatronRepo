using UnityEngine;
using System.Collections;

public class PlayerTag : MonoBehaviour
{
	[SerializeField][Range(-1,3)] private int id = -1;
	public int Id{
		get { return id; }
		set {
			if (id == -1)
				id = value;
		}
	}

	[SerializeField][Range(-1,4)] private int team = -1;
	public int Team{
		get { return team; }
		set {
			if (team == -1)
				team = value;
		}
	}

	public void SetTeam(int team)
	{
		this.team = team;
	}

	public void SetId(int id)
	{
		this.id = id;
	}
}


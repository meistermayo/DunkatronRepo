using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsDisplay : MonoBehaviour {
	public Text statsText;
	Child[] mostChild;
	bool[] tied;

	// Use this for initialization
	void Start () {
		//statsText = GetComponentInChildren<Text> ();	

		mostChild = new Child[6];
		tied = new bool[6];

//		Child mostPlayed, mostKills, mostDeaths, mostWins, mostLosses, bestKDA;
		//int maxPlayed=0, maxKills=0, maxDeaths=0, maxWins=0, maxLosses=0, bestKDA=0;
		//bool tiedPlayed, tiedKills, tiedDeaths, tiedWins, tiedLosses, tiedKDA;

		if (ChildQueueController.child_profiles.Count == 0)
			return;
		
		for (int i = 0; i < mostChild.Length; i++) {
			mostChild [i] = ChildQueueController.child_profiles [0];
		}

		if (mostChild [0] == null)
			return;

		for (int i = 0; i < ChildQueueController.child_profiles.Count; i++) {
			CompareChild (ChildQueueController.child_profiles [i].gamesPlayed,  mostChild [0].gamesPlayed,	i, 0);
			CompareChild (ChildQueueController.child_profiles [i].kills, 		mostChild [1].kills, 		i, 1);
			CompareChild (ChildQueueController.child_profiles [i].deaths,		mostChild [2].deaths,		i, 2);
			CompareChild (ChildQueueController.child_profiles [i].wins,			mostChild [3].wins, 		i, 3);
			CompareChild (ChildQueueController.child_profiles [i].losses, 		mostChild [4].losses,		i, 4);

			Child compareChild = ChildQueueController.child_profiles [i];
			CompareChild (compareChild.kills / (mostChild[5].deaths + (mostChild[5].deaths == 0 ? 1 : 0)), mostChild [5].kills / (mostChild[5].deaths + (mostChild[5].deaths == 0 ? 1 : 0)), i, 5); // KDA

		}

		if (mostChild [0].gamesPlayed == 0) {
			statsText.text = "";
			return;
		}
		
		ConstructString ();

	}

	void ConstructString()
	{
		string outString = "";

		for (int i = 0; i < mostChild.Length; i++) {
			if (tied [i])
				outString += "<color=#ffff00ff>";
			if (i==0) outString += "MOST PLAYED : " + mostChild [0].name + " / " + mostChild [0].gamesPlayed + "\n";
			if (i==1) outString += "MOST DUNKS : " + mostChild [1].name + " / " + mostChild [1].kills 		 + "\n";
			if (i==2) outString += "MOST DEATHS : " + mostChild [2].name + " / " + mostChild [2].deaths 	 + "\n";
			if (i==3) outString += "MOST WINS : " + mostChild [3].name + " / " + mostChild [3].wins 		 + "\n";
			if (i==4) outString += "MOST LOSSES : " + mostChild [4].name + " / " + mostChild [4].losses 	 + "\n";
			if (i==5) outString += "BEST KDA : " + mostChild [5].name + " / " + Mathf.Round ((mostChild [5].kills / (mostChild [5].deaths + (mostChild[5].deaths == 0 ? 1 : 0))) * 100f) / 100f;
			if (tied [i])
				outString += "</color>";
		}

		statsText.text = outString;
	}

	void CompareChild(int compareIntA, int compareIntB, int indexA, int indexB)
	{
		List<Child> childrenA = ChildQueueController.child_profiles;
		Child[] childrenB = mostChild;

		if (childrenA [indexA].name == childrenB [indexB].name)
			return;


		if (compareIntA > compareIntB) {
			childrenB [indexB] = childrenA [indexA];
			tied[indexB] = (compareIntA == compareIntB);
		}
	}
}

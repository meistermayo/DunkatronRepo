using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{
	// 
	int heapIndex; //4b
	public Vector3 worldPosition; //4f: 4b 4b 4b 4b
	public int gridX; // 4b
	public int gridY;// 4b
	public int gCost; //4b 
	public int hCost; //4b
	public bool walkable; //1b
	char pad1,pad2,pad3; // 3p

	public Node parent; //? pointer? 4b? reference 32b?

	public Node (bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPosition;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost{
		get {
			return gCost + hCost;
		}
	}

	public int HeapIndex{
		get{ 
			return heapIndex; 
		}
		set{ 
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo (nodeToCompare.fCost);
		if (compare == 0) {
			compare = hCost.CompareTo (nodeToCompare.hCost);
		}
		return -compare;
	}
}

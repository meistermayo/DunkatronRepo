using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class EnemyPathfinding : MonoBehaviour {

	Grid grid;

	public Transform seeker, target;

	public List<Node> targetPath;


	Coroutine pathCoroutine;

	void Awake()
	{
		grid = GetComponent<Grid>();
		StartCoroutine (PathRoutine ());
	}

	void Update()
	{
	}

	IEnumerator PathRoutine()
	{
		while (true) {
			if (target != null)
				targetPath = FindPath(seeker.position, target.position );
			for (int i = 0; i < 15; i++) {
				yield return new WaitForEndOfFrame ();
			}
		}
	}

	List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
	{

		Stopwatch sw = new Stopwatch ();
		sw.Start ();

		Node startNode = grid.NodeFromWorldPoint (startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);


		RaycastHit2D hit = Physics2D.Linecast (startNode.worldPosition,targetNode.worldPosition,8); // Base case -- saves performance
		//Debug.DrawLine(startNode.worldPosition,targetNode.worldPosition,Color.red);

		if (hit == null) {
			List<Node> ShortList = new List<Node> ();
			ShortList.Add (startNode);
			ShortList.Add(targetNode);
			return ShortList;
		}

		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst ();
			closedSet.Add (currentNode);

			if (currentNode == targetNode) {
				sw.Stop ();
				print ("Path Found " + sw.ElapsedMilliseconds + "ms");
				return RetracePath (startNode,targetNode);
			}

			foreach (Node neighbor in grid.GetNeighbors(currentNode)) {
				if (!neighbor.walkable || closedSet.Contains (neighbor))
					continue;

				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode,neighbor);
				if (newMovementCostToNeighbour < neighbor.gCost || !openSet.Contains (neighbor)) {
					neighbor.gCost = newMovementCostToNeighbour;
					neighbor.hCost = GetDistance (neighbor, targetNode);
					neighbor.parent = currentNode;

					if (!openSet.Contains (neighbor))
						openSet.Add (neighbor);

				}
			}
		}
		return null;
	}


	List<Node> RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node> ();
		Node currentNode = endNode;

		while (currentNode != startNode) {
			path.Add (currentNode);
			currentNode = currentNode.parent;
		}	
		path.Reverse();

		grid.path = path;

		return path;
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int dstX = Mathf.Abs (nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs (nodeA.gridY - nodeB.gridY);

		if (dstX > dstY)
			return 14 * dstY + 10 * (dstX - dstY);
		return 14 * dstX + 10 * (dstY - dstX);
	}
}

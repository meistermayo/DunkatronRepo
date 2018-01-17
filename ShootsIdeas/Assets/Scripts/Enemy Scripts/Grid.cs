using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Grid : MonoBehaviour {

	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	static Node[,] grid;

	float nodeDiamater;
	int gridSizeX, gridSizeY;

	void Start()
	{
		nodeDiamater = nodeRadius * 2;
		if (SceneManager.GetActiveScene ().name == "CoopRoom") {
			gridWorldSize = new Vector2 (15,11);
		}
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiamater);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiamater);
		CreateGrid ();
	}

	public int MaxSize{
		get {
			return gridSizeX * gridSizeY;
		}
	}
	void CreateGrid()
	{
		if (grid != null)
			return;
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 worldButtomLeft = /*transform.position*/ - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;
		for (int x = 0; x < gridSizeX; x++)
			for (int y = 0; y < gridSizeY; y++) {
				Vector3 worldPoint = worldButtomLeft + Vector3.right * (x * nodeDiamater + nodeRadius) + Vector3.up * (y * nodeDiamater + nodeRadius);
				bool walkable = !(Physics2D.OverlapCircle (worldPoint, nodeRadius-.1f,unwalkableMask));
				grid [x, y] = new Node (walkable, worldPoint,x,y);
			}
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01 (percentX);
		percentY = Mathf.Clamp01 (percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		return grid [x,y];
	}

	public List<Node> path;

	void OnDrawGizmos()
	{
		Gizmos.DrawWireCube (transform.position,new Vector3 (gridWorldSize.x,gridWorldSize.y,1));

		if (grid != null) {
			foreach (Node n in grid)
			{
				Gizmos.color = n.walkable ? Color.white : Color.red;
				if (path != null) {
					if (path.Contains (n))
						Gizmos.color = Color.green;
				}
				Gizmos.DrawCube (n.worldPosition,Vector3.one * (nodeDiamater-.1f));
			}
		}
	}

	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neightobrs = new List<Node> ();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == y)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neightobrs.Add(grid[checkX,checkY]);
				}
			}
		}

		return neightobrs;
	}
}

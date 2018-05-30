using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNodes : MonoBehaviour 
{
	private Transform			_transform;
	private PolygonCollider2D	_polygonCollider2D;

	private List<Pathfinding.GridNode>	_nodes	= new List<Pathfinding.GridNode>();

	void Awake()
	{
		_transform			= this.transform;
		_polygonCollider2D	= GetComponent<PolygonCollider2D>();
	}

	void Start()
	{
		// collider bound내에 외곽 노드 추출
		if (_polygonCollider2D == null)
		{
			return;
		}

		var nodesInRegion = AstarPath.active.data.gridGraph.GetNodesInRegion(_polygonCollider2D.bounds);

		for (int nodeIndex = 0; nodeIndex < nodesInRegion.Count; ++nodeIndex)
		{
			Pathfinding.GridNode gridNode = (Pathfinding.GridNode)nodesInRegion[nodeIndex];			
			if (gridNode.Walkable == false)
			{
				_nodes.Add(gridNode);
			}
		}
	}

	public List<Pathfinding.GridNode> GetNodes()
	{
		return _nodes;
	}
}

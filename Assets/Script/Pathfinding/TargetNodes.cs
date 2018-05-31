using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetNodes : MonoBehaviour 
{
	private Transform	_transform;
	private Collider2D	_collider2D;

	private List<Pathfinding.GridNode>	_nodes	= new List<Pathfinding.GridNode>();

	void Awake()
	{
		_transform	= this.transform;
		_collider2D	= GetComponent<Collider2D>();
	}

	void Start()
	{
		// collider bound내에 외곽 노드 추출
		if (_collider2D == null)
		{
			return;
		}

		var nodesInRegion = AstarPath.active.data.gridGraph.GetNodesInRegion(_collider2D.bounds);

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

	public Pathfinding.GraphNode GetClosestNode(Vector3 position)
	{
		Pathfinding.GraphNode closestNode	= null;
		float minDistance		= float.MaxValue;

		for (int nodeIndex = 0; nodeIndex < _nodes.Count; ++nodeIndex)
		{
			Pathfinding.GraphNode node	= _nodes[nodeIndex];
			float distance				= Vector3.Distance((Vector3)node.position, position);
			if (minDistance > distance)
			{
				closestNode	= node;
				minDistance	= distance;
			}
		}

		return closestNode;
	}
}

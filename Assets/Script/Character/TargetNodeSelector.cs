using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TargetNodeSelector : MonoBehaviour 
{
	Transform	_transform;
	AIPath	 	_aiPath;

	Transform	_targetTransform;

	void Start()
	{
		_transform	= this.transform;
		if (_aiPath == null)
		{
			_aiPath	= GetComponent<AIPath>();
		}

		selectTargetNode();
	}

	
	void OnEnable() 
	{
		if (_aiPath == null)
		{
			_aiPath	= GetComponent<AIPath>();
		}
		// Update the destination right before searching for a path as well.
		// This is enough in theory, but this script will also update the destination every
		// frame as the destination is used for debugging and may be used for other things by other
		// scripts as well. So it makes sense that it is up to date every frame.
		if (_aiPath != null)
		{
			_aiPath.onSearchPath += selectTargetNode;
		}
	}

	void OnDisable() 
	{
		if (_aiPath != null)
		{
			_aiPath.onSearchPath -= selectTargetNode;
		}
	}


	void selectTargetTransform()
	{
		_targetTransform		= TargetSelector.Instance.GetClosestTarget(_transform.position);
	}

	void selectTargetNode()
	{
		selectTargetTransform();

		if (_targetTransform == null)
		{
			return;
		}

		TargetNodes targetNodes				= _targetTransform.GetComponent<TargetNodes>();
		Pathfinding.GraphNode targetNode	= targetNodes.GetClosestNode(_transform.position);

		if (targetNode == null)
		{
			return;
		}

		_aiPath.destination	= (Vector3)targetNode.position;
	}
}

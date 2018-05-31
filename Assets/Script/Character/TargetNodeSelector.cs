using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class TargetNodeSelector : MonoBehaviour 
{
	Transform	_transform;
	IAstarAI 	_astarAI;

	void Start()
	{
		_transform	= this.transform;
		_astarAI	= GetComponent<IAstarAI>();

		selectTargetNode();
	}

/*	
	void OnEnable() 
	{
		// Update the destination right before searching for a path as well.
		// This is enough in theory, but this script will also update the destination every
		// frame as the destination is used for debugging and may be used for other things by other
		// scripts as well. So it makes sense that it is up to date every frame.
		if (_astarAI != null)
		{
			_astarAI.onSearchPath += selectTargetNode;
		}
	}

	void OnDisable() 
	{
		if (_astarAI != null)
		{
			_astarAI.onSearchPath -= selectTargetNode;
		}
	}
*/

	void selectTargetNode()
	{
		_astarAI.destination	= TargetSelector.Instance.GetClosestTarget(_transform.position).position;
	}
}

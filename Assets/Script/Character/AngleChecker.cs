using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleChecker : MonoBehaviour 
{
	public Vector3		forwardVector;
	public float		angle;
	public float		distance;

	Transform	_transform;

	void Awake()
	{
		_transform	= this.transform;
	}

	public bool WithinRange(Vector3 position)
	{
		Vector3 delta	= position - _transform.position;
		if (delta.magnitude	> distance)
		{
			Debug.Log("Out of distance");

			return false;
		}

		if (Vector3.Dot(forwardVector, delta.normalized) < Mathf.Cos(angle * Mathf.Deg2Rad * 0.5f))
		{
			Debug.Log("Out of angle");

			return false;
		}

		Debug.Log("Within Range");

		return true;
	}
}

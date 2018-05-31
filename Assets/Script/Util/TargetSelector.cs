using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : Util.GlobalBase<TargetSelector>
{
	public Transform		transformCar;
	public Transform		transformBarricade;

	[HideInInspector]
	public List<Transform>	listTarget;

	void Awake()
	{
		if (transformCar == null)
		{
			Debug.LogWarning("Failed to find car");

			return;
		}

		for (int childIndex = 0; childIndex < transformCar.childCount; ++childIndex)
		{
			listTarget.Add(transformCar.GetChild(childIndex));
		}

		if (transformBarricade == null)
		{
			return;
		}

		for (int childIndex = 0; childIndex < transformBarricade.childCount; ++childIndex)
		{
			listTarget.Add(transformBarricade.GetChild(childIndex));
		}
	}

	public Transform GetClosestTarget(Vector3 position)
	{
		Transform closestSeat	= null;
		float minDistance		= float.MaxValue;

		for (int seatIndex = 0; seatIndex < listTarget.Count; ++seatIndex)
		{
			Transform seat = listTarget[seatIndex];
			float distance = Vector3.Distance(seat.position, position);
			if (minDistance > distance)
			{
				closestSeat	= seat;
				minDistance	= distance;
			}
		}

		return closestSeat;
	}
}

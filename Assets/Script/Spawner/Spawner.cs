using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour 
{
	public float		radius;
	public float		delay;
	public int 			remainSpawnCount;
	public float		interval;
	public int			spawnCount;

	public GameObject	enemyPrefab;
	public Transform	enemyGroupTransform;
	
	private int			_spawnedCount	= 0;
	private Transform	_transform;

	void Awake()
	{
		_transform	= this.transform;
	}
	
	void Start()
	{
		StartCoroutine(spawnCoroutine());
	}

	IEnumerator	spawnCoroutine()
	{
		if (delay > 0)
		{
			yield return new WaitForSeconds(delay);
		}

		float elapsedTime	= interval;
		while (remainSpawnCount < 0 || remainSpawnCount > _spawnedCount)
		{
			if (elapsedTime <= 0)
			{
				for (int spawnedIndex = 0; spawnedIndex < spawnCount; ++spawnedIndex)
				{
					Vector2		spawnPosition	= new Vector2(_transform.position.x, _transform.position.y) + Random.insideUnitCircle * radius;
					GameObject	enemyObject		= Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, enemyGroupTransform);

					++_spawnedCount;

					if (_spawnedCount >= remainSpawnCount)
					{
						yield break;
					}
				}

				elapsedTime = interval;
			}
			else
			{
				elapsedTime -= Time.deltaTime;
			}

			yield return null;
		}
	}
}

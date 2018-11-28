using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

	[SerializeField] private GameObject enemyPrefab;
	[SerializeField] private float minRespawnTime = 60;
	[SerializeField] private float maxRespawnTime = 90;
	[SerializeField] private bool patrollingEnemy = false;
	[SerializeField] private float minPatrolWaitTime = 2f;
	[SerializeField] private float maxPatrolWaitTime = 7f;

	private WaypointContainer patrolPath;
	private GameObject spawnedEnemy;


	private void Start()
	{
		patrolPath = GetComponentInChildren<WaypointContainer>();

		if (GetComponentInChildren<CharacterStats>())
		{
			CharacterStats enemy = GetComponentInChildren<CharacterStats>();
			enemy.OnDead += StartSpawnCoroutine;
			spawnedEnemy = enemy.gameObject;

			if (patrollingEnemy)
			{
				Destroy(spawnedEnemy.GetComponent<SearchingState>());
				NPCController npcController = spawnedEnemy.GetComponent<NPCController>();

				float rand = Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
				npcController.SetPatrolWaitTime(rand);
				npcController.SetPatrolPath(patrolPath);

				spawnedEnemy.AddComponent<PatrolState>();
				npcController.UpdateStateComponents();
			}

			spawnedEnemy.transform.parent = null;
		}
		else
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		if (spawnedEnemy == null)
		{
			spawnedEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
			spawnedEnemy.GetComponent<CharacterStats>().OnDead += StartSpawnCoroutine;

			if (patrollingEnemy)
			{
				Destroy(spawnedEnemy.GetComponent<SearchingState>());
				NPCController npcController = spawnedEnemy.GetComponent<NPCController>();

				float rand = Random.Range(minPatrolWaitTime, maxPatrolWaitTime);
				npcController.SetPatrolWaitTime(rand);
				npcController.SetPatrolPath(patrolPath);

				spawnedEnemy.AddComponent<PatrolState>();
				npcController.UpdateStateComponents();
			}
		}
		else
		{
			StartCoroutine(WaitForSpawn());
		}
	}

	private void StartSpawnCoroutine()
	{
		spawnedEnemy.GetComponent<CharacterStats>().OnDead -= StartSpawnCoroutine;
		StartCoroutine(WaitForSpawn());
	}

	private IEnumerator WaitForSpawn()
	{
		float respawnTime = Random.Range(minRespawnTime, maxRespawnTime);

		yield return new WaitForSeconds(respawnTime);

		Spawn();
	}
}

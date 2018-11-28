using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingNodeSpawner : MonoBehaviour {


	[SerializeField] private GameObject craftingNodePrefab;
	[SerializeField] private float minRespawnTime = 60;
	[SerializeField] private float maxRespawnTime = 90;

	private GameObject spawnedNode;


	private void Start()
	{
		if (GetComponentInChildren<LootableObject>())
		{
			LootableObject node = GetComponentInChildren<LootableObject>();
			node.OnFinishedLooting += StartSpawnCoroutine;
			spawnedNode = node.gameObject;
		}
		else
		{
			Spawn();
		}
	}

	private void Spawn()
	{
		if (spawnedNode == null)
		{
			spawnedNode = Instantiate(craftingNodePrefab, transform.position, Quaternion.identity, transform);
			spawnedNode.GetComponent<LootableObject>().OnFinishedLooting += StartSpawnCoroutine;

		}
		else
		{
			StartCoroutine(WaitForSpawn());
		}
	}

	private void StartSpawnCoroutine()
	{
		spawnedNode.GetComponent<LootableObject>().OnFinishedLooting -= StartSpawnCoroutine;
		StartCoroutine(WaitForSpawn());
	}

	private IEnumerator WaitForSpawn()
	{
		float respawnTime = Random.Range(minRespawnTime, maxRespawnTime);

		yield return new WaitForSeconds(respawnTime);

		Spawn();
	}
}

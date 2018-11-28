using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectPoolItem
{
	public int pooledAmount;
	public GameObject objectToPool;
	public bool willGrow;
}


public class ObjectPooler : MonoBehaviour {

	//Singleton pattern
	public static ObjectPooler Instance;

	[SerializeField] private GameObject lootBag;
	[SerializeField] private List<ObjectPoolItem> itemsToPool;

	private Dictionary<int, List<GameObject>> pooledObjects = new Dictionary<int, List<GameObject>>();


	//Singleton
	private ObjectPooler() { }

	private void Awake()
	{
		//enforce singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		PoolObjects();
	}

	private void PoolObjects()
	{
		foreach (ObjectPoolItem item in itemsToPool)
		{
			int objId = item.objectToPool.GetInstanceID();
			if (!pooledObjects.ContainsKey(objId))
			{
				pooledObjects.Add(objId, new List<GameObject>());
			}

			GameObject obj;
			for (int i = 0; i < item.pooledAmount; i++)
			{
				obj = Instantiate(item.objectToPool, transform.position, transform.rotation);
				obj.SetActive(false);
				pooledObjects[objId].Add(obj);
			}
		}
	}

	private GameObject GetPooledObject(GameObject sourceObj)
	{
		int objId = sourceObj.GetInstanceID();

		if (!pooledObjects.ContainsKey(objId))
		{
			Debug.Log(sourceObj.name + " not found in object pool!");
			return null;
		}

		List<GameObject> itemPool = pooledObjects[objId];

		for(int i = 0; i < itemPool.Count; i++)
		{
			if (!itemPool[i].activeInHierarchy)
			{
				return itemPool[i];
			}
		}

		foreach(ObjectPoolItem item in itemsToPool)
		{
			int newObjId = item.objectToPool.GetInstanceID();
			if(newObjId == objId && item.willGrow)
			{
				GameObject obj = Instantiate(item.objectToPool, transform.position, transform.rotation);
				obj.SetActive(false);
				pooledObjects[objId].Add(obj);
				Debug.Log("Added " + item.objectToPool.name + " to the object pool!");
				return obj;
			}
		}

		return null;
	}

	public GameObject ActivatePooledObject(GameObject objectToActivate, Vector3 position, Quaternion rotation, float duration = 0, Transform parent = null)
	{
		GameObject newObject = GetPooledObject(objectToActivate);
		newObject.transform.position = position;
		newObject.transform.rotation = rotation;

		if(duration != 0)
		{
			try
			{
				newObject.GetComponent<ParticleSystemScript>().Lifetime = duration;
			}
			catch
			{
				Debug.Log("No particle system script attached to " + newObject.name);
			}
		}

		if(parent != null)
		{
			newObject.transform.parent = parent;
		}

		newObject.SetActive(true);

		return newObject;
	}

	public GameObject GetLootBag()
	{
		return lootBag;
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemActivator : MonoBehaviour
{

	public static ItemActivator Instance;

	public List<GameObject> addList = new List<GameObject>();
	public List<GameObject> externalRemoveList = new List<GameObject>();

	[SerializeField] private int distanceFromPlayer;

	private GameObject player;
	private List<GameObject> activatorItems = new List<GameObject>();
	private List<GameObject> removeList = new List<GameObject>();

	private ItemActivator() { }

	private void Awake()
	{
		//enforce singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
	}

	private void Start()
	{
		player = PlayerManager.Instance.gameObject;

		AddToList();
	}

	private void AddToList()
	{
		if (addList.Count > 0)
		{
			foreach (GameObject item in addList)
			{
				if (item != null)
				{
					activatorItems.Add(item);
				}
			}

			addList.Clear();
		}

		ExternalRemoveFromList();

		StartCoroutine(CheckActivation());
	}

	private void ExternalRemoveFromList()
	{
		if(externalRemoveList.Count > 0)
		{
			foreach(GameObject item in externalRemoveList)
			{
				if(item != null && activatorItems.Contains(item))
				{
					activatorItems.Remove(item);
				}
			}

			externalRemoveList.Clear();
		}
	}

	private IEnumerator CheckActivation()
	{
		if (activatorItems.Count > 0)
		{
			foreach (GameObject item in activatorItems)
			{
				if (item == null)
				{
					removeList.Add(item);
					continue;
				}

				if (Vector3.Distance(player.transform.position, item.transform.position) > distanceFromPlayer)
				{
					item.SetActive(false);
				}
				else
				{
					item.SetActive(true);
				}
				
				yield return new WaitForSeconds(0.01f);
			}
		}

		yield return new WaitForSeconds(0.01f);

		if (removeList.Count > 0)
		{
			foreach (GameObject item in removeList)
			{
				activatorItems.Remove(item);
			}
		}
		removeList.Clear();

		yield return new WaitForSeconds(0.01f);

		AddToList();
	}
}
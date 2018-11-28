using UnityEngine;
using System.Collections;

public class DisableIfFarAway : MonoBehaviour
{

	private void Start()
	{
		AddToList();
	}

	public void AddToList()
	{
		ItemActivator.Instance.addList.Add(gameObject);
	}

	public void RemoveFromList()
	{
		ItemActivator.Instance.externalRemoveList.Add(gameObject);
	}
}
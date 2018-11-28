using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateChildOnEnable : MonoBehaviour {

	public GameObject[] itemsToActivate;

	private void OnEnable()
	{
		foreach(GameObject item in itemsToActivate)
		{
			item.SetActive(true);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendorUI : MonoBehaviour {

	private InventorySlot[] slots;


	private void Start()
	{
		slots = GetComponentsInChildren<InventorySlot>(true);
		gameObject.SetActive(false);
	}


	public void LoadItems(List<Item> items) 
	{
		CloseVendorPanel(); //Reset panel just in case.
		gameObject.SetActive(true);
		UIManager.Instance.PlayOpenMenuSound();

		for (int i = 0; i < items.Count; i++)
		{
			if (slots[i] != null)
			{
				slots[i].AddItem(items[i], 1);
			}
		}
	}

	public void CloseVendorPanel()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			slots[i].ClearSlot();
		}
		UIManager.Instance.PlayCloseMenuSound();
		gameObject.SetActive(false);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootPanelScript : MonoBehaviour {

	private InventorySlot[] slots;
	private LootableObject lootedObject;
	private int filledSlots = 0;

	private void Start()
	{
		slots = GetComponentsInChildren<InventorySlot>(true);
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			LootAll();
		}
	}

	public void LootAll()
	{
		for (int i = 0; i < filledSlots; i++)
		{
			slots[i].LootItem();
		}
		CloseLootPanel();
	}

	public void LoadItems(List<Item> items, Dictionary<Item, int> stackedItems, LootableObject objectLooted) //items is passed by reference from the looted object. Clearing it here will clear it in the lootable object class too.
	{
		CloseLootPanel(); //Reset loot panel just in case.
		UIManager.Instance.PlayOpenMenuSound();
		UIManager.Instance.HideToolTip();
		gameObject.SetActive(true);
		lootedObject = objectLooted;

		for(int i = 0; i < items.Count; i++)
		{
			if(slots[i] != null)
			{
				if(items[i] is Gold)
				{
					items[i].sellValue = objectLooted.GetGoldAmount();
				}

				if(items[i].stackable && stackedItems.ContainsKey(items[i]))
				{
					slots[i].AddItem(items[i], stackedItems[items[i]]);
				}
				else
				{
					slots[i].AddItem(items[i], 1);
				}
				
			}
			filledSlots = 1 + i;
		}
		items.Clear();
	}

	public void CloseLootPanel()
	{
		if(lootedObject == null)
		{
			for (int i = 0; i < slots.Length; i++)
			{
				slots[i].ClearSlot();
			}
			gameObject.SetActive(false);
			return;
		}

		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i].HasItem())
			{
				lootedObject.AddLoot(slots[i].GetItem());
			}
			slots[i].ClearSlot();
		}

		if(lootedObject.destroyOnLooted && lootedObject.GetNumItemsInLoot() <= 0)
		{
			lootedObject.DestroyLootedObject();
		}

		UIManager.Instance.PlayCloseMenuSound();
		lootedObject = null;
		gameObject.SetActive(false);
	}
}

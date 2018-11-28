using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendor : Interactable {

	public List<Item> vendorInventory = new List<Item>();

	private VendorUI vendorUI;


	protected override void Start()
	{
		base.Start();
		vendorUI = UIManager.Instance.GetVendorPanel().GetComponent<VendorUI>();
	}

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);

		List<Item> itemsToLoad = new List<Item>();
		foreach(Item item in vendorInventory)
		{
			if (item is QuestItem)
			{
				List<QuestData> quests = (item as QuestItem).objectiveForList;
				QuestManager questManager = QuestManager.Instance;

				foreach (QuestData quest in quests)
				{
					if (questManager.RequestAcceptedQuest(quest.id))
					{
						itemsToLoad.Add(item);
						break;
					}
				}
			}
			else
			{
				itemsToLoad.Add(item);
			}
		}
	
		vendorUI.LoadItems(itemsToLoad);
	}
}

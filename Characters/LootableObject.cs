using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LootableObject : Interactable {

	public delegate void FinishedLooting();
	public event FinishedLooting OnFinishedLooting;

	public bool destroyOnLooted = false;

	private List<Item> loot = new List<Item>();
	private Dictionary<Item, int> stackedItems = new Dictionary<Item, int>();
	private List<QuestItem> questItems = new List<QuestItem>();
	private int goldAmount;
	private LootPanelScript lootPanelScript;


	protected override void Start()
	{
		base.Start();
		lootPanelScript = UIManager.Instance.GetLootPanelScript();
	}

	public void AddLoot(Item item)
	{
		if(item is QuestItem)
		{
			questItems.Add(item as QuestItem);
			return;
		}

		loot.Add(item);
	}

	public int GetNumItemsInLoot()
	{
		return loot.Count;
	}

	public void SetGoldAmount(int amount)
	{
		goldAmount = amount;
	}

	public int GetGoldAmount()
	{
		return goldAmount;
	}

	public void SetLootList(List<Item> lootList)
	{
		foreach(Item item in lootList)
		{
			if(item is QuestItem)
			{
				questItems.Add(item as QuestItem);
			}
		}

		foreach(QuestItem questItem in questItems)
		{
			lootList.Remove(questItem);
		}

		loot = lootList;
	}

	public void SetStackedItems(Dictionary<Item, int> stackedItemsDictionary)
	{
		stackedItems = stackedItemsDictionary;
	}

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);

		if(questItems.Count > 0)
		{
			QuestManager questManager = QuestManager.Instance;
			
			for (int i = questItems.Count - 1; i >= 0; i--)
			{
				List<QuestData> quests = questItems[i].objectiveForList;

				foreach (QuestData quest in quests)
				{
					if (questManager.RequestAcceptedQuest(quest.id))
					{
						loot.Add(questItems[i]);
						questItems.RemoveAt(i);
						break;
					}
					else
					{
						if (loot.Contains(questItems[i]))
						{
							loot.Remove(questItems[i]);
						}
					}
				}
			}
		}

		lootPanelScript.LoadItems(loot, stackedItems, this);
	}

	public void DestroyLootedObject()
	{
		if (destroyOnLooted)
		{
			if(loot.Count > 0 || questItems.Count > 0)
			{
				return;
			}

			if(OnFinishedLooting != null)
			{
				OnFinishedLooting.Invoke();
			}

			Destroy(gameObject);
		}
	}
}

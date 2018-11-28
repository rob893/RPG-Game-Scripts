using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootScript : MonoBehaviour {

	public List<Loot> lootTable = new List<Loot>();
	public List<Loot> guarenteedDrop = new List<Loot>();
	
	public int maxPossibleRandomDrops = 3;
	public int dropChance;
	public int minGoldDrop = 0;
	public int maxGoldDrop = 0;

	private GameObject droppedLootBag;
	private Gold goldObject;
	private int goldAmount = 0;
	private List<Item> itemList = new List<Item>();
	private Dictionary<Item, int> stackedItems = new Dictionary<Item, int>();


	private void Start()
	{
		goldObject = GameManager.Instance.GetItemFromDatabase(0) as Gold;
		droppedLootBag = ObjectPooler.Instance.GetLootBag();

		if(maxGoldDrop > 0)
		{
			goldAmount = Random.Range(minGoldDrop, maxGoldDrop + 1);
		}

		if (GetComponent<LootableObject>())
		{
			GenerateItemList();
			LootableObject lootableObject = GetComponent<LootableObject>();
			lootableObject.SetLootList(itemList);
			lootableObject.SetStackedItems(stackedItems);
			if (goldAmount > 0)
			{
				GetComponent<LootableObject>().SetGoldAmount(goldAmount);
			}
		}
	}

	private Loot SpawnLoot()
	{
		int calcDropChance = Random.Range(0, 100);

		if(calcDropChance > dropChance)
		{
			return null;
		}

		int itemWeight = 0;

		for(int i = 0; i < lootTable.Count; i++)
		{
			itemWeight += lootTable[i].dropRarity;
		}

		int randomValue = Random.Range(0, itemWeight);

		for(int i = 0; i < lootTable.Count; i++)
		{
			if(randomValue <= lootTable[i].dropRarity)
			{
				lootTable[i].GenerateStackSize();
				return lootTable[i];
			}
			randomValue -= lootTable[i].dropRarity;
		}
		return null;
	}

	private void GenerateItemList()
	{
		itemList.Clear();
		stackedItems.Clear();

		if (goldAmount > 0)
		{
			itemList.Add(goldObject);
		}

		foreach (Loot newItem in guarenteedDrop)
		{
			newItem.GenerateStackSize();
			if (newItem.GetStackSize() > 1)
			{
				if (stackedItems.ContainsKey(newItem.item))
				{
					stackedItems[newItem.item] += newItem.GetStackSize();
					continue;
				}
				else
				{
					stackedItems.Add(newItem.item, newItem.GetStackSize());
				}
			}
			itemList.Add(newItem.item);
		}

		for(int i = 0; i < maxPossibleRandomDrops; i++)
		{
			Loot newItem = SpawnLoot();
			if(newItem != null)
			{
				if(newItem.GetStackSize() > 1)
				{
					if (stackedItems.ContainsKey(newItem.item))
					{
						stackedItems[newItem.item] += newItem.GetStackSize();
						continue;
					}
					else
					{
						stackedItems.Add(newItem.item, newItem.GetStackSize());
					}
				}
				itemList.Add(newItem.item);
			}
		}
	}

	public int GetGoldAmount()
	{
		return goldAmount;
	}

	public void DropLootBag()
	{
		if(droppedLootBag == null)
		{
			Debug.Log("No lootbag found! Add one in the inspector!");
			return;
		}

		GenerateItemList();

		if(itemList.Count <= 0)
		{
			return;
		}
		
		GameObject lootBag = Instantiate(droppedLootBag, new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), Quaternion.identity);
		LootableObject lootableObject = lootBag.GetComponent<LootableObject>();
		lootableObject.SetLootList(itemList);
		lootableObject.SetStackedItems(stackedItems);

		if (goldAmount > 0)
		{
			lootableObject.SetGoldAmount(goldAmount);
		}
	}
}

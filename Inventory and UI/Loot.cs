using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loot
{
	public Item item;
	public int dropRarity;
	public bool isStack;
	public int minStackSize;
	public int maxStackSize;

	private int stackSize = 1;

	public int GetStackSize()
	{
		return stackSize;
	}

	public void GenerateStackSize()
	{
		if (isStack && !item.stackable)
		{
			Debug.Log(item.itemName + " is set to a stack in the loot script but the item is not flagged as stackable! Setting stack size to 1.");
			stackSize = 1;
			return;
		}

		if (isStack && item.stackable)
		{
			stackSize = Random.Range(minStackSize, maxStackSize + 1);
		}
		else
		{
			stackSize = 1;
		}
	}
}

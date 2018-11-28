using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject {

	public string itemName = "New Item";
	[TextArea] public string flavorText = "";
	public Sprite icon = null;
	public int itemId;
	public ItemRarity rarity;
	public int requiredLevel = 0;
	public int sellValue;
	public bool stackable;
	public bool craftable = false;
	public int craftingSkillRequired = 1;
	public List<CraftingRequirement> craftingRequirements = new List<CraftingRequirement>();


	public virtual bool Use()
	{
		if (PlayerManager.Instance.playerStats.GetLevel() < requiredLevel && !Input.GetKey(KeyCode.LeftShift))
		{
			UIManager.Instance.ShowMessage("You are not high enough level to use this item!");
			return false;
		}
		return true;
	}

	public void RemoveFromInventory()
	{
		Inventory.Instance.Remove(this);
	}

	public void CraftItem()
	{
		if (!craftable)
		{
			return;
		}

		if (Input.GetKey(KeyCode.LeftShift)) //for testing
		{
			PlayerManager.Instance.playerStats.AddCraftingSkillLevel();
			Inventory.Instance.Add(this);
			return;
		}

		if(PlayerManager.Instance.playerStats.GetCraftingSkillLevel() < craftingSkillRequired)
		{
			UIManager.Instance.ShowMessage("You don't have a high enough crafting skill to make this item!");
			return;
		}

		if (!Inventory.Instance.HasEnoughRoomInInventory())
		{
			UIManager.Instance.ShowMessage("You don't have enough room in your inventory to craft this item!");
			return;
		}

		foreach(CraftingRequirement craftingRequirement in craftingRequirements)
		{
			if(!Inventory.Instance.HasEnoughOfItem(craftingRequirement.material, craftingRequirement.numMaterialRequired))
			{
				UIManager.Instance.ShowMessage("You don't have the required materials to craft this item!");
				return;
			}
		}

		foreach(CraftingRequirement craftingRequirement in craftingRequirements)
		{
			Inventory.Instance.UseStackedItem(craftingRequirement.material, craftingRequirement.numMaterialRequired);
		}

		PlayerManager.Instance.playerStats.AddCraftingSkillLevel();
		Inventory.Instance.Add(this);
	}

	//This is the template method. It shows the order of functions, but allows subclasses to define the functions.
	public string GetToolTip()
	{
		string toolTip = "<b><color=" + Utility.GetRarityColor(rarity) + ">" + itemName + "</color></b><size=12>";

		toolTip += GetEquipmentSlot();
		toolTip += GetWeaponDamage();
		toolTip += GetStatModifiers();
		
		if (PlayerManager.Instance.playerStats.GetLevel() < requiredLevel)
		{
			toolTip += "\n<color=red>Requires level " + requiredLevel + "</color>";
		}
		else if(requiredLevel > 0)
		{
			toolTip += "\n<color=white>Requires level " + requiredLevel + "</color>";
		}

		toolTip += GetSetBonusInfo();
		toolTip += GetDescription();

		toolTip += GetValue();

		return toolTip;
	}

	protected virtual string GetEquipmentSlot()
	{
		return null;
	}

	protected virtual string GetWeaponDamage()
	{
		return null;
	}

	protected virtual string GetSetBonusInfo()
	{
		return null;
	}

	protected virtual string GetDescription()
	{
		if(flavorText != null && flavorText.Length > 0)
		{
			return "\n\n<color=yellow>" + flavorText + "</color>";
		}

		return null;
	}

	protected virtual string GetStatModifiers()
	{
		return null;
	}

	protected virtual string GetValue()
	{
		return "\nSell Value: " + sellValue;
	}

	public void SetItemId()
	{
		Dictionary<int, Item> itemDatabase = new Dictionary<int, Item>();
		foreach (Item item in Resources.LoadAll<Item>("Items"))
		{
			if (item != this && !itemDatabase.ContainsKey(item.itemId))
			{
				itemDatabase.Add(item.itemId, item);
			}
		}

		if (!itemDatabase.ContainsKey(itemId))
		{
			Debug.Log("This item has a unique id already!");
			return;
		}

		int i = 1;
		while (itemDatabase.ContainsKey(i) && i < 10000)
		{
			i++;
		}
		Debug.Log("The next available item id is " + i + ". Setting this item's id to " + i);
		itemId = i;
	}
}

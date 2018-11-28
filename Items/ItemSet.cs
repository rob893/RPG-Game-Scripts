using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Set", menuName = "Inventory/Item Set")]
public class ItemSet : ScriptableObject {

	public string setName;
	public List<Equipment> itemsInSet = new List<Equipment>();
	public List<SetBonus> setBonuses = new List<SetBonus>();

	public string GetSetToolTip()
	{
		string toolTip = "";

		toolTip += "\n\n<color=yellow>" + setName + " (" + EquipmentManager.Instance.GetNumSetItemsEquipped(this) + "/" + itemsInSet.Count + ")</color>";


		foreach(Equipment item in itemsInSet)
		{
			string color = Utility.GetHexColor(HexColor.Gray);

			if (EquipmentManager.Instance.HasSetItemEquipped(item))
			{
				color = Utility.GetHexColor(HexColor.Green);
			}

			toolTip += "\n<color=" + color + ">  " + item.itemName + "</color>";
		}

		toolTip += "\n";

		foreach(SetBonus bonus in setBonuses)
		{
			string color = Utility.GetHexColor(HexColor.Gray);

			if (EquipmentManager.Instance.GetNumSetItems(this) >= bonus.numSetItemsReq)
			{
				color = Utility.GetHexColor(HexColor.Green);
			}

			toolTip += "\n<color=" + color + ">" + bonus.GetSetBonusToolTip() + "</color>";
		}

		return toolTip;
	}
}

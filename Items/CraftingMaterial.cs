using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Crafting Material", menuName = "Inventory/Crafting Material")]
public class CraftingMaterial : Item
{
	protected override string GetDescription()
	{
		return "\nCrafting Material";
	}
}

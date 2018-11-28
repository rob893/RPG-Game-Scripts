using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gold", menuName = "Inventory/Gold")]
public class Gold : Item {

	public override bool Use()
	{
		Inventory.Instance.AddGold(sellValue);
		return true;
	}

	protected override string GetValue()
	{
		return "\nAmount: " + sellValue; 
	}
}

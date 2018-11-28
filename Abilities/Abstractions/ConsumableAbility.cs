using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableAbility : Ability {

	public Consumable consumable;

	private Inventory inventory;
	private UIManager uiManager;

	protected override void Start()
	{
		base.Start();
		inventory = Inventory.Instance;
		uiManager = UIManager.Instance;
	}

	protected override void Update()
	{
	
	}

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		consumable = (configToSet as ConsumableAbilityConfig).consumable;
	}

	public override bool Use(CharacterStats targetStats)
	{
		if (inventory.GetCurrentInventory().Contains(consumable))
		{
			if(myStats.HealthAsPercentage == 1)
			{
				uiManager.ShowMessage("You are already at full health!");
			}
			else
			{
				consumable.Use();
				//characterCombat.TriggerGCD();
			}
			
			return true;
		}

		uiManager.ShowMessage("You don't have any " + consumable.itemName + "s!");
		return true;
	}
}

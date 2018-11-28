using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/Consumable")]
public class Consumable : Item {

	public List<EffectConfig> effects;
	public float instantHealAmount = 0;
	[TextArea]
	public string description;

	public override bool Use()
	{
		if (!base.Use())
		{
			return false;
		}

		GameObject player = PlayerManager.Instance.player;
		CharacterStats playerStats = PlayerManager.Instance.playerStats;

		foreach(EffectConfig effect in effects)
		{
			Effect effectInstance = effect.AttachEffectTo(player);
			effectInstance.MyStats = playerStats;
			effectInstance.TheirStats = playerStats;
		}

		if(instantHealAmount > 0)
		{
			playerStats.TakeHeal(instantHealAmount, playerStats);
		}

		if (stackable)
		{
			Inventory.Instance.UseStackedItem(this);
		}
		else
		{
			RemoveFromInventory();
		}
		return true;
	}

	protected override string GetDescription()
	{
		return "\n" + description;
	}
}

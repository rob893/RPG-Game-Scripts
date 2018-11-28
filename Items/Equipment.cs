using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {

	public EquipmentSlot equipSlot;
	public int armorModifier;
	public int attackPowerModifier;
	public int healthModifier;
	public int critChanceModifier;
	public int manaPerSecondModifier;
	public int manaModifier;
	public int percentHealthRegainPerFiveSecondsModifier;
	public bool isSetItem = false;
	public ItemSet setPartOf;

	public override bool Use()
	{
		if (!base.Use())
		{
			return false;
		}

		EquipmentManager.Instance.Equip(this);
		RemoveFromInventory();

		return true;
	}

	//Here, this subclass of item is defining the methods used in the template method in the Item base class.
	protected override string GetEquipmentSlot()
	{
		return "\n" + equipSlot;
	}

	protected override string GetStatModifiers()
	{
		string statMods = "";

		if (armorModifier > 0)
		{
			statMods += "\n" + armorModifier + " Armor";
		}

		if (attackPowerModifier > 0)
		{
			statMods += "\n+" + attackPowerModifier + " Attack Power";
		}

		statMods += "<color=green>";

		if (critChanceModifier > 0)
		{
			statMods += "\n+" + critChanceModifier + " Critical Strike Chance";
		}

		if (healthModifier > 0)
		{
			statMods += "\n+" + healthModifier + " Health";
		}

		if (manaModifier > 0)
		{
			statMods += "\n+" + manaModifier + " Mana";
		}

		if (percentHealthRegainPerFiveSecondsModifier > 0)
		{
			statMods += "\n+" + percentHealthRegainPerFiveSecondsModifier + " Health Regain";
		}

		if (manaPerSecondModifier > 0)
		{
			statMods += "\n+" + manaPerSecondModifier + " Mana Regain";
		}

		statMods += "</color>";
		return statMods;
	}

	protected override string GetSetBonusInfo()
	{
		if (isSetItem)
		{
			return setPartOf.GetSetToolTip();
		}
		return null;
	}

	public void RandomizeStats()
	{
		attackPowerModifier = 0;
		armorModifier = 0;
		healthModifier = 0;
		critChanceModifier = 0;
		manaModifier = 0;
		manaPerSecondModifier = 0;
		percentHealthRegainPerFiveSecondsModifier = 0;

		int statBudget = requiredLevel + (int)rarity;

		sellValue = statBudget * 3;
		
		if(this is Weapon)
		{
			if((this as Weapon).isTwoHanded)
			{
				statBudget *= 2;
			}
			Debug.Log("Randomizing stats with a stat budget of " + statBudget);

			attackPowerModifier = (statBudget / 2) + statBudget % 2;
			statBudget /= 2;

			if (attackPowerModifier <= 0)
			{
				attackPowerModifier = 1;
				statBudget = 0;
			}
		}
		else
		{
			Debug.Log("Randomizing stats with a stat budget of " + statBudget);
			armorModifier = (statBudget / 2) + statBudget % 2;
			statBudget /= 2;

			if(armorModifier <= 0)
			{
				armorModifier = 1;
				statBudget = 0;
			}
		}

		while(statBudget > 0)
		{
			int ranStat = Random.Range(1, 7);
			switch (ranStat)
			{
				case 1:
					if(this is Weapon)
					{
						attackPowerModifier++;
					}
					else
					{
						armorModifier++;
					}
					break;

				case 2:
					healthModifier += 5;
					break;

				case 3:
					critChanceModifier++;
					break;

				case 4:
					manaModifier += 5;
					break;

				case 5:
					manaPerSecondModifier++;
					break;

				case 6:
					percentHealthRegainPerFiveSecondsModifier++;
					break;

				default:
					Debug.Log("Something went wrong.");
					break;
			}

			statBudget--;
		}
	}
}

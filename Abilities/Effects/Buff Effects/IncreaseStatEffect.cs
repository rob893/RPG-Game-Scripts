using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreaseStatEffect : Effect {

	private int amountIncrease;
	private int attackPowerIncrease; //Special due to it being a % increase of current AP
	private bool armor;
	private bool attackPowerPercentIncrease;
	private bool health;
	private bool power;
	private bool critChance;
	private bool powerRegainPerSecond;
	private bool percHealthRegainPerFive;


	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		amountIncrease = (configToSet as IncreaseStatEffectConfig).amountIncrease;
		armor = (configToSet as IncreaseStatEffectConfig).armor;
		attackPowerPercentIncrease = (configToSet as IncreaseStatEffectConfig).attackPowerPercentIncrease;
		health = (configToSet as IncreaseStatEffectConfig).health;
		power = (configToSet as IncreaseStatEffectConfig).power;
		critChance = (configToSet as IncreaseStatEffectConfig).critChance;
		powerRegainPerSecond = (configToSet as IncreaseStatEffectConfig).powerRegainPerSecond;
		percHealthRegainPerFive = (configToSet as IncreaseStatEffectConfig).percHealthRegainPerFive;
	}

	protected override void LogicBeforeDestroy()
	{

	}

	protected override IEnumerator EffectLogic()
	{
		attackPowerIncrease = (int)Mathf.Ceil(TheirStats.attackPower.GetValue() * ((float)amountIncrease / 100));
		
		if (armor)
		{
			TheirStats.armor.AddModifier(amountIncrease);
		}
		if (attackPowerPercentIncrease)
		{
			TheirStats.attackPower.AddModifier(attackPowerIncrease);
		}
		if (health)
		{
			TheirStats.health.AddModifier(amountIncrease);
		}
		if (power)
		{
			TheirStats.mana.AddModifier(amountIncrease);
		}
		if (critChance)
		{
			TheirStats.crit.AddModifier(amountIncrease);
		}
		if (powerRegainPerSecond)
		{
			TheirStats.manaRegainPerSecond.AddModifier(amountIncrease);
		}
		if (percHealthRegainPerFive)
		{
			TheirStats.healthRegainPerFiveSeconds.AddModifier(amountIncrease);
		}

		yield return new WaitForSeconds(duration);

		if (armor)
		{
			TheirStats.armor.RemoveModifier(amountIncrease);
		}
		if (attackPowerPercentIncrease)
		{
			TheirStats.attackPower.RemoveModifier(attackPowerIncrease);
		}
		if (health)
		{
			TheirStats.health.RemoveModifier(amountIncrease);
		}
		if (power)
		{
			TheirStats.mana.RemoveModifier(amountIncrease);
		}
		if (critChance)
		{
			TheirStats.crit.RemoveModifier(amountIncrease);
		}
		if (powerRegainPerSecond)
		{
			TheirStats.manaRegainPerSecond.RemoveModifier(amountIncrease);
		}
		if (percHealthRegainPerFive)
		{
			TheirStats.healthRegainPerFiveSeconds.RemoveModifier(amountIncrease);
		}

		RemoveAndDestroy();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPowerSetEffect : SetBonusEffect {

	private int attackPowerBonus;


	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		attackPowerBonus = (configToSet as AttackPowerSetEffectConfig).attackPowerBonus;
	}

	protected override void SetBonusLogic()
	{
		PlayerManager.Instance.playerStats.attackPower.AddModifier(attackPowerBonus);
	}

	protected override void RemoveAndDestroySetBonusLogic()
	{
		PlayerManager.Instance.playerStats.attackPower.RemoveModifier(attackPowerBonus);
	}
}

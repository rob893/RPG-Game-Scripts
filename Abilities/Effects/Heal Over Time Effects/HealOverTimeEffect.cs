using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOverTimeEffect : Effect {

	public float baseHealPerTick;
	public float tickFreq;


	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		tickFreq = (configToSet as HealOverTimeConfig).tickFrequency;
		baseHealPerTick = (configToSet as HealOverTimeConfig).baseHealPerTick;
	}

	protected override void LogicBeforeDestroy()
	{
		theirActiveEffects[effectName][0].ResetEffectTimer();
	}

	protected override IEnumerator EffectLogic()
	{
		baseHealPerTick = baseHealPerTick / 100;
		while (timer <= duration)
		{
			yield return new WaitForSeconds(tickFreq);
			if (TheirStats.CurrentHealth <= 0)
			{
				break;
			}
			TheirStats.TakeHeal(baseHealPerTick * MyStats.attackPower.GetValue(), MyStats);
			if (effectPrefab != null)
			{
				ObjectPooler.Instance.ActivatePooledObject(effectPrefab, TheirStats.transform.position, TheirStats.transform.rotation, 2, TheirStats.transform);
			}
		}

		RemoveAndDestroy();
	}
}

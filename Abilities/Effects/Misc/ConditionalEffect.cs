using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionalEffect : Effect
{
	public List<EffectConfig> ifHasEffect;
	public List<EffectConfig> effectsToApply;

	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		ifHasEffect = (configToSet as ConditionalEffectConfig).ifHasEffect;
		effectsToApply = (configToSet as ConditionalEffectConfig).effectsToApply;
	}

	protected override IEnumerator EffectLogic()
	{
		foreach(EffectConfig effect in ifHasEffect)
		{
			if(theirActiveEffects.ContainsKey(effect.name))
			{
				foreach(EffectConfig effectToApply in effectsToApply)
				{
					Effect effectInstance = effectToApply.AttachEffectTo(gameObject);
					effectInstance.TheirStats = TheirStats;
					effectInstance.MyStats = MyStats;
				}
				break;
			}
		}

		yield return new WaitForEndOfFrame();

		RemoveAndDestroy();
	}

	protected override void LogicBeforeDestroy()
	{
	
	}
}

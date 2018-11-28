using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedMeleeAbility : TargetedAbility {

	public override void AnimationEvent()
	{
		if(targetedStats != null)
		{
			foreach (EffectConfig effect in effects)
			{
				Effect effectInstance = effect.AttachEffectTo(targetedStats.gameObject);
				effectInstance.MyStats = myStats;
				effectInstance.TheirStats = targetedStats;
			}
		}
	}

	protected override bool PlayVoiceSound()
	{
		if(Random.Range(0, 5) == 0)
		{
			return true;
		}
		return false;
	}
}

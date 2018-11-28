using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEffectToAbilityEffect : Effect {

	public EffectConfig effectToAdd;
	public AbilityConfig abilityToAddTo;


	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectToAdd = (configToSet as AddEffectToAbilityEffectConfig).effectToAdd;
		abilityToAddTo = (configToSet as AddEffectToAbilityEffectConfig).abilityToAddTo;
	}

	protected override void LogicBeforeDestroy()
	{
		
	}

	protected override IEnumerator EffectLogic()
	{
		TargetedMeleeAbility meleeAbility = GetComponent<TargetedMeleeAbility>();
		if(meleeAbility != null)
		{
			meleeAbility.AddEffect(effectToAdd);
			
			yield return new WaitForSeconds(duration);

			meleeAbility.RemoveEffect(effectToAdd);
		
		}
		else
		{
			Debug.Log("no melee abilities found!");
		}
		
		RemoveAndDestroy();
	}
}

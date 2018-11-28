using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Add Effect To Ability"))]
public class AddEffectToAbilityEffectConfig : EffectConfig {

	[Header("Effect to add specific")]
	public EffectConfig effectToAdd;
	public AbilityConfig abilityToAddTo;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<AddEffectToAbilityEffect>();
	}
}

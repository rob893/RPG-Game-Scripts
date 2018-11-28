using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Conditional Effect"))]
public class ConditionalEffectConfig : EffectConfig
{
	[Header("Conditional Effect Specific")]
	public List<EffectConfig> ifHasEffect;
	public List<EffectConfig> effectsToApply;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<ConditionalEffect>();
	}
}

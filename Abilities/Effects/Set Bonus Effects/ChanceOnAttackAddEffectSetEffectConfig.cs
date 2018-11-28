using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Add Effect Set Bonus"))]
public class ChanceOnAttackAddEffectSetEffectConfig : SetBonusEffectConfig {

	[Header("Add Effect Set Bonus Specific")]
	public List<EffectConfig> effectsToAdd;
	public int chanceOnAttack;
	public float buffDuration;
	public float internalCD = 15;


	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<ChanceOnAttackAddEffectSetEffect>();
	}

}

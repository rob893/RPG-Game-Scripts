using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Summon Creature"))]
public class SummonCreatureEffectConfig : EffectConfig {

	//[Header("Summon creature specific")]

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<SummonCreatureEffect>();
	}
}

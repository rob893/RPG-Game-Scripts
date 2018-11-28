using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Stun"))]
public class StunConfig : EffectConfig
{
	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<StunEffect>();
	}
}

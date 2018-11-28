using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Slow"))]
public class SlowConfig : EffectConfig
{
	[Header("Slow Specific")]
	public float slowPercent;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<SlowEffect>();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Damage Over Time"))]
public class DamageOverTimeConfig : EffectConfig
{
	[Header("Damage Over Time Specific")]
	public float tickFrequency;
	public float attackPowerPercent;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<DamageOverTimeEffect>();
	}
}

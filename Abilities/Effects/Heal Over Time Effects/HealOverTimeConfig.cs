using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Heal Over Time"))]
public class HealOverTimeConfig : EffectConfig
{
	[Header("Heal Over Time Specific")]
	public float tickFrequency;
	public float baseHealPerTick;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<HealOverTimeEffect>();
	}
}

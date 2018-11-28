using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Pulsate Damage"))]
public class PulsateDamageEffectConfig : EffectConfig {

	[Header("Pulsate Damage specific")]
	public float attackPowerPercent;
	public float radius;
	public float tickFrequency;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<PulsateDamageEffect>();
	}
}

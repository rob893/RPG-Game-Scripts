using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Attack Power Set Bonus"))]
public class AttackPowerSetEffectConfig : SetBonusEffectConfig {

	[Header("Attack Power Set Bonus Specific")]
	public int attackPowerBonus;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<AttackPowerSetEffect>();
	}
}

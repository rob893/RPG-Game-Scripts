using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Consumable Ability"))]
public class ConsumableAbilityConfig : AbilityConfig
{
	[Header("Consumable Specific")]
	public Consumable consumable;

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<ConsumableAbility>();
	}
}

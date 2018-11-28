using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Targeted Ranged"))]
public class TargetedRangedConfig : AbilityConfig
{
	[Header("Targeted Ranged Specific")]
	public GameObject projectile;
	public float speed;
	

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<TargetedRangedAbility>();
	}
}

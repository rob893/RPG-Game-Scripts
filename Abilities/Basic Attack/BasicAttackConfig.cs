using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Basic Attack"))]
public class BasicAttackConfig : AbilityConfig
{
	//[Header("Basic Attack Specific")]
	

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<MeleeAttack>();
	}
}

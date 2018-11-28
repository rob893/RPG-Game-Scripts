using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Player Basic Attack"))]
public class PlayerBasicAttackConfig : AbilityConfig
{
	//[Header("Basic Attack Specific")]


	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<PlayerBasicAttack>();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Self Cast"))]
public class SelfCastConfig : AbilityConfig
{
	[Header("Self Cast Specific")]
	public GameObject graphic;


	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<SelfCastAbility>();
	}
}

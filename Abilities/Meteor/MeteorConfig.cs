using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Meteor"))]
public class MeteorConfig : AbilityConfig
{
	[Header("Meteor Specific")]
	public GameObject effect;
	public float graphicDuration;
	public float effectRadius;


	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<MeteorAbility>();
	}
}

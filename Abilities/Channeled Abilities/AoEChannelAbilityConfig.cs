using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Channeled AoE"))]
public class AoEChannelAbilityConfig : AbilityConfig {

	[Header("AoE Channeled Specific")]
	public GameObject effect;
	public GameObject tickPrefabGraphic;
	public float effectRadius;
	public float tickFreq;

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<AoEChannelAbility>();
	}
}

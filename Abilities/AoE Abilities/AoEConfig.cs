using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/AoE"))]
public class AoEConfig : AbilityConfig
{
	[Header("AoE Specific")]
	public GameObject initialAoEGraphic;
	public GameObject initialHitGraphic;
	public LayerMask hitLayers;
	public float radius;
	

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<AoEAbility>();
	}
}
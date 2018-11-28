using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Trap"))]
public class TrapAbilityConfig : AbilityConfig {

	[Header("Trap Specific")]
	public GameObject trapGameObject;
	public float trapDuration = 60;
	public float effectRadius = 5;
	public float armTime = 3;
	public LayerMask hitLayers;


	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<TrapAbility>();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/Tornado"))]
public class TornadoConfig : AbilityConfig
{
	[Header("Tornado Specific")]
	public GameObject effect;
	public float abilityDuration;
	public float effectRadius;
	public float pullForce;

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<Tornado>();
	}
}

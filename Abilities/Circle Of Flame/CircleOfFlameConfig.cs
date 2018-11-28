using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO abstract to generic "circular spawned prefab around player"
[CreateAssetMenu(menuName = ("Special Abiltiy/Circle of Flame"))]
public class CircleOfFlameConfig : AbilityConfig
{
	[Header("Circle of Flame Specific")]
	public GameObject effect;
	public GameObject tickGraphic;
	public float tickFrequency;
	public float abilityDuration;
	public float radius;
	public GameObject initialAoEGraphic;

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<CircleOfFlameAbility>();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Special Abiltiy/AoE/Offset Spawned"))]
public class OffSetSpawnedAoEConfig : AoEConfig {

	[Header("OffSet Spawned Specific")]
	public float spawnDistance;
	public float damageDelayTime;

	public override Ability GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<OffSetSpawnedAoE>();
	}
}

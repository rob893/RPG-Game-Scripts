using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Weapon Mesh Effect"))]
public class WeaponMeshEffectConfig : EffectConfig {

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<WeaponMeshEffect>();
	}
}

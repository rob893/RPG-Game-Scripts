using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMeshEffect : Effect {

	protected override IEnumerator EffectLogic()
	{
		TargetedMeleeAbility meleeAbility = GetComponent<TargetedMeleeAbility>();
		if (meleeAbility != null)
		{
			GameObject effectInstance = Instantiate(effectPrefab, EquipmentManager.Instance.GetCurrentWeapon().transform); //object pooling causes too many bugs and does not offer much performance in this situation
			PSMeshRendererUpdater meshEffect = effectInstance.GetComponent<PSMeshRendererUpdater>();
			if (EquipmentManager.Instance.GetCurrentWeapon().GetComponentInChildren<Target>())
			{
				meshEffect.UpdateMeshEffect(EquipmentManager.Instance.GetCurrentWeapon().GetComponentInChildren<Target>().gameObject);
			}
			else
			{
				meshEffect.UpdateMeshEffect(EquipmentManager.Instance.GetCurrentWeapon());
			}


			yield return new WaitForSeconds(duration);

			if (meshEffect != null)
			{
				Destroy(effectInstance);
			}

		}
		else
		{
			Debug.Log("no melee abilities found!");
		}

		RemoveAndDestroy();
	}

	protected override void LogicBeforeDestroy()
	{
		
	}
}

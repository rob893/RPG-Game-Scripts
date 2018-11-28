using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEChannelAbility : ChanneledAbility {

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectRadius = (config as AoEChannelAbilityConfig).effectRadius;
		spawnedEffect = (config as AoEChannelAbilityConfig).effect;
		tickFreq = (config as AoEChannelAbilityConfig).tickFreq;
		tickPrefabGraphic = (config as AoEChannelAbilityConfig).tickPrefabGraphic;
	}

	protected override void AbilityLogic()
	{
		base.AbilityLogic();
		Collider[] colliders = Physics.OverlapSphere(transform.position, effectRadius, 1 << 10);
		foreach (Collider collider in colliders)
		{
			if (collider.GetComponent<CharacterStats>())
			{
				CharacterStats collidedStats = collider.GetComponent<CharacterStats>();

				if (attackPowerPercent > 0)
				{
					collidedStats.TakeDamage(myStats.attackPower.GetValue() * attackPowerPercent, myStats);
					PlayAbilityAudioClip(2);
				}

				if(tickPrefabGraphic != null)
				{
					ObjectPooler.Instance.ActivatePooledObject(tickPrefabGraphic, collidedStats.transform.position + new Vector3(0, 1, 0), Quaternion.identity, tickFreq, collidedStats.transform);
				}

				foreach (EffectConfig effect in effects)
				{
					Effect effectInstance = effect.AttachEffectTo(collidedStats.gameObject);
					effectInstance.MyStats = myStats;
					effectInstance.TheirStats = collidedStats;
				}
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorAbility : ReticalSpawnedAbility {

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectRadius = (config as MeteorConfig).effectRadius;
		spawnedEffect = (config as MeteorConfig).effect;
		effectDuration = (config as MeteorConfig).graphicDuration;
	}

	public override void AnimationEvent()
	{
		base.AnimationEvent();
		GameObject effectInstance = ObjectPooler.Instance.ActivatePooledObject(spawnedEffect, spawnPoint + new Vector3(-6, 5, 0), Quaternion.Euler(-45, -75, 0), effectDuration);
		Spell_Meteor meteor = effectInstance.GetComponent<Spell_Meteor>();
		meteor.abilityReference = this;
	}

	public void OnImpact(Vector3 impactPoint)
	{
		Collider[] colliders = Physics.OverlapSphere(impactPoint, effectRadius, 1 << 10);
		foreach (Collider collider in colliders)
		{
			CharacterStats hitStats = collider.GetComponent<CharacterStats>();
			if (hitStats != null)
			{
				hitStats.TakeDamage(myStats.attackPower.GetValue() * attackPowerPercent, myStats);
				foreach (EffectConfig effect in effects)
				{
					Effect effectInstance = effect.AttachEffectTo(hitStats.gameObject);
					effectInstance.MyStats = myStats;
					effectInstance.TheirStats = hitStats;
				}
			}
		}
	}
}

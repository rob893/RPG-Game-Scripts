using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapAbility : ReticalSpawnedAbility {

	private float armTime;
	private LayerMask hitLayers;


	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectRadius = (config as TrapAbilityConfig).effectRadius;
		spawnedEffect = (config as TrapAbilityConfig).trapGameObject;
		effectDuration = (config as TrapAbilityConfig).trapDuration;
		armTime = (config as TrapAbilityConfig).armTime;
		hitLayers = (config as TrapAbilityConfig).hitLayers;
	}

	public override void AnimationEvent()
	{
		base.AnimationEvent();
		GameObject effectInstance = ObjectPooler.Instance.ActivatePooledObject(spawnedEffect, spawnPoint, Quaternion.Euler(0, 0, 0));
		Trap trap = effectInstance.GetComponent<Trap>();
		effectInstance.GetComponent<ParticleSystemScript>().Lifetime = effectDuration;
		trap.damage = attackPowerPercent * myStats.attackPower.GetValue();
		trap.myStats = myStats;
		trap.damageRadius = effectRadius;
		trap.armTime = armTime;
		trap.effects = effects;
		trap.hitLayers = hitLayers;
	}
}

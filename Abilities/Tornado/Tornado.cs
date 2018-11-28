using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : ReticalSpawnedAbility
{
	public float pullForce;


	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectRadius = (config as TornadoConfig).effectRadius;
		spawnedEffect = (config as TornadoConfig).effect;
		effectDuration = (config as TornadoConfig).abilityDuration;
		pullForce = (config as TornadoConfig).pullForce;
	}

	public override void AnimationEvent()
	{
		base.AnimationEvent();
		GameObject effectInstance = ObjectPooler.Instance.ActivatePooledObject(spawnedEffect, spawnPoint, Quaternion.Euler(0, 0, 0));
		Spell_Tornado tornado = effectInstance.GetComponent<Spell_Tornado>();
		tornado.pullForce = pullForce;
		tornado.spellDamage = attackPowerPercent * myStats.attackPower.GetValue();
		tornado.spellDuration = effectDuration;
		tornado.pullRadius = effectRadius;
		tornado.spellRadius = effectRadius;
	}
}

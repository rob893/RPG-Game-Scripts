using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleOfFlameAbility : AoEAbility {

	public GameObject effect;
	public GameObject tickGraphic;
	public float tickFreq = 1;
	public float duration = 5;

	private Vector3 offset = new Vector3(0, 1, 0);


	public override void SetConfig(AbilityConfig configToSet)
	{
		config = configToSet;
		abilityId = config.abilityId;
		abilityName = config.abilityName;
		icon = config.icon;
		description = config.description;
		abilityAnimationClips = config.abilityAnimationClips;
		initialSound = config.initialSound;
		secondarySound = config.secondarySound;
		abilityRange = config.abilityRange;
		abilityAnimationTime = config.abilityAnimationTime;
		abilityCooldown = config.abilityCooldown;
		effects = config.effects;
		abilityManaCost = config.manaCost;
		attackPowerPercent = config.attackPowerPercent;
		radius = 6;
		effect = (config as CircleOfFlameConfig).effect;
		tickGraphic = (config as CircleOfFlameConfig).tickGraphic;
		tickFreq = (config as CircleOfFlameConfig).tickFrequency;
		duration = (config as CircleOfFlameConfig).abilityDuration;
	}

	public override void AnimationEvent()
	{
		base.AnimationEvent();
		GameObject effectInstance = ObjectPooler.Instance.ActivatePooledObject(effect, transform.position, transform.rotation, duration);
		Spell_CircleOfFlame circleOfFlame = effectInstance.GetComponent<Spell_CircleOfFlame>();
		circleOfFlame.abilityReference = this;
		circleOfFlame.tickFreq = tickFreq;
	}

	public void Tick(Vector3 position)
	{
		Collider[] colliders = Physics.OverlapSphere(position, radius, 1 << 10);
		foreach (Collider collider in colliders)
		{
			CharacterStats hitStats = collider.GetComponent<CharacterStats>();
			if (hitStats != null)
			{
				foreach (EffectConfig effect in effects)
				{
					Effect effectInstance = effect.AttachEffectTo(hitStats.gameObject);
					effectInstance.MyStats = myStats;
					effectInstance.TheirStats = hitStats;
				}
				ObjectPooler.Instance.ActivatePooledObject(tickGraphic, hitStats.transform.position + offset, transform.rotation, tickFreq + 0.5f, hitStats.transform);
				hitStats.TakeDamage(myStats.attackPower.GetValue() * attackPowerPercent, myStats);
			}
		}
	}
}

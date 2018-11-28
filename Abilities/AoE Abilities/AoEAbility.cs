using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoEAbility : Ability
{
	protected float radius;
	protected GameObject initialAoEGraphic;
	protected GameObject initialHitGraphic;
	protected LayerMask hitLayers;
	

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		radius = (config as AoEConfig).radius;
		hitLayers = (config as AoEConfig).hitLayers;
		initialAoEGraphic = (config as AoEConfig).initialAoEGraphic;
		initialHitGraphic = (config as AoEConfig).initialHitGraphic;
	}

	public override void AnimationEvent()
	{
		Vector3 spawnPosition = transform.position;

		if (initialAoEGraphic != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(initialAoEGraphic, spawnPosition, transform.rotation, 7);
		}

		Collider[] colliders = Physics.OverlapSphere(spawnPosition, radius, hitLayers);
		foreach(Collider collider in colliders)
		{
			if (collider.GetComponent<CharacterStats>())
			{
				CharacterStats collidedStats = collider.GetComponent<CharacterStats>();

				if(initialHitGraphic != null)
				{
					ObjectPooler.Instance.ActivatePooledObject(initialHitGraphic, collidedStats.transform.position, transform.rotation, 3);
				}

				if(attackPowerPercent > 0)
				{
					collidedStats.TakeDamage(myStats.attackPower.GetValue() * attackPowerPercent, myStats);
					PlayAbilityAudioClip(2);
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

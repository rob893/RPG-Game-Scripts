using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffSetSpawnedAoE : AoEAbility {

	protected float spawnDistance;
	protected float damageDelayTime;


	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		spawnDistance = (config as OffSetSpawnedAoEConfig).spawnDistance;
		damageDelayTime = (config as OffSetSpawnedAoEConfig).damageDelayTime;
	}

	public override void AnimationEvent()
	{
		Vector3 spawnPosition = transform.position;		
		spawnPosition = spawnPosition + transform.forward * spawnDistance;

		ObjectPooler.Instance.ActivatePooledObject(initialAoEGraphic, spawnPosition, transform.rotation, 7);
		StartCoroutine(DamageDelay(spawnPosition, damageDelayTime));
	}

	private IEnumerator DamageDelay(Vector3 spawnPoint, float time)
	{
		yield return new WaitForSeconds(time);

		Collider[] colliders = Physics.OverlapSphere(spawnPoint, radius, hitLayers);
		foreach (Collider collider in colliders)
		{
			if (collider.GetComponent<CharacterStats>())
			{
				CharacterStats collidedStats = collider.GetComponent<CharacterStats>();

				if (initialHitGraphic != null)
				{
					ObjectPooler.Instance.ActivatePooledObject(initialHitGraphic, collidedStats.transform.position, transform.rotation, 3);
				}

				if (attackPowerPercent > 0)
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

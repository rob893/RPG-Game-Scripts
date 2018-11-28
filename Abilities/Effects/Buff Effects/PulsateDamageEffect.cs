using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsateDamageEffect : Effect {

	public float attackPowerPercent = 1;
	public float tickFreq;
	public float radius;

	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		tickFreq = (configToSet as PulsateDamageEffectConfig).tickFrequency;
		attackPowerPercent = (configToSet as PulsateDamageEffectConfig).attackPowerPercent;
		radius = (configToSet as PulsateDamageEffectConfig).radius;
	}

	protected override void LogicBeforeDestroy()
	{
		
	}

	protected override IEnumerator EffectLogic()
	{
		attackPowerPercent = attackPowerPercent / 100;

		while (timer <= duration)
		{
			yield return new WaitForSeconds(tickFreq);

			if (MyStats == null || TheirStats.IsDead || MyStats.IsDead)
			{
				break;
			}
			else
			{
				Collider[] colliders = Physics.OverlapSphere(transform.position, radius, 1 << 10);
				foreach (Collider collider in colliders)
				{
					if (collider.GetComponent<CharacterStats>())
					{
						CharacterStats collidedStats = collider.GetComponent<CharacterStats>();

						if (attackPowerPercent > 0)
						{
							collidedStats.TakeDamage(attackPowerPercent * MyStats.attackPower.GetValue(), MyStats);

							if (effectPrefab != null)
							{
								ObjectPooler.Instance.ActivatePooledObject(effectPrefab, collidedStats.transform.position + new Vector3(0, 0.5f, 0), collidedStats.transform.rotation, tickFreq + 0.5f, collidedStats.transform);
							}
						}

					}
				}

				
			}
		}

		RemoveAndDestroy();
	}
}

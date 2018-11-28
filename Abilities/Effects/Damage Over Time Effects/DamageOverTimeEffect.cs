using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeEffect : Effect {

	public float attackPowerPercent = 1;
	public float tickFreq;

	private Vector3 offset = new Vector3(0, 1, 0);

	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		tickFreq = (configToSet as DamageOverTimeConfig).tickFrequency;
		attackPowerPercent = (configToSet as DamageOverTimeConfig).attackPowerPercent;
	}

	protected override void LogicBeforeDestroy()
	{
		theirActiveEffects[effectName][0].ResetEffectTimer();
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
				TheirStats.TakeDamage(attackPowerPercent * MyStats.attackPower.GetValue(), MyStats, false);

				if (effectPrefab != null)
				{
					ObjectPooler.Instance.ActivatePooledObject(effectPrefab, TheirStats.transform.position + offset, TheirStats.transform.rotation, tickFreq + 0.5f, transform);
				}
			}
		}

		RemoveAndDestroy();
	}
}

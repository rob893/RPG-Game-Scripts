using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SlowEffect : Effect {

	public float slowPercent;


	public override void SetConfig(EffectConfig effectConfig)
	{
		base.SetConfig(effectConfig);
		slowPercent = (config as SlowConfig).slowPercent;
	}

	protected override void LogicBeforeDestroy()
	{
		
	}

	protected override IEnumerator EffectLogic()
	{
		NavMeshAgent agent = TheirStats.GetComponent<NavMeshAgent>();

		if (effectPrefab != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(effectPrefab, TheirStats.transform.position, TheirStats.transform.rotation, duration, TheirStats.transform);
		}

		slowPercent = slowPercent / 100;
		slowPercent = Mathf.Clamp(slowPercent, 0.01f, 0.99f);
		float startingSpeed = agent.speed;
		agent.speed = startingSpeed * slowPercent;

		yield return new WaitForSeconds(duration);

		agent.speed = startingSpeed;

		RemoveAndDestroy();
	}
}

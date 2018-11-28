using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RootEffect : Effect {

	protected override void LogicBeforeDestroy()
	{

	}

	protected override IEnumerator EffectLogic()
	{
		NavMeshAgent agent = TheirStats.GetComponent<NavMeshAgent>();

		if(effectPrefab != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(effectPrefab, TheirStats.transform.position, TheirStats.transform.rotation, duration, TheirStats.transform);
		}

		agent.isStopped = true;

		yield return new WaitForSeconds(duration);

		agent.isStopped = false;

		RemoveAndDestroy();
	}
}

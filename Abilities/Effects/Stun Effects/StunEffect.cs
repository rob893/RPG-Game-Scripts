using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StunEffect : Effect {

	protected override void LogicBeforeDestroy()
	{
	
	}

	protected override IEnumerator EffectLogic()
	{
		NPCController npcController = null;
		PlayerController playerController = null;

		if (TheirStats.GetComponent<NPCController>())
		{
			npcController = TheirStats.GetComponent<NPCController>();
		}
		else if(TheirStats.GetComponent<PlayerController>())
		{
			playerController = TheirStats.GetComponent<PlayerController>();
		}
		
		NavMeshAgent agent = TheirStats.GetComponent<NavMeshAgent>();

		if (effectPrefab != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(effectPrefab, TheirStats.transform.position, TheirStats.transform.rotation, duration, TheirStats.transform);
		}

		if (!TheirStats.IsDead)
		{
			agent.isStopped = true;

			if (npcController != null)
			{
				npcController.enabled = false;
			}
			else if (playerController != null)
			{
				playerController.enabled = false;
			}
		}
		
		yield return new WaitForSeconds(duration);

		if (!TheirStats.IsDead)
		{
			if (npcController != null)
			{
				npcController.enabled = true;
			}
			else if (playerController != null)
			{
				playerController.enabled = true;
			}

			agent.isStopped = false;
		}

		RemoveAndDestroy();
	}
}

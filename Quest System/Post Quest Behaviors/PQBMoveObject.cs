using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterAnimator))]
public class PQBMoveObject : PostQuestBehavior {

	[SerializeField] private Transform targetLocation;
	[SerializeField] private bool destroyAfterMove = false;

	private NavMeshAgent agent;


	protected override void Start()
	{
		base.Start();

		agent = GetComponent<NavMeshAgent>();
	}

	protected override void DynamicQuestLogic()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			if(targetLocation != null)
			{
				if (GetComponent<DisableIfFarAway>())
				{
					GetComponent<DisableIfFarAway>().RemoveFromList();
				}

				agent.stoppingDistance = 3;
				agent.SetDestination(targetLocation.position);
				StartCoroutine(Moving());
			}
			
			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
		}
	}

	protected override bool IfQuestCompletedOnStart()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;

			if (destroyAfterMove)
			{
				Destroy(gameObject);
			}
			else
			{
				gameObject.SetActive(false);
				transform.position = targetLocation.position;
				gameObject.SetActive(true);
			}
			
			return true;
		}

		return false;
	}

	private IEnumerator Moving()
	{
		while ((transform.position - targetLocation.position).magnitude > 50)
		{
			yield return new WaitForSeconds(5);
		}


		while ((transform.position - targetLocation.position).magnitude > 5)
		{
			agent.SetDestination(targetLocation.position);
			yield return new WaitForSeconds(5);
		}
		
		agent.ResetPath();

		if (GetComponent<DisableIfFarAway>())
		{
			GetComponent<DisableIfFarAway>().AddToList();
		}

		if (destroyAfterMove)
		{
			Destroy(gameObject);
		}
	}
}

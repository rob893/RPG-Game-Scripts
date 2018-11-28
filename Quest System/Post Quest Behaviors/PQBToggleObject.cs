using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQBToggleObject : PostQuestBehavior
{
	[SerializeField] private bool disableAtStart = false;
	[SerializeField] private GameObject toggleGraphic;
	
	protected override void Start()
	{
		base.Start();

		if (disableAtStart)
		{
			if (GetComponent<DisableIfFarAway>())
			{
				GetComponent<DisableIfFarAway>().RemoveFromList();
			}

			gameObject.SetActive(false);
		}
	}

	protected override void DynamicQuestLogic()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			if(toggleGraphic != null)
			{
				ObjectPooler.Instance.ActivatePooledObject(toggleGraphic, transform.position, transform.rotation, 7);
			}

			if (disableAtStart)
			{
				gameObject.SetActive(true);
				if (GetComponent<DisableIfFarAway>())
				{
					GetComponent<DisableIfFarAway>().AddToList();
				}
			}
			else
			{
				gameObject.SetActive(false);
				if (GetComponent<DisableIfFarAway>())
				{
					GetComponent<DisableIfFarAway>().RemoveFromList();
				}
			}
			
			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
		}
	}

	protected override bool IfQuestCompletedOnStart()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			gameObject.SetActive(disableAtStart);
			disableAtStart = !disableAtStart;
			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
			return true;
		}

		return false;
	}
}

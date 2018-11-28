using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQBDestroyObject : PostQuestBehavior {

	
	protected override void DynamicQuestLogic()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			Destroy(gameObject);
		}
	}

	protected override bool IfQuestCompletedOnStart()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			Destroy(gameObject);
			return true;
		}

		return false;
	}
}

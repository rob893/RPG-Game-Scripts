using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PQBToggleMono : PostQuestBehavior {

	[SerializeField] private List<MonoBehaviour> scriptsToToggle = new List<MonoBehaviour>();
	[SerializeField] private bool destroyInsteadOfToggle = false;


	protected override void DynamicQuestLogic()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			foreach(MonoBehaviour script in scriptsToToggle)
			{
				if (destroyInsteadOfToggle)
				{
					Destroy(script);
				}
				else
				{
					script.enabled = !script.enabled;
				}
			}

			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
		}
	}

	protected override bool IfQuestCompletedOnStart()
	{
		if (questManager.RequestFinishedQuest(quest.id))
		{
			foreach (MonoBehaviour script in scriptsToToggle)
			{
				if (destroyInsteadOfToggle)
				{
					Destroy(script);
				}
				else
				{
					script.enabled = !script.enabled;
				}
			}

			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
			return true;
		}

		return false;
	}
}

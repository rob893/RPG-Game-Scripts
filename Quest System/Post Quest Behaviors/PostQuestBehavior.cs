using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PostQuestBehavior : MonoBehaviour {

	[SerializeField] protected QuestData quest;

	protected QuestManager questManager;


	protected virtual void Start()
	{
		questManager = QuestManager.Instance;

		if (IfQuestCompletedOnStart())
		{
			return;
		}

		questManager.GetMasterQuestDictionary()[quest.id].StateChanged += DynamicQuestLogic;
	}

	private void OnDestroy()
	{
		if(questManager != null)
		{
			questManager.GetMasterQuestDictionary()[quest.id].StateChanged -= DynamicQuestLogic;
		}
	}

	protected abstract void DynamicQuestLogic();

	protected abstract bool IfQuestCompletedOnStart();
}

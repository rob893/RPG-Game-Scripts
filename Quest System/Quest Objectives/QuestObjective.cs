using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestObjective : MonoBehaviour {

	[SerializeField] QuestData[] objectiveFor;
	[SerializeField] int value = 1;

	private QuestManager questManager;

	protected virtual void Start()
	{
		questManager = QuestManager.Instance;
	}

	protected void UpdateObjective()
	{
		foreach(QuestData quest in objectiveFor)
		{
			if (questManager.RequestAcceptedQuest(quest.id))
			{
				questManager.AddQuestItemToQuest(quest.id, value);
			}
		}
	}
}

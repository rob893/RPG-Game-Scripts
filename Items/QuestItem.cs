using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Item", menuName = "Inventory/Quest Item")]
public class QuestItem : Item {

	public List<QuestData> objectiveForList = new List<QuestData>();


	public void UpdateQuestObjectives(int amount = 1)
	{
		QuestManager questManager = QuestManager.Instance;
		
		foreach(QuestData quest in objectiveForList)
		{
			if (questManager.RequestAcceptedQuest(quest.id))
			{
				questManager.AddQuestItemToQuest(quest.id, amount);
			}
		}
	}

	protected override string GetDescription()
	{
		return "\nQuest Item";
	}
}

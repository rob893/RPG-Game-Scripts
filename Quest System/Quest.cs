using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest {

	public delegate void StateChange();
	public event StateChange StateChanged;

	public enum QuestProgress {NotAvailable, Available, Accepted, Complete, Done }

	private QuestData data;
	public string questTitle;
	public int id;
	public QuestProgress progress;
	[TextArea]
	public string questDescription;
	[TextArea]
	public string questHint;
	[TextArea]
	public string questCongrats;
	[TextArea]
	public string questSummary;
	public QuestData[] nextQuests; //next quest id if there is a next quest (chain quest)

	public string questObjective;
	public int questObjectiveCount;
	public int questObjectiveRequirement;

	public int expReward;
	public int goldReward;
	public Item[] itemReward;

	public Quest(QuestData questData)
	{
		data = questData;
	}

	public void ChangeState(QuestProgress newProgress)
	{
		progress = newProgress;
		if(StateChanged != null)
		{
			StateChanged();
		}
	}

	public void LoadData()
	{
		questTitle = data.questTitle;
		id = data.id;
		questDescription = data.questDescription;
		questHint = data.questHint;
		questCongrats = data.questCongrats;
		questSummary = data.questSummary;
		nextQuests = data.nextQuests;
		questObjective = data.questObjective;
		questObjectiveRequirement = data.questObjectiveRequirement;
		expReward = data.expReward;
		goldReward = data.goldReward;
		itemReward = data.itemReward;
		progress = data.initialQuestState;
	}
}

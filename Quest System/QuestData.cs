using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class QuestData : ScriptableObject {

	public string questTitle;
	public int id;
	public Quest.QuestProgress initialQuestState;
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

	public int questObjectiveRequirement;

	public int expReward;
	public int goldReward;
	public Item[] itemReward;


	public void SetQuestId()
	{
		Dictionary<int, QuestData> questDatabase = new Dictionary<int, QuestData>();
		foreach (QuestData quest in Resources.LoadAll<QuestData>("Quests"))
		{
			if (quest != this && !questDatabase.ContainsKey(quest.id))
			{
				questDatabase.Add(quest.id, quest);
			}
		}

		if (!questDatabase.ContainsKey(id))
		{
			Debug.Log("This quest has a unique id already!");
			return;
		}

		int i = 1;
		while (questDatabase.ContainsKey(i) && i < 10000)
		{
			i++;
		}
		Debug.Log("The next available quest id is " + i + ". Setting this quest's id to " + i);
		id = i;
	}
}

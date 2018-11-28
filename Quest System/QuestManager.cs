using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

	public static QuestManager Instance;

	public List<Quest> masterQuestList = new List<Quest>();

	private Dictionary<int, Quest> masterQuestDictionary = new Dictionary<int, Quest>();
	private Dictionary<int, Quest> currentQuestDictionary = new Dictionary<int, Quest>();
	

	private QuestManager() { }

	private void Awake()
	{
		if (Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		foreach (QuestData quest in Resources.LoadAll<QuestData>("Quests"))
		{
			Quest newQuest = new Quest(quest);
			newQuest.LoadData();
			masterQuestList.Add(newQuest);

			if (!masterQuestDictionary.ContainsKey(newQuest.id))
			{
				masterQuestDictionary.Add(newQuest.id, newQuest);
			}
			else
			{
				Debug.Log(newQuest.questTitle + " has the same ID as " + masterQuestDictionary[newQuest.id].questTitle);
			}
		}
	}

	public Dictionary<int, Quest> GetMasterQuestDictionary()
	{
		return masterQuestDictionary;
	}

	public Dictionary<int, Quest> GetCurrentQuestDictionary()
	{
		return currentQuestDictionary;
	}

	public void QuestRequest(QuestObject questGiver)
	{
		QuestUI questUI = UIManager.Instance.GetQuestUI();

		if(questGiver.availableQuests.Count > 0)
		{
			foreach(QuestData questGiverQuest in questGiver.availableQuests)
			{
				int questId = questGiverQuest.id;
				if (masterQuestDictionary.ContainsKey(questId) && masterQuestDictionary[questId].progress == Quest.QuestProgress.Available)
				{
					questUI.questAvailable = true;
					questUI.availableQuests.Add(masterQuestDictionary[questId]);
				}
			}
		}

		foreach (QuestData questGiverQuest in questGiver.receivableQuests)
		{
			int questId = questGiverQuest.id;
			if (masterQuestDictionary.ContainsKey(questId) && masterQuestDictionary[questId].progress == Quest.QuestProgress.Accepted || masterQuestDictionary[questId].progress == Quest.QuestProgress.Complete)
			{
				questUI.questRunning = true;
				questUI.runningQuests.Add(masterQuestDictionary[questId]);
			}
		}
	}

	public void SubscribeToQuests(QuestObject questGiver)
	{
		if(questGiver.receivableQuests.Count > 0)
		{
			foreach(QuestData receivableQuest in questGiver.receivableQuests)
			{
				masterQuestDictionary[receivableQuest.id].StateChanged += questGiver.OnStateChange;
			}
		}

		if (questGiver.availableQuests.Count > 0)
		{
			foreach (QuestData availableQuest in questGiver.availableQuests)
			{
				masterQuestDictionary[availableQuest.id].StateChanged += questGiver.OnStateChange;
			}
		}
	}

	public void UnsubscribeToQuests(QuestObject questGiver)
	{
		if (questGiver.receivableQuests.Count > 0)
		{
			foreach (QuestData receivableQuest in questGiver.receivableQuests)
			{
				masterQuestDictionary[receivableQuest.id].StateChanged -= questGiver.OnStateChange;
			}
		}

		if (questGiver.availableQuests.Count > 0)
		{
			foreach (QuestData availableQuest in questGiver.availableQuests)
			{
				masterQuestDictionary[availableQuest.id].StateChanged -= questGiver.OnStateChange;
			}
		}
	}

	public void AcceptQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			Quest quest = masterQuestDictionary[questId];
			if (quest.progress == Quest.QuestProgress.Available)
			{
				currentQuestDictionary.Add(questId ,quest);
				UIManager.Instance.ShowMessage("You have accepted " + masterQuestDictionary[questId].questTitle, Color.yellow);
				quest.ChangeState(Quest.QuestProgress.Accepted);
			}
		}

		else
		{
			Debug.Log("Quest id" + questId + "is not in the dictionary!");
		}
	}

	public void GiveUpQuest(int questId)
	{
		if (currentQuestDictionary.ContainsKey(questId))
		{
			Quest quest = currentQuestDictionary[questId];
			if(quest.progress == Quest.QuestProgress.Accepted)
			{
				quest.ChangeState(Quest.QuestProgress.Available);
				quest.questObjectiveCount = 0;
				currentQuestDictionary.Remove(questId);
			}
		}

		else
		{
			Debug.Log("Quest id" + questId + "is not in the dictionary!");
		}
	}

	public void CompleteQuest(int questId)
	{
		if (currentQuestDictionary.ContainsKey(questId))
		{
			Quest quest = currentQuestDictionary[questId];
			if (quest.progress == Quest.QuestProgress.Complete)
			{
				UIManager.Instance.ShowMessage("You have completed " + quest.questTitle, Color.yellow);
				quest.ChangeState(Quest.QuestProgress.Done);
				currentQuestDictionary.Remove(questId);

				if(quest.itemReward.Length > 0)
				{
					foreach(Item item in quest.itemReward)
					{
						Inventory.Instance.Add(item);
					}
				}

				Inventory.Instance.AddGold(quest.goldReward);
				PlayerManager.Instance.playerStats.GainExp(quest.expReward);

				CheckForChainQuest(questId);
			}
		}

		else
		{
			Debug.Log("Quest id" + questId + "is not in the dictionary!");
		}
	}

	private void CheckForChainQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			if(masterQuestDictionary[questId].nextQuests.Length > 0)
			{
				foreach(QuestData nextQuest in masterQuestDictionary[questId].nextQuests)
				{
					int nextQuestId = nextQuest.id;
					if (masterQuestDictionary.ContainsKey(nextQuestId) && masterQuestDictionary[nextQuestId].progress == Quest.QuestProgress.NotAvailable)
					{
						masterQuestDictionary[nextQuestId].ChangeState(Quest.QuestProgress.Available);
					}

					else
					{
						Debug.Log("Quest id " + nextQuestId + " is not in the dictionary or is not set to not available!");
					}
				}
			}
		}

		else
		{
			Debug.Log("Quest id " + questId + " is not in the dictionary!");
		}
	}

	public void AddQuestItem(string questObjective, int itemAmount)
	{
		foreach(KeyValuePair<int, Quest> questEntry in currentQuestDictionary)
		{
			Quest quest = questEntry.Value;
			if(quest.questObjective == questObjective && quest.progress == Quest.QuestProgress.Accepted)
			{
				quest.questObjectiveCount += itemAmount;
				UIManager.Instance.ShowMessage(quest.questObjective + ": " + quest.questObjectiveCount + "/" + quest.questObjectiveRequirement, Color.yellow);
			}

			if (quest.questObjectiveCount >= quest.questObjectiveRequirement && quest.progress == Quest.QuestProgress.Accepted)
			{
				quest.progress = Quest.QuestProgress.Complete;
			}
		}
	}

	public void AddQuestItemToQuest(int questId, int itemAmount)
	{
		if (currentQuestDictionary.ContainsKey(questId))
		{
			Quest quest = currentQuestDictionary[questId];
			if (quest.progress == Quest.QuestProgress.Accepted)
			{
				quest.questObjectiveCount += itemAmount;
				UIManager.Instance.ShowMessage(quest.questObjective + ": " + quest.questObjectiveCount + "/" + quest.questObjectiveRequirement, Color.yellow);
			}

			if (quest.questObjectiveCount >= quest.questObjectiveRequirement && quest.progress == Quest.QuestProgress.Accepted)
			{
				quest.ChangeState(Quest.QuestProgress.Complete);
			}
		}

		else
		{
			Debug.Log("You are not on this quest");
		}
	}

	public bool RequestAvailableQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			if(masterQuestDictionary[questId].progress == Quest.QuestProgress.Available)
			{
				return true;
			}
		}

		else
		{
			Debug.Log("Quest id " + questId + " is not in the dictionary!");
		}

		return false;
	}

	public bool RequestAcceptedQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			if (masterQuestDictionary[questId].progress == Quest.QuestProgress.Accepted)
			{
				return true;
			}
		}

		else
		{
			Debug.Log("Quest id " + questId + " is not in the dictionary!");
		}

		return false;
	}

	public bool RequestCompleteQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			if (masterQuestDictionary[questId].progress == Quest.QuestProgress.Complete)
			{
				return true;
			}
		}

		else
		{
			Debug.Log("Quest id " + questId + " is not in the dictionary!");
		}

		return false;
	}

	public bool RequestFinishedQuest(int questId)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			if (masterQuestDictionary[questId].progress == Quest.QuestProgress.Done)
			{
				return true;
			}
		}

		else
		{
			Debug.Log("Quest id " + questId + " is not in the dictionary!");
		}

		return false;
	}

	public bool CheckAvailableQuests(QuestObject questGiver)
	{
		foreach(QuestData quest in questGiver.availableQuests)
		{
			int questId = quest.id;
			if (masterQuestDictionary.ContainsKey(questId))
			{
				if(masterQuestDictionary[questId].progress == Quest.QuestProgress.Available)
				{
					return true;
				}
			}

			else
			{
				Debug.Log("Quest id " + questId + " is not in the dictionary!");
			}
		}
		return false;
	}

	public bool CheckAcceptedQuests(QuestObject questGiver)
	{
		foreach (QuestData quest in questGiver.receivableQuests)
		{
			int questId = quest.id;
			if (masterQuestDictionary.ContainsKey(questId))
			{
				if (masterQuestDictionary[questId].progress == Quest.QuestProgress.Accepted)
				{
					return true;
				}
			}

			else
			{
				Debug.Log("Quest id " + questId + " is not in the dictionary!");
			}
		}
		return false;
	}

	public bool CheckCompletedQuests(QuestObject questGiver)
	{
		foreach (QuestData quest in questGiver.receivableQuests)
		{
			int questId = quest.id;
			if (masterQuestDictionary.ContainsKey(questId))
			{
				if (masterQuestDictionary[questId].progress == Quest.QuestProgress.Complete)
				{
					return true;
				}
			}

			else
			{
				Debug.Log("Quest id " + questId + " is not in the dictionary!");
			}
		}
		return false;
	}

	public void SetQuestProgress(int questId, Quest.QuestProgress progress)
	{
		if (masterQuestDictionary.ContainsKey(questId))
		{
			masterQuestDictionary[questId].ChangeState(progress);
		}
	}

	public void SetCurrentQuest(int questId, int objCount)
	{
		if (!currentQuestDictionary.ContainsKey(questId))
		{
			currentQuestDictionary.Add(questId, masterQuestDictionary[questId]);
			currentQuestDictionary[questId].questObjectiveCount = objCount;
		}
	}
}

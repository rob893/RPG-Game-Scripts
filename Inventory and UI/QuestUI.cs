using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour {

	//TODO refactor this. It is pretty ugly.

	public GameObject availableQuestUI;
	public GameObject questLogUI;
	public bool questAvailable = false;
	public bool questRunning = false;
	public TextMeshProUGUI questTitle;
	public TextMeshProUGUI questDescription;
	public TextMeshProUGUI questRewards;
	public TextMeshProUGUI questLogTitle;
	public TextMeshProUGUI questLogDescription;
	public TextMeshProUGUI questLogRewards;
	public List<Quest> availableQuests = new List<Quest>();
	public List<Quest> runningQuests = new List<Quest>();
	public GameObject qButton; //ToDO make this like item rewards (enable/disable instead of creating and destroying.
	public GameObject acceptButton;
	public GameObject abandonButton;
	public GameObject completeButton;
	public Transform qButtonSpacer1;
	public Transform qButtonSpacer2;
	public Transform qLogButtonSpacer;
	public Transform qItemRewardSpacer;
	public Transform qLogItemRewardSpacer;

	private bool questPanelActive = false;
	private bool questLogActive = false;
	private UIManager uiManager;
	private List<GameObject> qButtons = new List<GameObject>();
	private List<GameObject> itemRewards = new List<GameObject>();
	


	private void Start()
	{
		uiManager = UIManager.Instance;

		HideQuestPanel();
		questLogUI.SetActive(false);
	}

	public void ToggleQuestLog()
	{
		if (questLogActive)
		{
			HideQuestLogPanel();
		}
		else
		{
			ShowQuestLogPanel();
		}

		uiManager.HideToolTip();
	}

	public void CheckQuests(QuestObject questGiver)
	{
		QuestManager.Instance.QuestRequest(questGiver);
		if((questRunning || questAvailable) && !questPanelActive)
		{
			ShowQuestPanel();
		}
	}

	public void ShowQuestPanel()
	{
		uiManager.PlayOpenMenuSound();
		questPanelActive = true;
		availableQuestUI.SetActive(questPanelActive);
		FillQuestButtons();
		uiManager.HideToolTip();
	}

	public void ShowQuestLogPanel()
	{
		uiManager.PlayOpenMenuSound();
		questLogDescription.text = "";
		questLogRewards.text = "";
		questLogTitle.text = "";
		abandonButton.SetActive(false);
		questLogActive = true;
		questLogUI.SetActive(questLogActive);
		if(questLogActive && !questPanelActive)
		{
			foreach(KeyValuePair<int, Quest> currentQuest in QuestManager.Instance.GetCurrentQuestDictionary())
			{
				GameObject questButton = Instantiate(qButton, qLogButtonSpacer, false);
				QuestButton qBScript = questButton.GetComponent<QuestButton>();
				qBScript.questId = currentQuest.Value.id;
				qBScript.questTitle.text = currentQuest.Value.questTitle;
				qBScript.isLogButton = true;
				qBScript.SetAbandonButton(abandonButton);
				qButtons.Add(questButton);
			}
		}
	}

	public void ShowQuestLogQuestInfo(int qId)
	{
		uiManager.PlaySelectSound();

		Quest quest = QuestManager.Instance.GetCurrentQuestDictionary()[qId];
		questLogTitle.text = quest.questTitle;
		if(quest.progress == Quest.QuestProgress.Accepted || quest.progress == Quest.QuestProgress.Complete)
		{
			questLogDescription.text = quest.questDescription;
			questLogDescription.text += "\n\nObjective: " + quest.questObjective + ": " + quest.questObjectiveCount + "/" + quest.questObjectiveRequirement;

			if (quest.questHint.Length > 0)
			{
				questLogDescription.text += "\n\nHint: " + quest.questHint;
			}

			questLogRewards.text = "You will receive:";
			if (quest.expReward > 0)
			{
				questLogRewards.text += "\nExperience: " + quest.expReward;
			}
			if (quest.goldReward > 0)
			{
				questLogRewards.text += "\nGold: " + quest.goldReward;
			}
			ClearItemRewards();
			if (quest.itemReward.Length > 0)
			{
				questLogRewards.text += "\nItems: ";
				foreach (Item item in quest.itemReward)
				{
					foreach(Transform itemRewardObject in qLogItemRewardSpacer)
					{
						if (!itemRewardObject.gameObject.activeInHierarchy)
						{
							itemRewardObject.GetComponent<QuestRewardItemSlot>().AddItem(item);
							itemRewards.Add(itemRewardObject.gameObject);
							break;
						}
					}
				}
			}
		}
	}

	public void HideQuestPanel()
	{
		questPanelActive = false;
		questAvailable = false;
		questRunning = false;
		questTitle.text = "";
		questDescription.text = "";
		questRewards.text = "";
		availableQuests.Clear();
		runningQuests.Clear();

		for(int i = 0; i < qButtons.Count; i++)
		{
			Destroy(qButtons[i]);
		}
		qButtons.Clear();

		uiManager.PlayCloseMenuSound();
		uiManager.HideToolTip();
		ClearItemRewards();
		availableQuestUI.SetActive(questPanelActive);
	}

	public void HideQuestLogPanel()
	{
		questLogActive = false;
		questLogTitle.text = "";
		questLogDescription.text = "";
		questLogRewards.text = "";

		for (int i = 0; i < qButtons.Count; i++)
		{
			Destroy(qButtons[i]);
		}
		qButtons.Clear();

		uiManager.PlayCloseMenuSound();
		ClearItemRewards();
		questLogUI.SetActive(questLogActive);
	} 

	private void ClearItemRewards()
	{
		for (int i = 0; i < itemRewards.Count; i++)
		{
			itemRewards[i].SetActive(false);
		}
		itemRewards.Clear();
	}

	private void FillQuestButtons()
	{
		foreach(Quest quest in availableQuests)
		{
			GameObject questButton = Instantiate(qButton, qButtonSpacer1, false);
			QuestButton qBScript = questButton.GetComponent<QuestButton>();
			qBScript.questId = quest.id;
			qBScript.questTitle.text = quest.questTitle;
			qBScript.SetAcceptButton(acceptButton);
			qBScript.SetCompleteButton(completeButton);
			qBScript.isLogButton = false;
			qButtons.Add(questButton);
		}

		foreach (Quest quest in runningQuests)
		{
			GameObject questButton = Instantiate(qButton, qButtonSpacer2, false);
			QuestButton qBScript = questButton.GetComponent<QuestButton>();
			qBScript.questId = quest.id;
			qBScript.questTitle.text = quest.questTitle;
			qBScript.SetAcceptButton(acceptButton);
			qBScript.SetCompleteButton(completeButton);
			qBScript.isLogButton = false;
			qButtons.Add(questButton);
		}
	}

	public void ShowSelectedQuest(int questId)
	{
		uiManager.PlaySelectSound();

		foreach(Quest quest in availableQuests)
		{
			if(quest.id == questId)
			{
				questTitle.text = quest.questTitle;
				if(quest.progress == Quest.QuestProgress.Available)
				{
					questDescription.text = quest.questDescription;
					questDescription.text += "\n\nObjective: " + quest.questObjective + ": " + quest.questObjectiveCount + "/" + quest.questObjectiveRequirement;

					if (quest.questHint.Length > 0)
					{
						questDescription.text += "\n\nHint: " + quest.questHint;
					}

					questRewards.text = "You will receive:";
					if(quest.expReward > 0)
					{
						questRewards.text += "\nExperience: " + quest.expReward;
					}
					if(quest.goldReward > 0)
					{
						questRewards.text += "\nGold: " + quest.goldReward;
					}

					ClearItemRewards();
					if(quest.itemReward.Length > 0)
					{
						questRewards.text += "\nItems: ";
						foreach(Item item in quest.itemReward)
						{
							foreach (Transform itemRewardObject in qItemRewardSpacer)
							{
								if (!itemRewardObject.gameObject.activeInHierarchy)
								{
									itemRewardObject.GetComponent<QuestRewardItemSlot>().AddItem(item);
									itemRewards.Add(itemRewardObject.gameObject);
									break;
								}
							}
						}
					}
				}
			}
		}

		foreach (Quest quest in runningQuests)
		{
			if (quest.id == questId)
			{
				questTitle.text = quest.questTitle;

				questRewards.text = "You will receive:";
				if (quest.expReward > 0)
				{
					questRewards.text += "\nExperience: " + quest.expReward;
				}
				if (quest.goldReward > 0)
				{
					questRewards.text += "\nGold: " + quest.goldReward;
				}

				ClearItemRewards();
				if (quest.itemReward.Length > 0)
				{
					questRewards.text += "\nItems: ";
					foreach (Item item in quest.itemReward)
					{
						foreach (Transform itemRewardObject in qItemRewardSpacer)
						{
							if (!itemRewardObject.gameObject.activeInHierarchy)
							{
								itemRewardObject.GetComponent<QuestRewardItemSlot>().AddItem(item);
								itemRewards.Add(itemRewardObject.gameObject);
								break;
							}
						}
					}
				}

				if (quest.progress == Quest.QuestProgress.Accepted)
				{
					questDescription.text = quest.questHint;
					questDescription.text += "\n" + quest.questObjective + ": " + quest.questObjectiveCount + "/" + quest.questObjectiveRequirement;
				}
				else if(quest.progress == Quest.QuestProgress.Complete)
				{
					questDescription.text = quest.questCongrats;
				}
			}
		}
	}
}

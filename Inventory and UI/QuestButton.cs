using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestButton : MonoBehaviour {

	public int questId;
	public TextMeshProUGUI questTitle;
	public bool isLogButton = false;
	public Sprite notSelectedImage;
	public Sprite selectedImage;

	private GameObject acceptButton;
	private GameObject completeButton;
	private GameObject abandonButton;
	private QuestButton acceptButtonScript;
	private QuestButton completeButtonScript;
	private QuestButton abandonButtonScript;
	private Image buttonImage;

	private void Start()
	{
		buttonImage = GetComponent<Image>();
	}

	public void ShowQuestInfo()
	{
		QuestButton[] qButtons = transform.parent.GetComponentsInChildren<QuestButton>();
		foreach(QuestButton button in qButtons)
		{
			button.SetNotSelectedImage();
		}
		buttonImage.sprite = selectedImage;
		if (isLogButton)
		{
			UIManager.Instance.GetQuestUI().ShowQuestLogQuestInfo(questId);
			abandonButton.SetActive(true);
			abandonButtonScript.questId = questId;
		}

		else
		{
			UIManager.Instance.GetQuestUI().ShowSelectedQuest(questId);

			if (QuestManager.Instance.RequestAvailableQuest(questId))
			{
				acceptButton.SetActive(true);
				acceptButtonScript.questId = questId;
			}
			else
			{
				acceptButton.SetActive(false);
			}

			if (QuestManager.Instance.RequestCompleteQuest(questId))
			{
				completeButton.SetActive(true);
				completeButtonScript.questId = questId;
			}
			else
			{
				completeButton.SetActive(false);
			}
		}
	}

	public void SetNotSelectedImage()
	{
		buttonImage.sprite = notSelectedImage;
	}

	public void SetAcceptButton(GameObject button)
	{
		acceptButton = button;
		acceptButtonScript = acceptButton.GetComponent<QuestButton>();
		acceptButton.SetActive(false);
	}

	public void SetCompleteButton(GameObject button)
	{
		completeButton = button;
		completeButtonScript = completeButton.GetComponent<QuestButton>();
		completeButton.SetActive(false);
	}
	public void SetAbandonButton(GameObject button)
	{
		abandonButton = button;
		abandonButtonScript = abandonButton.GetComponent<QuestButton>();
		abandonButton.SetActive(false);
	}


	public void AcceptQuest()
	{
		QuestManager.Instance.AcceptQuest(questId);
		UIManager.Instance.GetQuestUI().HideQuestPanel();
	}

	public void CompleteQuest()
	{
		QuestManager.Instance.CompleteQuest(questId);
		UIManager.Instance.GetQuestUI().HideQuestPanel();
	}

	public void GiveUpQuest()
	{
		QuestManager.Instance.GiveUpQuest(questId);
		UIManager.Instance.GetQuestUI().HideQuestLogPanel();
	}
}

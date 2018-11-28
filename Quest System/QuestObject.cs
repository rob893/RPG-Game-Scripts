using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestObject : Interactable {

	public List<QuestData> availableQuests = new List<QuestData>();
	public List<QuestData> receivableQuests = new List<QuestData>();

	private QuestMarker questMarker;
	private QuestManager questManager;
	private QuestUI questUI;

	protected override void Start()
	{
		base.Start();
		questManager = QuestManager.Instance;
		questMarker = GetComponentInChildren<QuestMarker>();
		questUI = UIManager.Instance.GetQuestUI();

		try
		{
			questManager.SubscribeToQuests(this);
			SetQuestMarker();
		}
		catch
		{
			StartCoroutine(LateStart());
		}
	}

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);

		questUI.CheckQuests(this);
		SetQuestMarker();
	}

	private void SetQuestMarker()
	{
		if (questManager.CheckCompletedQuests(this))
		{
			questMarker.SetCompletedQuestMarker();
		}

		else if (questManager.CheckAvailableQuests(this))
		{
			questMarker.SetAvailableQuestMarker();
		}
		else if (questManager.CheckAcceptedQuests(this))
		{
			questMarker.SetRunningQuestMarker();
		}
		else
		{
			questMarker.SetNoQuestMarker();
		}
	}

	public void OnStateChange()
	{
		SetQuestMarker();
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		questManager.SubscribeToQuests(this);
		SetQuestMarker();
	}

	private void OnDestroy()
	{
		if(questManager != null)
		{
			questManager.UnsubscribeToQuests(this);
		}
	}
}

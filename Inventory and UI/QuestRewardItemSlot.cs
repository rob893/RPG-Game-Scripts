using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardItemSlot : MonoBehaviour {

	private Image icon;
	private Item item;
	private UIManager uiManager;
	private RectTransform rt;
	private TextMeshProUGUI stackText;


	private void Start()
	{
		icon = GetComponent<Image>();
		uiManager = UIManager.Instance;
		rt = GetComponent<RectTransform>();

		if(GetComponentInChildren<TextMeshProUGUI>() != null)
		{
			stackText = GetComponentInChildren<TextMeshProUGUI>();
			stackText.text = "";
			stackText.enabled = false;
		}

		gameObject.SetActive(false);
	}

	public void AddItem(Item newItem, int stackCount = 1)
	{
		item = newItem;

		icon.sprite = item.icon;
		gameObject.SetActive(true);

		if(stackCount > 1)
		{
			if(stackText != null)
			{
				stackText.text = "x" + stackCount.ToString();
				stackText.enabled = true;
			}
			else
			{
				Debug.Log("No text object!");
			}
		}
		else
		{
			if (stackText != null)
			{
				stackText.text = "";
				stackText.enabled = false;
			}
		}
	}

	public void ClearSlot()
	{
		item = null;

		icon.sprite = null;
		gameObject.SetActive(false);
	}

	public void ShowToolTip()
	{
		if (item != null)
		{
			uiManager.SetToolTipLocation(new Vector2(transform.position.x - (rt.rect.width / 2), transform.position.y + (rt.rect.height / 2)));
			uiManager.ShowItemToolTip(item.GetToolTip());
		}
	}

	public void HideToolTip()
	{
		uiManager.HideToolTip();
	}
}

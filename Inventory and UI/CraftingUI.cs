using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftingUI : MonoBehaviour {

	[SerializeField] private GameObject craftingUI;
	[SerializeField] private GameObject itemButton;
	[SerializeField] private CraftingButton craftButton;
	[SerializeField] private Transform itemButtonSpacer1;
	[SerializeField] private Transform itemIconSpacer;
	[SerializeField] private Transform craftingMatIconSpacer;
	[SerializeField] private TextMeshProUGUI itemName;
	[SerializeField] private TextMeshProUGUI uiTitle;
	[SerializeField] private TextMeshProUGUI itemDescription;
	[SerializeField] private TextMeshProUGUI youWillCreateText;
	[SerializeField] private UnityEngine.UI.Scrollbar scrollBar;

	private UIManager uiManager;
	private PlayerStats playerStats;
	private List<GameObject> itemIconObjects = new List<GameObject>();
	private List<CraftingButton> cButtons = new List<CraftingButton>();


	private void Start()
	{
		uiManager = UIManager.Instance;
		playerStats = PlayerManager.Instance.playerStats;
		playerStats.OnCraftingSkilledUp += UpdateCraftingSkillText;

		List<Item> craftableItems = GameManager.Instance.GetCraftableItems();

		craftableItems.Sort((x, y) => x.craftingSkillRequired.CompareTo(y.craftingSkillRequired));

		foreach (Item item in craftableItems)
		{
			GameObject newItemButton = Instantiate(itemButton, itemButtonSpacer1, false);
			CraftingButton cButton = newItemButton.GetComponent<CraftingButton>();
			cButton.SetItem(item);
			cButtons.Add(cButton);
		}
		craftingUI.SetActive(false);
		ResetCraftingUI();
	}

	public void ToggleCraftingUI()
	{
		ResetCraftingUI();
		craftingUI.SetActive(!craftingUI.activeSelf);

		if (craftingUI.activeSelf)
		{
			uiManager.PlayOpenMenuSound();
		}
		else
		{
			uiManager.PlayCloseMenuSound();
		}

		scrollBar.value = 1;
		uiManager.HideToolTip();
	}

	private void ResetCraftingUI()
	{
		craftButton.gameObject.SetActive(false);
		itemName.text = "Select an item to craft!";
		itemDescription.text = "";
		youWillCreateText.enabled = false;
		UpdateCraftingSkillText();
		ResetButtonSprites();
		ClearItemObjects();
	}

	public void SetCraftItemButton(Item item)
	{
		ResetButtonSprites();
		ClearItemObjects();
		uiManager.PlaySelectSound();
		craftButton.gameObject.SetActive(true);
		craftButton.SetItem(item, false);
		youWillCreateText.enabled = true;
		itemName.text = item.itemName;
		itemDescription.text = GetItemCraftingRequirements(item);

		foreach (Transform itemIconObject in itemIconSpacer)
		{
			if (!itemIconObject.gameObject.activeInHierarchy)
			{
				itemIconObject.GetComponent<QuestRewardItemSlot>().AddItem(item);
				itemIconObjects.Add(itemIconObject.gameObject);
				break;
			}
		}
	}

	private void UpdateCraftingSkillText()
	{
		uiTitle.text = "Crafting Skill: " + playerStats.GetCraftingSkillLevel() + " / 100";
	}

	private void ResetButtonSprites()
	{
		foreach (CraftingButton button in cButtons)
		{
			button.SetNotSelectedImage();
		}
	}

	private string GetItemCraftingRequirements(Item item)
	{
		string craftingReqs = "To craft this item, you will need:\nCrafting Skill Level: " + item.craftingSkillRequired + "\nMaterials:";

		foreach (CraftingRequirement req in item.craftingRequirements)
		{
			foreach (Transform itemIconObject in craftingMatIconSpacer)
			{
				if (!itemIconObject.gameObject.activeInHierarchy)
				{
					itemIconObject.GetComponent<QuestRewardItemSlot>().AddItem(req.material, req.numMaterialRequired);
					itemIconObjects.Add(itemIconObject.gameObject);
					break;
				}
			}
		}

		return craftingReqs;
	}

	private void ClearItemObjects()
	{
		for (int i = 0; i < itemIconObjects.Count; i++)
		{
			itemIconObjects[i].SetActive(false);
		}
		itemIconObjects.Clear();
	}
}

using System.Collections;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour {
	//TODO clean up all these delegates.
	public Transform itemsParent;
	public Transform equippedItemsParent;
	public GameObject inventoryUI;
	public TextMeshProUGUI statsText;
	public TextMeshProUGUI statsText2;
	public TextMeshProUGUI goldText;

	private Inventory inventory;
	private EquipmentManager equipmentManager;
	private InventorySlot[] slots;
	private EquippedSlot[] equippedSlots;
	private PlayerStats playerStats;
	private UIManager uiManager;
	private int playerCurrentExp;
	private int playerExpToNextLevel;


	private void Start ()
	{
		playerStats = PlayerManager.Instance.playerStats;
		uiManager = UIManager.Instance;
		equipmentManager = EquipmentManager.Instance;
		inventory = Inventory.Instance;
		playerExpToNextLevel = playerStats.GetExpToNextLevel();
		playerCurrentExp = playerStats.GetCurrentExp();
		equipmentManager.onEquipmentChanged += UpdateEquippedUI;
		equipmentManager.onEquipmentChanged += UpdateStatsText;
		playerStats.OnExpGained += UpdateExp;
		playerStats.OnLeveledUp += UpdateLevel;
		playerStats.OnHealthChanged += UpdateHealth;
		playerStats.OnManaChanged += UpdateManaText;
		playerStats.armor.onStatChanged += UpdateLevel;
		playerStats.attackPower.onStatChanged += UpdateLevel;
		inventory.onItemChangedCallback += UpdateUI;
		inventory.onGoldChangedCallback += UpdateLevel;
		equippedSlots = equippedItemsParent.GetComponentsInChildren<EquippedSlot>();
		slots = itemsParent.GetComponentsInChildren<InventorySlot>();
		inventoryUI.SetActive(false);
	}

	private void UpdateEquippedUI(Equipment newItem, Equipment oldItem)
	{
		if(newItem != null && newItem != equipmentManager.GetUnarmed())
		{
			foreach (EquippedSlot slot in equippedSlots)
			{
				if (slot.slot == newItem.equipSlot)
				{
					slot.AddItem(newItem);
				}
			}
		}
		else
		{
			foreach(EquippedSlot slot in equippedSlots)
			{
				if(oldItem != null && slot.slot == oldItem.equipSlot)
				{
					slot.ClearSlot();
				}
			}
		}
	}

	public void OpenInventory()
	{
		if (inventoryUI.activeSelf)
		{
			uiManager.HideToolTip();
			inventoryUI.SetActive(false);
			uiManager.PlayCloseMenuSound();
		}
		else
		{
			uiManager.HideToolTip();
			inventoryUI.SetActive(true);
			StartCoroutine(UpdateText());
			uiManager.PlayOpenMenuSound();
		}
	}

	private void UpdateUI()
	{
		for(int i = 0; i < slots.Length; i++)
		{
			if (i < inventory.GetCurrentInventory().Count)
			{
				Item item = inventory.GetCurrentInventory()[i];
				if (item.stackable && inventory.GetStackedItems().ContainsKey(item))
				{
					slots[i].AddItem(item, inventory.GetStackedItems()[item]);
				}
				else
				{
					slots[i].AddItem(item, 1);
				}
			}
			else
			{
				slots[i].ClearSlot();
			}
		}
	}

	private void UpdateHealth(float healthAsPercentage, float currentHealth, float maxHealth)
	{
		StartCoroutine(UpdateText());
	}

	private void UpdateManaText(float manaAsPercentage)
	{
		StartCoroutine(UpdateText());
	}

	private void UpdateExp(int currentExp, int expToNextLevel)
	{
		playerCurrentExp = currentExp;
		playerExpToNextLevel = expToNextLevel;
		StartCoroutine(UpdateText());
	}

	private void UpdateLevel()
	{
		StartCoroutine(UpdateText());
	}

	private void UpdateStatsText(Equipment newItem, Equipment oldItem)
	{
		StartCoroutine(UpdateText());
	}

	private IEnumerator UpdateText()
	{
		if (!inventoryUI.activeInHierarchy)
		{
			yield break;
		}

		yield return new WaitForEndOfFrame();
		
		statsText.text = "Name: " + playerStats.characterName + "\nLevel: " + playerStats.GetLevel() + "\nExperience: " + playerCurrentExp + " / " + playerExpToNextLevel
			+ "\nHealth: " + (int)playerStats.CurrentHealth + " / " + playerStats.health.GetValue() + "\nMana: " + (int)playerStats.CurrentMana + " / " + playerStats.mana.GetValue();

		statsText2.text = "Armor: " + playerStats.armor.GetValue() + "\nAttack Power: " + playerStats.attackPower.GetValue() + "\nCritical Strike Chance: " + playerStats.crit.GetValue() 
			+ "\nHealth Regain: " + playerStats.healthRegainPerFiveSeconds.GetValue() + "\nMana Regain: " + playerStats.manaRegainPerSecond.GetValue();

		goldText.text = "" + inventory.GetGoldCount();
	}
}

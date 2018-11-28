using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	public Image icon;
	public Button removeButton;
	public TextMeshProUGUI stackCountText;

	private Item item;
	private UIManager uiManager;
	private Inventory inventory;
	private RectTransform rt;
	private int stackSize;

	private void Start()
	{
		uiManager = UIManager.Instance;
		inventory = Inventory.Instance;
		rt = GetComponent<RectTransform>();
	}

	public void AddItem(Item newItem, int stackCount)
	{
		item = newItem;
		icon.sprite = item.icon;
		icon.enabled = true;
		stackSize = stackCount;
		
		if(newItem is Gold)
		{
			stackCountText.text = newItem.sellValue.ToString();
			stackCountText.enabled = true;
		}
		else if (stackCount > 1)
		{
			stackCountText.text = stackCount.ToString();
			stackCountText.enabled = true;
		}
		else
		{
			stackCountText.text = "";
			stackCountText.enabled = false;
		}

		if (removeButton != null)
		{
			removeButton.interactable = true;
		}
	}

	public bool HasItem()
	{
		return item != null;
	}

	public Item GetItem()
	{
		return item;
	}

	public void ClearSlot()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		stackCountText.text = "";
		stackCountText.enabled = false;
		stackSize = 0;

		if (removeButton != null)
		{
			removeButton.interactable = false;
		}
	}

	public void OnRemoveButton()
	{
		inventory.Remove(item);
	}

	public void UseItem()
	{
		if(item != null)
		{
			if (uiManager.GetVendorPanel().activeInHierarchy && Input.GetKey(KeyCode.LeftShift))
			{
				SellItem();
				uiManager.HideToolTip();
			}
			else
			{
				item.Use();
				uiManager.HideToolTip();
			}
			
		}
	}

	public void LootItem()
	{
		if(item != null)
		{
			if(item is Gold)
			{
				inventory.AddGold(item.sellValue);
				uiManager.HideToolTip();
				ClearSlot();
			}
			else
			{
				if (item is QuestItem)
				{
					(item as QuestItem).UpdateQuestObjectives(stackSize);
				}
				else
				{
					if(!inventory.Add(item, stackSize))
					{
						return;
					}
				}
				
				uiManager.HideToolTip();
				ClearSlot();
			}
		}
	}

	public void SellItem()
	{
		if (HasItem())
		{
			inventory.AddGold(item.sellValue);
			if (item.stackable)
			{
				inventory.UseStackedItem(item);
			}
			else
			{
				inventory.Remove(item);
			}
		}
	}

	public void BuyItem()
	{
		if (HasItem())
		{
			if (inventory.HasEnoughGold(item.sellValue))
			{
				if(item is QuestItem)
				{
					(item as QuestItem).UpdateQuestObjectives();
					inventory.SpendGold(item.sellValue);
				}
				else
				{
					if (inventory.Add(item))
					{
						inventory.SpendGold(item.sellValue);
					}
					else
					{
						return;
					}
				}
			}
			else
			{
				uiManager.ShowMessage("You don't have enough gold!");
			}
		}
	}

	public void ShowToolTip()
	{
		if(item != null)
		{
			uiManager.SetToolTipLocation(new Vector2(transform.position.x - (rt.rect.width / 2), transform.position.y));
			uiManager.ShowItemToolTip(item.GetToolTip());
		}
	}
}

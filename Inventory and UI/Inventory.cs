using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

	public static Inventory Instance;

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public delegate void OnGoldChanged();
	public OnGoldChanged onGoldChangedCallback;

	[SerializeField] private List<Item> items = new List<Item>();
	[SerializeField] private Dictionary<Item, int> stackedItems = new Dictionary<Item, int>();
	[SerializeField] private int space = 32;
	[SerializeField] private int currentGold = 0;

	[Header("Inventory Sounds")]
	[SerializeField] private AudioClip lootGoldSound;
	[SerializeField] private AudioClip lootArmorSound;
	[SerializeField] private AudioClip lootWeaponSound;
	[SerializeField] private AudioClip lootNonEquipmentSound;

	private AudioManager audioManager;
	private UIManager uiManager;

	private Inventory() { }

	private void Awake()
	{
		if(Instance == null)
		{
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		audioManager = AudioManager.Instance;
		uiManager = UIManager.Instance;
	}

	public List<Item> GetCurrentInventory()
	{
		return items;
	}

	public Dictionary<Item, int> GetStackedItems()
	{
		return stackedItems;
	}

	private bool AddItems(List<Item> itemsToAdd)
	{
		if(space - items.Count < itemsToAdd.Count)
		{
			uiManager.ShowMessage("Not enough room in your inventory!");
			return false;
		}

		foreach(Item item in itemsToAdd)
		{
			Add(item);
		}

		return true;
	}

	public bool Add(Item item, int stackSize = 1)
	{
		if(items.Count >= space && !item.stackable)
		{
			uiManager.ShowMessage("Not enough room in your inventory!");
			return false;
		}

		if(!item.stackable && stackSize > 1)
		{
			Debug.Log("the item is not stackable but a stack size of > than 1 was passed in! Returning.");
			return false;
		}

		if(item.stackable)
		{
			if (items.Contains(item))
			{
				if (stackedItems.ContainsKey(item))
				{
					stackedItems[item] += stackSize;
				}
				else
				{
					stackedItems.Add(item, 1 + stackSize);
				}
			}
			else
			{
				if(items.Count >= space)
				{
					uiManager.ShowMessage("Not enough room in your inventory!");
					return false;
				}

				items.Add(item);
				stackedItems.Add(item, stackSize);
			}
		}
		else
		{
			items.Add(item);
		}

		if(onItemChangedCallback != null)
		{
			onItemChangedCallback.Invoke();
		}

		PlayLootSound(item, true);

		return true;
	}

	public bool HasEnoughOfItem(Item item, int amount)
	{
		if (!items.Contains(item))
		{
			return false;
		}

		if(amount == 1)
		{
			return true;
		}
		else
		{
			if(stackedItems.ContainsKey(item) && stackedItems[item] >= amount)
			{
				return true;
			}
		}

		return false;
	}

	public void Remove(Item item)
	{
		items.Remove(item);

		if (stackedItems.ContainsKey(item))
		{
			stackedItems.Remove(item);
		}

		if (onItemChangedCallback != null)
		{
			onItemChangedCallback.Invoke();
		}
	}

	public void UseStackedItem(Item item, int amount = 1)
	{
		if (item.stackable)
		{
			if (!HasEnoughOfItem(item, amount))
			{
				return;
			}

			if (stackedItems.ContainsKey(item))
			{
				stackedItems[item] = stackedItems[item] - amount;
				onItemChangedCallback.Invoke();
				if (stackedItems[item] <= 0)
				{
					Remove(item);
				}
			}
			else
			{
				Remove(item);
			}
		}
		else
		{
			Debug.Log(item.itemName + " is not stackable!");
		}
	}

	public void AddGold(int amount)
	{
		PlayLootSound(null, true, true);
		currentGold += amount;

		if (onGoldChangedCallback != null)
		{
			onGoldChangedCallback.Invoke();
		}
	}

	public void SpendGold(int amount)
	{
		currentGold -= amount;
		PlayLootSound(null, true, true);
		if (onGoldChangedCallback != null)
		{
			onGoldChangedCallback.Invoke();
		}
	}

	public int GetGoldCount()
	{
		return currentGold;
	}

	public void SetGoldCount(int amount)
	{
		currentGold = amount;
	}

	public bool HasEnoughGold(int amount)
	{
		return currentGold >= amount;
	}

	public bool HasEnoughRoomInInventory()
	{
		return items.Count < space;
	}

	public void LoadSavedInventory(int savedGoldAmount, List<Item> savedInventory, Dictionary<Item, int> savedStackedItems)
	{
		AddItems(savedInventory);
		currentGold = savedGoldAmount;
		stackedItems.Clear();
		stackedItems = savedStackedItems;
		onItemChangedCallback.Invoke();
	}

	public void PlayLootSound(Item item, bool isOneShot = false, bool isGold = false)
	{
		if (audioManager == null)
		{
			return;
		}

		if (isGold || item is Gold)
		{
			audioManager.PlaySoundEffect(lootGoldSound, isOneShot);
		}
		else if (item is Weapon)
		{
			audioManager.PlaySoundEffect(lootWeaponSound, isOneShot);
		}
		else if (item is Equipment)
		{
			audioManager.PlaySoundEffect(lootArmorSound, isOneShot);
		}
		else
		{
			audioManager.PlaySoundEffect(lootNonEquipmentSound, isOneShot);
		}
	}
}

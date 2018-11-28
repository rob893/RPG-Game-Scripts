using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	//Singleton Pattern
	public static GameManager Instance;

	public bool dataToLoad = false;
	public bool scaleEnemies = false;
	public Difficulty difficulty = Difficulty.Normal;

	private List<Item> inventory = null;
	private Dictionary<Item, int> stackedItems = null;
	private Equipment[] currentEquipment = null;
	private PlayerStatistics playerStatistics = null;
	private int currentPlayerGold;
	private Dictionary<int, Item> itemDatabase = new Dictionary<int, Item>();
	private Dictionary<int, AbilityConfig> abilityDatabase = new Dictionary<int, AbilityConfig>();
	private Dictionary<int, Vector3> playerLocationDictionary = new Dictionary<int, Vector3>();
	private Dictionary<KeyCode, int> currentPlayerAbilities = new Dictionary<KeyCode, int>();
	private int sceneIndex;


	//Singleton
	private GameManager() { }

	private void Awake()
	{
		//enforce singleton
		if(Instance == null)
		{
			DontDestroyOnLoad(gameObject);
			Instance = this;
		}
		else if(Instance != this)
		{
			Destroy(gameObject);
		}

		List<int> duplicateIds = new List<int>();
		foreach (Item item in Resources.LoadAll<Item>("Items"))
		{
			if (!itemDatabase.ContainsKey(item.itemId))
			{
				itemDatabase.Add(item.itemId, item);
			}
			else
			{
				duplicateIds.Add(item.itemId);
				Debug.Log("Duplicate item id for " + item.itemName + " found! Look into item id " + item.itemId);
			}
		}

		if(duplicateIds.Count > 0)
		{
			int i = 1;
			while (itemDatabase.ContainsKey(i))
			{
				i++;
			}
			Debug.Log("The next available item id is " + i);
		}
		duplicateIds.Clear();

		foreach (AbilityConfig ability in Resources.LoadAll<AbilityConfig>("Abilities/Player Abilities"))
		{
			if (!abilityDatabase.ContainsKey(ability.abilityId))
			{
				abilityDatabase.Add(ability.abilityId, ability);
			}
			else
			{
				duplicateIds.Add(ability.abilityId);
				Debug.Log("Duplicate player ability id found! Look into " + ability.name + " with id of " + ability.abilityId);
			}
		}

		if (duplicateIds.Count > 0)
		{
			int i = 1;
			while (abilityDatabase.ContainsKey(i))
			{
				i++;
			}
			Debug.Log("The next available ability id is " + i);
		}

		if (PlayerPrefs.HasKey("enemyScaling"))
		{
			if (PlayerPrefs.GetInt("enemyScaling") == 1)
			{
				scaleEnemies = true;
			}
			else
			{
				scaleEnemies = false;
			}
		}

		if (PlayerPrefs.HasKey("difficulty"))
		{
			if (PlayerPrefs.GetInt("difficulty") == 0)
			{
				difficulty = Difficulty.Easy;
			}
			else if (PlayerPrefs.GetInt("difficulty") == 1)
			{
				difficulty = Difficulty.Normal;
			}
			else
			{
				difficulty = Difficulty.Hard;
			}
		}
		else
		{
			difficulty = Difficulty.Normal;
		}
	}

	public PlayerStatistics GetPlayerStatistics()
	{
		return playerStatistics;
	}

	public int GetSceneIndex()
	{
		return sceneIndex;
	}

	public List<Item> GetInventory()
	{
		return inventory;
	}

	public Dictionary<Item, int> GetStackedItems()
	{
		return stackedItems;
	}

	public Equipment[] GetCurrentEquipment()
	{
		return currentEquipment;
	}

	public int GetPlayerGold()
	{
		return currentPlayerGold;
	}

	public void SetLayer(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform child in obj.transform)
			SetLayer(child.gameObject, layer);
	}

	public Item GetItemFromDatabase(int itemId)
	{
		return itemDatabase[itemId];
	}

	public List<Item> GetCraftableItems()
	{
		List<Item> craftableItems = new List<Item>();
		foreach(KeyValuePair<int, Item> item in itemDatabase)
		{
			if (item.Value.craftable)
			{
				craftableItems.Add(item.Value);
			}
		}

		return craftableItems;
	}

	public void SetNextSceneLocation(int nextSceneIndex, Vector3 nextSceneLocation)
	{
		if (playerLocationDictionary.ContainsKey(nextSceneIndex))
		{
			Debug.Log("overriding previous location");
			playerLocationDictionary[nextSceneIndex] = nextSceneLocation;
		}
		else
		{
			playerLocationDictionary.Add(nextSceneIndex, nextSceneLocation);
		}
	}

	public void SaveDataBetweenScenes()
	{
		dataToLoad = true;
		PlayerManager.Instance.playerStats.SaveStats();
		playerStatistics = PlayerManager.Instance.playerStats.GetPlayerStatistics();
		inventory = Inventory.Instance.GetCurrentInventory();
		stackedItems = Inventory.Instance.GetStackedItems();
		currentPlayerGold = Inventory.Instance.GetGoldCount();
		currentEquipment = EquipmentManager.Instance.currentEquipment;

		currentPlayerAbilities.Clear();
		foreach(KeyValuePair<KeyCode, Ability> entry in PlayerManager.Instance.selectedAbilities)
		{
			currentPlayerAbilities.Add(entry.Key, entry.Value.GetAbilityId());
		}

		int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (playerLocationDictionary.ContainsKey(activeSceneIndex))
		{
			playerLocationDictionary[activeSceneIndex] = PlayerManager.Instance.transform.position;
		} 
		else
		{
			playerLocationDictionary.Add(activeSceneIndex, PlayerManager.Instance.transform.position);
		}
	}

	public void LoadDataToPlayer()
	{
		StartCoroutine(LoadDataCoroutine());
	}

	private IEnumerator LoadDataCoroutine()
	{
		int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
		if (playerLocationDictionary.ContainsKey(activeSceneIndex))
		{
			PlayerManager.Instance.gameObject.SetActive(false);
			PlayerManager.Instance.transform.position = playerLocationDictionary[activeSceneIndex];
			PlayerManager.Instance.gameObject.SetActive(true);
			playerLocationDictionary.Remove(activeSceneIndex);
		}

		yield return new WaitForEndOfFrame();

		Inventory.Instance.LoadSavedInventory(currentPlayerGold, inventory, stackedItems);
		PlayerManager.Instance.playerStats.LoadSavedStats(playerStatistics);
		PlayerManager.Instance.LoadCurrentAbilities(currentPlayerAbilities, abilityDatabase);
		EquipmentManager.Instance.LoadSavedEquipment(currentEquipment);
	}

	public void SaveGame()
	{
		Save save = CreateSaveGameObject();

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
		bf.Serialize(file, save);
		file.Close();

		UIManager.Instance.ShowMessage("Game Saved!");
	}

	private Save CreateSaveGameObject()
	{
		SaveDataBetweenScenes();
		Save save = new Save();

		save.playerStatistics = playerStatistics;
		save.playerGold = Inventory.Instance.GetGoldCount();
		save.sceneIndex = SceneManager.GetActiveScene().buildIndex;
		save.currentPlayerAbilities = currentPlayerAbilities;

		foreach(KeyValuePair<int, Vector3> location in playerLocationDictionary)
		{
			float[] playerLocation = new float[3];
			playerLocation[0] = location.Value.x;
			playerLocation[1] = location.Value.y;
			playerLocation[2] = location.Value.z;

			if (save.playerLocations.ContainsKey(location.Key))
			{
				Debug.Log("Overriding a location for some reason.");
				save.playerLocations[location.Key] = playerLocation;
			}
			else
			{
				save.playerLocations.Add(location.Key, playerLocation);
			}
		}

		foreach (KeyValuePair<int, Quest> quest in QuestManager.Instance.GetMasterQuestDictionary())
		{
			save.masterQuestList.Add(quest.Key, quest.Value.progress);
		}

		foreach (KeyValuePair<int, Quest> quest in QuestManager.Instance.GetCurrentQuestDictionary())
		{
			save.currentQuests.Add(quest.Key, quest.Value.questObjectiveCount);
		}

		foreach (Item item in inventory)
		{
			if (item.stackable && stackedItems.ContainsKey(item))
			{
				save.stackItemsIds.Add(item.itemId, stackedItems[item]);
			}
			
			save.inventoryIds.Add(item.itemId);
		}

		foreach(Equipment equipment in currentEquipment)
		{
			if(equipment != null)
			{
				save.currentEquipmentIds.Add(equipment.itemId);
			}
		}

		return save;
	}

	public bool HasSavedGamed()
	{
		return File.Exists(Application.persistentDataPath + "/gamesave.save");
	}

	public void LoadGame()
	{
		if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
			Save save = (Save)bf.Deserialize(file);
			file.Close();

			playerStatistics = save.playerStatistics;
			currentPlayerGold = save.playerGold;
			sceneIndex = save.sceneIndex;

			Dictionary<int, Vector3> locationsToLoad = new Dictionary<int, Vector3>();
			foreach(KeyValuePair<int, float[]> location in save.playerLocations)
			{
				Vector3 playerLocation = new Vector3(location.Value[0], location.Value[1], location.Value[2]);

				if (locationsToLoad.ContainsKey(location.Key))
				{
					locationsToLoad[location.Key] = playerLocation;
				}
				else
				{
					locationsToLoad.Add(location.Key, playerLocation);
				}
			}
			playerLocationDictionary = locationsToLoad;

			foreach (KeyValuePair<int, Quest.QuestProgress> quest in save.masterQuestList)
			{
				QuestManager.Instance.SetQuestProgress(quest.Key, quest.Value);
			}

			QuestManager.Instance.GetCurrentQuestDictionary().Clear();
			foreach(KeyValuePair<int, int> quest in save.currentQuests)
			{
				QuestManager.Instance.SetCurrentQuest(quest.Key, quest.Value);
			}

			Dictionary<Item, int> stackedItemsToLoad = new Dictionary<Item, int>();
			foreach(KeyValuePair<int, int> itemEntry in save.stackItemsIds)
			{
				if (itemDatabase.ContainsKey(itemEntry.Key))
				{
					Item itemToLoad = itemDatabase[itemEntry.Key];
					if (itemEntry.Value > 1)
					{
						stackedItemsToLoad.Add(itemToLoad, itemEntry.Value);
					}
					else
					{
						//Debug.Log("something went wrong loading itemId " + itemEntry.Key);
					}
				}
				else
				{
					Debug.Log("something went wrong loading itemId " + itemEntry.Key);
				}
			}
			stackedItems = stackedItemsToLoad;

			List<Item> inventoryToLoad = new List<Item>();
			foreach (int itemId in save.inventoryIds)
			{
				if (itemDatabase.ContainsKey(itemId))
				{
					Item itemToLoad = itemDatabase[itemId];
					inventoryToLoad.Add(itemToLoad);
				}
				else
				{
					Debug.Log("something went wrong loading itemId " + itemId);
				}
			}
			inventory = inventoryToLoad;

			foreach(int itemId in save.currentEquipmentIds)
			{
				if (itemDatabase.ContainsKey(itemId))
				{
					if (currentEquipment != null)
					{
						currentEquipment[(int)((itemDatabase[itemId] as Equipment).equipSlot)] = itemDatabase[itemId] as Equipment;
					}
					else
					{
						int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
						currentEquipment = new Equipment[numSlots];
						currentEquipment[(int)((itemDatabase[itemId] as Equipment).equipSlot)] = itemDatabase[itemId] as Equipment;
					}
				}
				else
				{
					Debug.Log("Something went wrong.");
				}
			}

			currentPlayerAbilities = save.currentPlayerAbilities;

			dataToLoad = true;
		}

		else
		{
			Debug.Log("No game saved!");
		}
	}
}

[System.Serializable]
public class Save
{
	public PlayerStatistics playerStatistics;
	public int sceneIndex;
	public int playerGold;
	public List<int> currentEquipmentIds = new List<int>();
	public List<int> inventoryIds = new List<int>();
	public Dictionary<int, int> stackItemsIds = new Dictionary<int, int>(); //ItemId => Stack count.
	public Dictionary<KeyCode, int> currentPlayerAbilities = new Dictionary<KeyCode, int>();
	public Dictionary<int, Quest.QuestProgress> masterQuestList = new Dictionary<int, Quest.QuestProgress>(); //QuestID => quest progress
	public Dictionary<int, int> currentQuests = new Dictionary<int, int>(); //QuestID => quest objective count
	public Dictionary<int, float[]> playerLocations = new Dictionary<int, float[]>(); //scene index => float[3] (vector3 is not serializable, so [0] = x, [1] = y, [2] = z).
}

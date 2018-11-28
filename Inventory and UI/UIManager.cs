using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

	public static UIManager Instance;

	[SerializeField] private GameObject blocksRaycastElements;
	[SerializeField] private GameObject doesNotBlockRaycastElements;
	[SerializeField] private MapController mapController;
	[SerializeField] private GameObject pauseMenuUI;
	[SerializeField] private GameObject loadingScreen;
	[SerializeField] private GameObject selectAbilitiesUI;
	[SerializeField] private GameObject vendorPanel;
	[SerializeField] private GameObject lootPanel;
	[SerializeField] private GameObject floatingCombatText;
	[SerializeField] private QuestUI questUI;
	[SerializeField] private CraftingUI craftingUI;
	[SerializeField] private InventoryUI inventoryUI;
	[SerializeField] private Slider loadBarSlider;
	[SerializeField] private GameObject itemToolTipUI;
	[SerializeField] private GameObject fixedToolTipUI;
	[SerializeField] private GameObject sceneMessageTextGameObject;
	[SerializeField] private GameObject mouseOverHealthBar;
	[SerializeField] private Image mouseOverHealthBarFill;
	[SerializeField] private bool gameIsPaused = false;
	[SerializeField] private bool gameIsOver = false;

	[Header("UI Sounds")]
	[SerializeField] private AudioClip selectSound;
	[SerializeField] private AudioClip chooseSound;
	[SerializeField] private AudioClip openInventorySound;
	[SerializeField] private AudioClip closeInventorySound;

	private TextMeshProUGUI itemToolTipText;
	private RectTransform itemToolTipBackground;
	private TextMeshProUGUI fixedToolTipText;
	private TextMeshProUGUI sceneMessageText;
	private TextMeshProUGUI mouseOverText;
	private AudioManager audioManager;
	private float sceneMessageTimer = 0;

	#region Getters

	public LootPanelScript GetLootPanelScript()
	{
		return lootPanel.GetComponent<LootPanelScript>();
	}

	public GameObject GetVendorPanel()
	{
		return vendorPanel;
	}

	public GameObject GetFloatingCombatText()
	{
		return floatingCombatText;
	}

	public QuestUI GetQuestUI()
	{
		return questUI;
	}

	public MapController GetWolrdMapController()
	{
		return mapController;
	}

	#endregion

	//Singleton
	private UIManager() { }

	private void Awake()
	{
		//enforce singleton
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		mouseOverText = mouseOverHealthBar.GetComponentInChildren<TextMeshProUGUI>();
		sceneMessageText = sceneMessageTextGameObject.GetComponent<TextMeshProUGUI>();
		itemToolTipText = itemToolTipUI.GetComponentInChildren<TextMeshProUGUI>();
		itemToolTipBackground = itemToolTipUI.GetComponent<RectTransform>();
		fixedToolTipText = fixedToolTipUI.GetComponentInChildren<TextMeshProUGUI>();

		itemToolTipUI.SetActive(false);
		fixedToolTipUI.SetActive(false);
		loadingScreen.SetActive(false);
		selectAbilitiesUI.SetActive(false);
		mouseOverHealthBar.SetActive(false);
		sceneMessageTextGameObject.SetActive(false);
	}

	private void Start()
	{
		audioManager = AudioManager.Instance;
		Resume();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Pause();
		}
		if (Input.GetKeyDown(KeyCode.F5))
		{
			SaveGame();
		}
		if (Input.GetKeyDown(KeyCode.N))
		{
			ToggleSelectAbilitiesUI();
		}
		if (Input.GetKeyDown(KeyCode.J))
		{
			questUI.ToggleQuestLog();
		}
		if (Input.GetKeyDown(KeyCode.B))
		{
			inventoryUI.OpenInventory();
		}
		if (Input.GetKeyDown(KeyCode.K))
		{
			craftingUI.ToggleCraftingUI();
		}
		if (Input.GetKeyDown(KeyCode.M))
		{
			mapController.ToggleMap();
		}
	}

	public void PlaySelectSound()
	{
		if (audioManager == null)
		{
			return;
		}

		audioManager.PlaySoundEffect(selectSound, true);
	}

	public void PlayOpenMenuSound()
	{
		if (audioManager == null)
		{
			return;
		}

		audioManager.PlaySoundEffect(openInventorySound, true);
	}

	public void PlayCloseMenuSound()
	{
		if(audioManager == null)
		{
			return;
		}

		audioManager.PlaySoundEffect(closeInventorySound, true);
	}

	public void PlayChooseSound()
	{
		if (audioManager == null)
		{
			return;
		}

		audioManager.PlaySoundEffect(chooseSound, true);
	}

	public void ToggleSelectAbilitiesUI()
	{
		selectAbilitiesUI.SetActive(!selectAbilitiesUI.activeSelf);

		if (selectAbilitiesUI.activeSelf)
		{
			PlayOpenMenuSound();
		}
		else
		{
			PlayCloseMenuSound();
		}

		HideToolTip();
	}

	public void ToggleUIElements(bool active)
	{
		blocksRaycastElements.SetActive(active);
		doesNotBlockRaycastElements.SetActive(active);
	}

	public void ShowFixedToolTip(string text)
	{
		fixedToolTipText.text = text;
		fixedToolTipUI.SetActive(true);
	}

	public void ShowItemToolTip(string text)
	{
		itemToolTipText.text = text;
		itemToolTipUI.SetActive(true);
	}
	
	public void SetToolTipLocation(Vector2 position)
	{
		itemToolTipBackground.pivot = new Vector2(1, position.y / Screen.height);
		itemToolTipBackground.position = position;
	}

	public void ShowMouseOverHealthBar(string text, float healthAsPercentage, CharacterStats stats)
	{
		mouseOverHealthBarFill.fillAmount = healthAsPercentage;
		mouseOverText.text = text;
		stats.OnHealthChanged += UpdateHealthBar;
		mouseOverHealthBar.SetActive(true);
	}

	private void UpdateHealthBar(float healthAsPercentage, float currentHealth, float maxHealth)
	{
		mouseOverHealthBarFill.fillAmount = healthAsPercentage;
	}

	public void HideMouseOverHealthBar(CharacterStats stats)
	{
		stats.OnHealthChanged -= UpdateHealthBar;
		mouseOverHealthBar.SetActive(false);
	}

	public void HideToolTip()
	{
		itemToolTipText.text = "";
		itemToolTipUI.SetActive(false);
		fixedToolTipText.text = "";
		fixedToolTipUI.SetActive(false);
	}

	public void GameOver(bool win)
	{
		gameIsOver = true;
	}

	public void Pause()
	{
		if (gameIsPaused && !gameIsOver)
		{
			Resume();
		}
		else
		{
			PlayerManager.Instance.GetComponent<PlayerController>().enabled = false;
			pauseMenuUI.SetActive(true);
			Time.timeScale = 0f;
			gameIsPaused = true;
			PlayChooseSound();
			sceneMessageText.text = "";
		}
	}

	public void Resume()
	{
		if (gameIsPaused)
		{
			PlayChooseSound();
		}

		PlayerManager.Instance.GetComponent<PlayerController>().enabled = true;
		pauseMenuUI.SetActive(false);
		Time.timeScale = 1f;
		gameIsPaused = false;
	}

	public void Reload()
	{
		string currentSceneName = SceneManager.GetActiveScene().name;
		Destroy(GameManager.Instance.gameObject);
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}

	public void MainMenu()
	{
		PlayChooseSound();
		SceneManager.LoadScene(0);
	}

	public void LoadLevel(int sceneIndex)
	{
		StartCoroutine(LoadAsynchronously(sceneIndex));
	}

	private IEnumerator ShowMessageCoroutine(string message, float timeToShow, Color color)
	{
		sceneMessageText.text = message;
		sceneMessageText.color = color;
		sceneMessageTextGameObject.SetActive(true);
		
		while(sceneMessageTimer < timeToShow)
		{
			sceneMessageTimer += timeToShow;
			yield return new WaitForSeconds(timeToShow);
		}
		
		sceneMessageText.text = "";
		sceneMessageTextGameObject.SetActive(false);
	}

	public void ShowMessage(string message, Color? color = null, float timeToShow = 3)
	{
		sceneMessageTimer = 0;

		if (sceneMessageText.IsActive())
		{
			sceneMessageText.text = message;
			sceneMessageText.color = color ?? Color.red;
			return;
		}

		StartCoroutine(ShowMessageCoroutine(message, timeToShow, color ?? Color.red));
	}

	public void QuitGame()
	{
		PlayChooseSound();
		Application.Quit();
	}

	public void SaveGame()
	{
		PlayChooseSound();
		GameManager.Instance.SaveGame();
	}

	public void LoadGame()
	{
		PlayChooseSound();
		GameManager.Instance.LoadGame();
		StartCoroutine(LoadAsynchronously(GameManager.Instance.GetSceneIndex()));
	}

	private IEnumerator LoadAsynchronously(int sceneIndex)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		loadingScreen.SetActive(true);

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			loadBarSlider.value = progress;

			yield return null;
		}
	}
}

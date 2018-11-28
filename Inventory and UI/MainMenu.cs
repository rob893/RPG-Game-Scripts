using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour {

	public Texture2D cursor;
	public GameObject mainMenu;
	public GameObject optionsMenu;
	public GameObject helpMenu;
	public GameObject loadingScreen;
	public GameObject continueButton;
	public GameObject toolTip;
	public TextMeshProUGUI toolTipText;
	public Slider slider;
	public AudioClip selectSound;
	public AudioClip chooseSound;
	public Toggle enemyUpscaling;
	public Toggle easy;
	public Toggle normal;
	public Toggle hard;

	private AudioSource audioSource;

	private void Awake()
	{
		Cursor.SetCursor(cursor, new Vector2(0, 0), CursorMode.Auto);
		mainMenu.SetActive(true);
		optionsMenu.SetActive(false);
		helpMenu.SetActive(false);
		loadingScreen.SetActive(false);
		continueButton.SetActive(false);
		toolTip.SetActive(false);
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (GameManager.Instance.HasSavedGamed())
		{
			continueButton.SetActive(true);
		}

		if (PlayerPrefs.HasKey("enemyScaling"))
		{
			if(PlayerPrefs.GetInt("enemyScaling") == 1)
			{
				enemyUpscaling.isOn = true;
			}
			else
			{
				enemyUpscaling.isOn = false;
			}
		}

		if (PlayerPrefs.HasKey("difficulty"))
		{
			if (PlayerPrefs.GetInt("difficulty") == 0)
			{
				easy.isOn = true;
			}
			else if(PlayerPrefs.GetInt("difficulty") == 1)
			{
				normal.isOn = true;
			}
			else
			{
				hard.isOn = true;
			}
		}
		else
		{
			normal.isOn = true;
			SetNormal();
		}
	}

	public void LoadGame()
	{
		GameManager.Instance.LoadGame();
		StartCoroutine(LoadAsynchronously(GameManager.Instance.GetSceneIndex()));
	}

	public void NewGame()
	{
		Destroy(GameManager.Instance.gameObject);
		StartCoroutine(LoadAsynchronously(1));
	}

	private IEnumerator LoadAsynchronously(int sceneIndex)
	{
		audioSource.PlayOneShot(chooseSound);
		AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

		mainMenu.SetActive(false);
		optionsMenu.SetActive(false);
		loadingScreen.SetActive(true);

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress / 0.9f);
			slider.value = progress;

			yield return null;
		}
	}

	public void ShowOptions()
	{
		audioSource.PlayOneShot(chooseSound);
		helpMenu.SetActive(false);
		optionsMenu.SetActive(!optionsMenu.activeSelf);
	}

	public void ShowHelp()
	{
		audioSource.PlayOneShot(chooseSound);
		optionsMenu.SetActive(false);
		helpMenu.SetActive(!helpMenu.activeSelf);
	}

	public void PlaySelectSound()
	{
		audioSource.PlayOneShot(selectSound);
	}

	public void GoToSite()
	{
		Application.OpenURL("https://rwherber.com/Fireheart/#controls");
	}

	public void QuitGame()
	{
		audioSource.PlayOneShot(chooseSound);
		Application.Quit();
	}

	public void ToggleEnemyUpscaling()
	{
		GameManager.Instance.scaleEnemies = enemyUpscaling.isOn;

		if (enemyUpscaling.isOn)
		{
			PlayerPrefs.SetInt("enemyScaling", 1);
		}
		else
		{
			PlayerPrefs.SetInt("enemyScaling", 0);
		}
		audioSource.PlayOneShot(selectSound);
		PlayerPrefs.Save();
	}

	public void SetEasy()
	{
		GameManager.Instance.difficulty = Difficulty.Easy;
		audioSource.PlayOneShot(selectSound);
		PlayerPrefs.SetInt("difficulty", 0);

		PlayerPrefs.Save();
	}

	public void SetNormal()
	{
		GameManager.Instance.difficulty = Difficulty.Normal;
		audioSource.PlayOneShot(selectSound);
		PlayerPrefs.SetInt("difficulty", 1);

		PlayerPrefs.Save();
	}

	public void SetHard()
	{
		GameManager.Instance.difficulty = Difficulty.Hard;
		audioSource.PlayOneShot(selectSound);
		PlayerPrefs.SetInt("difficulty", 2);

		PlayerPrefs.Save();
	}

	public void HideToolTip()
	{
		toolTipText.text = "";
		toolTip.SetActive(false);
	}

	public void ShowUpscalingTooltip()
	{
		toolTipText.text = "<color=" + Utility.GetHexColor(HexColor.LightGray) + ">Enabling this option will cause all non-player characters to level up with the player keeping them a challenge no matter what level the player is." +
			" This option has no effect on non-player characters that are the same or higher level than the player.</color>";
		toolTip.SetActive(true);
	}

	public void ShowHardTooltip()
	{
		toolTipText.text = "<color=" + Utility.GetHexColor(HexColor.LightGray) + ">Selecting this option will cause all non-player characters to have 50% more health and deal 50% more damage.</color>";
		toolTip.SetActive(true);
	}

	public void ShowEasyTooltip()
	{
		toolTipText.text = "<color=" + Utility.GetHexColor(HexColor.LightGray) + ">Selecting this option will cause all non-player characters to have 50% less health and deal 50% less damage.</color>";
		toolTip.SetActive(true);
	}

	public void ShowNormalTooltip()
	{
		toolTipText.text = "<color=" + Utility.GetHexColor(HexColor.LightGray) + ">Selecting this option will cause all non-player characters to have no changes in their stats.</color>";
		toolTip.SetActive(true);
	}
}
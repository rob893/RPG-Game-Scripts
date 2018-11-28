using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;

	public GameObject player;
	public PlayerStats playerStats;
	public Dictionary<KeyCode, Ability> selectedAbilities = new Dictionary<KeyCode, Ability>();

	public delegate void OnAbilityChanged();
	public OnAbilityChanged onAbilityChangedCallback;

	[SerializeField] private AudioClip notEnoughMana;
	[SerializeField] private AudioClip cantCastThat;
	[SerializeField] private AudioClip levelUpSound;
	[SerializeField] private AudioClip deathMusic;
	[SerializeField] private GameObject levelUpGraphic;

	private AudioSource audioSource;

	private PlayerManager() { }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		player = gameObject;
		playerStats = player.GetComponent<PlayerStats>();
		audioSource = GetComponent<AudioSource>();
	}

	private void Start()
	{
		if (GameManager.Instance.dataToLoad)
		{
			GameManager.Instance.LoadDataToPlayer();
		}
	}

	public void LoadCurrentAbilities(Dictionary<KeyCode, int> currentAbilities, Dictionary<int, AbilityConfig> database)
	{
		foreach(KeyValuePair<KeyCode, int> entry in currentAbilities)
		{
			if (database.ContainsKey(entry.Value))
			{
				if(!selectedAbilities.ContainsKey(entry.Key) || entry.Value != selectedAbilities[entry.Key].GetAbilityId())
				{
					ChangeAbility(database[entry.Value], entry.Key);
				}
			}
		}
	}

	public void ChangeAbility(AbilityConfig abilityConfig, KeyCode key)
	{
		if (!selectedAbilities.ContainsKey(key))
		{
			Ability ability = abilityConfig.AttachAbilityTo(gameObject);
			selectedAbilities.Add(key, ability);
		}
		else
		{
			Destroy(selectedAbilities[key]);
			Ability ability = abilityConfig.AttachAbilityTo(gameObject);
			selectedAbilities[key] = ability;
		}
		if(onAbilityChangedCallback != null)
		{
			onAbilityChangedCallback.Invoke();
		}
	}

	public void EnableGodMode()
	{
		if(GetComponent<NavMeshAgent>().speed >= 30)
		{
			return;
		}

		playerStats.SetLevel(50);
		playerStats.armor.AddModifier(100);
		playerStats.attackPower.AddModifier(10000);
		playerStats.health.AddModifier(10000);
		playerStats.crit.AddModifier(50);
		playerStats.manaRegainPerSecond.AddModifier(50);
		playerStats.healthRegainPerFiveSeconds.AddModifier(50);
		
		GetComponent<NavMeshAgent>().speed += 10;

		UIManager.Instance.ShowMessage("God mode enabled!");
	}

	public void KillPlayer()
	{
		AudioManager.Instance.PausePlayOneShotResumeMusic(deathMusic);
		
		Camera.main.GetComponent<CameraRaycaster>().SetDefaultMouseCursor();
		Camera.main.GetComponent<CameraRaycaster>().enabled = false;
		player.GetComponent<NavMeshAgent>().enabled = false;
		player.GetComponent<CapsuleCollider>().enabled = false;
		player.GetComponent<PlayerController>().enabled = false;
		player.GetComponent<PlayerCombat>().enabled = false;
		player.GetComponent<PlayerStats>().enabled = false;
	}

	public void PlayAudioClip(int index)
	{
		if(index == 1)
		{
			audioSource.PlayOneShot(notEnoughMana);
		}
		else if(index == 2)
		{
			audioSource.PlayOneShot(cantCastThat);
		}
	}

	public void LevelUp()
	{
		AudioManager.Instance.PausePlayOneShotResumeMusic(levelUpSound);
		string levelUpMessage = "You have reached level " + playerStats.GetLevel() + "!\nYou have gained 10 health and 1 attack power!";

		if(playerStats.GetLevel() <= 8)
		{
			levelUpMessage += "\nYou have gained access to new abilities! Press 'N' to open the ability menu.";
		}

		UIManager.Instance.ShowMessage(levelUpMessage, Color.yellow);
		GameObject levelUpGraphicInstance = Instantiate(levelUpGraphic, player.transform.position + new Vector3(0, 1, 0), Quaternion.identity, player.transform);
		Destroy(levelUpGraphicInstance, 8);
	}
}

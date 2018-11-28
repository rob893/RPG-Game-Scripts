using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CharacterStats : MonoBehaviour {

	public delegate void OnHealthChange(float healthAsPercentage, float currentHealth, float maxHealth);
	public event OnHealthChange OnHealthChanged;

	public delegate void OnManaChange(float manaAsPercentage);
	public event OnManaChange OnManaChanged;

	public delegate void OnDamage(float damageAmount, float threatAmount, CharacterStats attacker);
	public event OnDamage OnDamaged;

	public delegate void OnDeath();
	public event OnDeath OnDead;

	public string characterName = "Placeholder";
	public Stat health;
	public Stat mana;
	public Stat attackPower;
	public Stat armor;
	public Stat crit;
	public Stat manaRegainPerSecond;
	public Stat healthRegainPerFiveSeconds;
	public Transform hand;
	public Dictionary<string, List<Effect>> activeEffects = new Dictionary<string, List<Effect>>();

	[SerializeField] protected int Level = 1;
	[SerializeField] protected CharacterType type = CharacterType.Humanoid;
	[SerializeField] protected float threatMultiplier = 1;
	[SerializeField] private AudioClip deathScream;
	[SerializeField] private AudioClip[] hurtSounds;
	[SerializeField] private int expWorth;

	protected float maxHealth;
	protected float maxMana;
	protected float manaRegain;
	protected float healthRegain;
	protected float currentHealth;
	protected float currentMana;
	protected Vector3 combatTextOffset;
	protected bool isDead;
	protected GameObject floatingCombatText;

	private UIManager uiManager;
	private Animator anim;
	private AudioSource audioSource;
	private int healthDifficultyMod = 0;
	private int attackPowerDifficultyMod = 0;


	#region Getters
	public float CurrentHealth
	{
		get
		{
			return currentHealth;
		}
	}

	public float CurrentMana
	{
		get
		{
			return currentMana;
		}
	}

	public float HealthAsPercentage
	{
		get
		{
			return currentHealth / maxHealth;
		}
	}

	public float GetThreatMultiplier()
	{
		return threatMultiplier;
	}

	public float ManaAsPercentage
	{
		get
		{
			return currentMana / maxMana;
		}
	}

	public bool IsDead
	{
		get
		{
			return isDead;
		}
	}

	public int GetLevel()
	{
		return Level;
	}

	public void SetLevel(int newLevel)
	{
		Level = newLevel;
	}
	#endregion

	protected virtual void Start()
	{
		health.onStatChanged += UpdateStats;
		mana.onStatChanged += UpdateStats;
		healthRegainPerFiveSeconds.onStatChanged += UpdateStats;
		manaRegainPerSecond.onStatChanged += UpdateStats;

		combatTextOffset = new Vector3(0, (GetComponent<CapsuleCollider>().height / 2) + 0.25f);
		uiManager = UIManager.Instance;
		floatingCombatText = uiManager.GetFloatingCombatText();
	
		anim = GetComponentInChildren<Animator>();
		audioSource = GetComponent<AudioSource>();

		if (GameManager.Instance.scaleEnemies && !(this is PlayerStats))
		{
			PlayerManager.Instance.playerStats.OnLeveledUp += ScaleStats;
			ScaleStatsToPlayerLevel(PlayerManager.Instance.playerStats.GetLevel());
		}

		if(GameManager.Instance.difficulty != Difficulty.Normal && !(this is PlayerStats))
		{
			ScaleToDifficulty();
		}
	}

	private void OnEnable()
	{
		ResetCharacter();
	}

	private void ResetCharacter()
	{
		if (GetComponent<CapsuleCollider>())
		{
			GetComponent<CapsuleCollider>().enabled = true;
		}

		if (GetComponent<NavMeshAgent>())
		{
			GetComponent<NavMeshAgent>().isStopped = false;
		}

		isDead = false;
		currentHealth = maxHealth;
		currentMana = maxMana;
		UpdateStats();

		StartCoroutine(AddHealthPoints());
		StartCoroutine(AddManaPoints());
	}

	public void TakeDamage(float damage, CharacterStats attacker, bool chancePlayHurtSound = true)
	{
		if (!isDead)
		{
			int rngRoll = Random.Range(-10, 11);
			float rngDamage = damage * ((float)rngRoll / 100);
			damage += Mathf.RoundToInt(rngDamage);
			
			
			bool crit = false;
			int critRoll = Random.Range(1, 101);
			if(critRoll <= attacker.crit.GetValue())
			{
				crit = true;
				damage *= 2;
			}

			damage -= (damage * (Mathf.Clamp((float)armor.GetValue(), 0, 80) / 100));
			damage = Mathf.Clamp(damage, 0, int.MaxValue);
			damage = Mathf.RoundToInt(damage);
			currentHealth -= damage;

			if(attacker is PlayerStats)
			{
				GameObject combatText = ObjectPooler.Instance.ActivatePooledObject(floatingCombatText, transform.position + combatTextOffset, Quaternion.identity);
				combatText.GetComponent<FloatingCombatText>().SetText((damage).ToString(), crit);
			}

			if (OnHealthChanged != null)
			{
				OnHealthChanged(HealthAsPercentage, currentHealth, maxHealth);
			}

			if(OnDamaged != null)
			{
				OnDamaged(damage, damage * attacker.GetThreatMultiplier(), attacker);
			}


			if (currentHealth <= 0)
			{
				Die();
				return;
			}

			if(!chancePlayHurtSound || hurtSounds == null || hurtSounds.Length < 1)
			{
				return;
			}

			if (crit)
			{
				audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
			}
			else if (Random.Range(0, 6) == 0)
			{
				audioSource.PlayOneShot(hurtSounds[Random.Range(0, hurtSounds.Length)]);
			}
		}
	}

	public void TakeHeal(float amount, CharacterStats healer)
	{
		if (!isDead)
		{
			int critRoll = Random.Range(1, 100);
			if (critRoll <= healer.crit.GetValue())
			{
				amount *= 2;
			}

			currentHealth += amount;

			if (currentHealth > maxHealth)
			{
				currentHealth = maxHealth;
			}

			if (OnHealthChanged != null)
			{
				OnHealthChanged(HealthAsPercentage, currentHealth, maxHealth);
			}
		}
	}

	public bool HasEnoughMana(float amount)
	{
		return currentMana >= amount;
	}

	private IEnumerator AddHealthPoints()
	{
		while (!isDead)
		{
			yield return new WaitForSeconds(5f);

			if(healthRegain > 0 && currentHealth < maxHealth)
			{
				currentHealth = Mathf.Clamp(currentHealth + healthRegain, 0, maxHealth);
				
				if (OnHealthChanged != null)
				{
					OnHealthChanged(HealthAsPercentage, currentHealth, maxHealth);
				}
			}
		}
	}

	private IEnumerator AddManaPoints()
	{
		while (!isDead)
		{
			yield return new WaitForSeconds(0.25f);

			if(currentMana < maxMana)
			{
				currentMana = Mathf.Clamp(currentMana + (manaRegain / 4), 0, maxMana);

				if (OnManaChanged != null)
				{
					OnManaChanged(ManaAsPercentage);
				}
			}
		}
	}

	public void UseMana(float amount)
	{
		currentMana = Mathf.Clamp(currentMana - amount, 0, maxMana);

		if(OnManaChanged != null)
		{
			OnManaChanged(ManaAsPercentage);
		}
	}

	private void ScaleStats()
	{
		ScaleStatsToPlayerLevel(PlayerManager.Instance.playerStats.GetLevel());
		UnscaleDifficulty();
		ScaleToDifficulty();
	}

	public void ScaleStatsToPlayerLevel(int playerLevel)
	{
		if(this is PlayerStats || playerLevel <= 1 || Level >= playerLevel)
		{
			return;
		}

		int modifier = playerLevel - Level;
		Level = playerLevel;
		health.AlterBaseValue(modifier * 10);
		attackPower.AlterBaseValue(modifier);

		UpdateStats();
	}

	private void ScaleToDifficulty()
	{
		if(this is PlayerStats || GameManager.Instance.difficulty == Difficulty.Normal)
		{
			return;
		}
		
		if(GameManager.Instance.difficulty == Difficulty.Hard)
		{
			healthDifficultyMod = Mathf.RoundToInt(health.GetValue() * 0.5f);
			attackPowerDifficultyMod = Mathf.RoundToInt(attackPower.GetValue() * 0.5f);
			health.AddModifier(healthDifficultyMod);
			attackPower.AddModifier(attackPowerDifficultyMod);
		}
		else if(GameManager.Instance.difficulty == Difficulty.Easy)
		{
			healthDifficultyMod = -Mathf.RoundToInt(health.GetValue() * 0.5f);
			attackPowerDifficultyMod = -Mathf.RoundToInt(attackPower.GetValue() * 0.5f);
			health.AddModifier(healthDifficultyMod);
			attackPower.AddModifier(attackPowerDifficultyMod);
		}

		UpdateStats();
	}

	private void UnscaleDifficulty()
	{
		if (this is PlayerStats || GameManager.Instance.difficulty == Difficulty.Normal)
		{
			return;
		}

		health.RemoveModifier(healthDifficultyMod);
		attackPower.RemoveModifier(attackPowerDifficultyMod);
		healthDifficultyMod = 0;
		attackPowerDifficultyMod = 0;
	}

	private void OnMouseEnter()
	{
		if (EventSystem.current.IsPointerOverGameObject() || this is PlayerStats)
		{
			return;
		}

		uiManager.ShowMouseOverHealthBar(characterName, HealthAsPercentage, this);
		uiManager.ShowFixedToolTip(GetToolTip());
	}

	private string GetToolTip()
	{
		string toolTip = "";

		if(gameObject.layer == (int)Layer.Attackable)
		{
			toolTip += "<color=red>" + characterName + "</color>";
		}
		else if(gameObject.layer == (int)Layer.Friendly)
		{
			toolTip += "<color=green>" + characterName + "</color>";
		}
		else
		{
			toolTip += characterName;
		}
		
		toolTip += "\nLevel " + Level + " " + type;

		//toolTip += "\nMax Health " + health.GetValue();
		//toolTip += "\nAttack Power " + attackPower.GetValue();

		return toolTip;
	}

	private void OnMouseExit()
	{
		if(this is PlayerStats)
		{
			return;
		}

		uiManager.HideMouseOverHealthBar(this);

		if (!EventSystem.current.IsPointerOverGameObject())
		{
			uiManager.HideToolTip();
		}
	}

	private void Die()
	{
		anim.SetTrigger("isDead");
		isDead = true;

		if(OnDead != null)
		{
			OnDead();
		}

		audioSource.PlayOneShot(deathScream);

		DeathLogic();
	}

	protected virtual void DeathLogic()
	{
		if (GetComponent<LootScript>())
		{
			GetComponent<LootScript>().DropLootBag();
		}

		if (GetComponent<CapsuleCollider>())
		{
			GetComponent<CapsuleCollider>().enabled = false;
		}

		if (GetComponent<NavMeshAgent>())
		{
			GetComponent<NavMeshAgent>().isStopped = true;
			GetComponent<NavMeshAgent>().enabled = false;
		}

		if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().detectCollisions = false;
		}

		PlayerManager.Instance.playerStats.GainExp(expWorth);
		PlayerManager.Instance.playerStats.OnLeveledUp -= ScaleStats;

		ObjectSaver[] saveObjects = GetComponentsInChildren<ObjectSaver>();
		foreach(ObjectSaver savedObject in saveObjects)
		{
			savedObject.UnsetParent();
		}

		Destroy(gameObject, 4);
	}

	private void UpdateStats()
	{
		if(maxHealth != health.GetValue())
		{
			currentHealth += health.GetValue() - maxHealth;
			maxHealth = health.GetValue();

			if (currentHealth > maxHealth)
			{
				currentHealth = maxHealth;
			}
		}
		
		if(maxMana != mana.GetValue())
		{
			currentMana += mana.GetValue() - maxMana;
			maxMana = mana.GetValue();

			if (currentMana > maxMana)
			{
				currentMana = maxMana;
			}
		}
		
		manaRegain = manaRegainPerSecond.GetValue();
		healthRegain = healthRegainPerFiveSeconds.GetValue();

		if (OnHealthChanged != null)
		{
			OnHealthChanged(HealthAsPercentage, currentHealth, maxHealth);
		}
	}

	private void OnDestroy()
	{
		ObjectSaver[] saveObjects = GetComponentsInChildren<ObjectSaver>();
		foreach (ObjectSaver savedObject in saveObjects)
		{
			savedObject.UnsetParent();
		}
	}

	private void OnDisable()
	{
		foreach(KeyValuePair<string, List<Effect>> entry in activeEffects)
		{
			foreach(Effect effect in entry.Value)
			{
				Destroy(effect);
			}
		}

		activeEffects.Clear();
	}

	private IEnumerator DeactivateCharacter()
	{
		yield return new WaitForSeconds(3);
		gameObject.SetActive(false);
	}
}

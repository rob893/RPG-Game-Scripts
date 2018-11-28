using UnityEngine;

public class PlayerStats : CharacterStats {

	public delegate void OnLevelUp();
	public event OnLevelUp OnLeveledUp;

	public delegate void OnCraftingSkillUp();
	public event OnLevelUp OnCraftingSkilledUp;

	public delegate void OnExpGain(int currentExp, int expToNextLevel);
	public event OnExpGain OnExpGained;

	private PlayerStatistics playerStatistics = new PlayerStatistics();
	private int craftingSkillLevel = 1;
	private int currentExp = 0;
	private int expToNextLevel = 100;
	

	#region Getters
	public int GetCurrentExp()
	{
		return currentExp;
	}

	public int GetExpToNextLevel()
	{
		return expToNextLevel;
	}

	public int GetCraftingSkillLevel()
	{
		return craftingSkillLevel;
	}

	public void AddCraftingSkillLevel()
	{
		if(craftingSkillLevel >= 100)
		{
			return;
		}

		craftingSkillLevel++;

		if(OnCraftingSkilledUp != null)
		{
			OnCraftingSkilledUp.Invoke();
		}
	}

	public PlayerStatistics GetPlayerStatistics()
	{
		return playerStatistics;
	}
	#endregion


	protected override void Start () {
		base.Start();

		if(!GameManager.Instance.dataToLoad)
		{
			SaveStats();
		}

		EquipmentManager.Instance.onEquipmentChanged += OnEquipmentChanged;
	}

	private void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
	{
		if (oldItem != null)
		{
			armor.RemoveModifier(oldItem.armorModifier);
			attackPower.RemoveModifier(oldItem.attackPowerModifier);
			health.RemoveModifier(oldItem.healthModifier);
			mana.RemoveModifier(oldItem.manaModifier);
			crit.RemoveModifier(oldItem.critChanceModifier);
			manaRegainPerSecond.RemoveModifier(oldItem.manaPerSecondModifier);
			healthRegainPerFiveSeconds.RemoveModifier(oldItem.percentHealthRegainPerFiveSecondsModifier);
		}

		if (newItem != null)
		{
			armor.AddModifier(newItem.armorModifier);
			attackPower.AddModifier(newItem.attackPowerModifier);
			health.AddModifier(newItem.healthModifier);
			mana.AddModifier(newItem.manaModifier);
			crit.AddModifier(newItem.critChanceModifier);
			manaRegainPerSecond.AddModifier(newItem.manaPerSecondModifier);
			healthRegainPerFiveSeconds.AddModifier(newItem.percentHealthRegainPerFiveSecondsModifier);
		}
	}

	public void GainExp(int amount)
	{
		int prevExp = currentExp;
		currentExp += amount;

		if(currentExp >= expToNextLevel)
		{
			LevelUp();

			int remainder = amount - (expToNextLevel - prevExp);
			
			currentExp = 0;
			expToNextLevel = expToNextLevel + 25;

			if(remainder > 0)
			{
				GainExp(remainder);
			}
		}

		if (OnExpGained != null)
		{
			OnExpGained(currentExp, expToNextLevel);
		}
	}

	private void LevelUp()
	{
		Level++;
		health.AlterBaseValue(10);
		TakeHeal(maxHealth, this);
		attackPower.AlterBaseValue(1);
		PlayerManager.Instance.LevelUp();

		if (OnLeveledUp != null)
		{
			OnLeveledUp.Invoke();
		}
	}

	protected override void DeathLogic()
	{
		PlayerManager.Instance.KillPlayer();
	}

	public void SaveStats()
	{
		playerStatistics.level = Level;
		playerStatistics.currentHealth = currentHealth;
		playerStatistics.currentExp = currentExp;
		playerStatistics.expToNextLevel = expToNextLevel;
		playerStatistics.currentMana = currentMana;
		playerStatistics.craftingSkill = craftingSkillLevel;
	}

	public void LoadSavedStats(PlayerStatistics ps)
	{
		playerStatistics = ps;

		//NOTE: Stats (armor and attack power) are being loading through the equipment manager by equipping the items. They are not loaded here other than base values.
		
		attackPower.AlterBaseValue(playerStatistics.level - 1);
		health.AlterBaseValue((playerStatistics.level - 1) * 10);
		Level = playerStatistics.level;
		craftingSkillLevel = playerStatistics.craftingSkill;
		currentHealth = playerStatistics.currentHealth;
		currentExp = playerStatistics.currentExp;
		expToNextLevel =  playerStatistics.expToNextLevel;
		currentMana = playerStatistics.currentMana;
		TakeHeal(0, this); //trigger UI update
		GainExp(0);

		if (Level > 1 && OnLeveledUp != null)
		{
			OnLeveledUp.Invoke();
		}
	}
}

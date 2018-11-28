using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerStatistics {
	public int level = 1;
	public int currentExp = 0;
	public int expToNextLevel = 1000;
	public float currentHealth;
	public float currentMana;
	public float maxHealth = 100;
	public float maxMana = 100;
	public float manaRegainPerSecond = 5;
	public Stat attackPower;
	public Stat armor;
	public int craftingSkill;

}

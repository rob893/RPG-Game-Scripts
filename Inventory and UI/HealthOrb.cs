using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthOrb : MonoBehaviour {

	[SerializeField] private Image orbFill;
	[SerializeField] private Image fillLip;

	private PlayerStats playerStats;
	

	private void Start()
	{
		playerStats = PlayerManager.Instance.playerStats;
		playerStats.OnHealthChanged += UpdateHealthOrb;
	}

	private void UpdateHealthOrb(float healthAsPercentage, float currentHealth, float maxHealth)
	{
		orbFill.fillAmount = healthAsPercentage;
		fillLip.fillAmount = healthAsPercentage + 0.01f;
	}
}

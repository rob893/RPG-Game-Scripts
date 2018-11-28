using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PowerOrb : MonoBehaviour {

	[SerializeField] private Image orbFill;
	[SerializeField] private Image fillLip;

	private PlayerStats playerStats;
	

	private void Start()
	{
		playerStats = PlayerManager.Instance.playerStats;
		playerStats.OnManaChanged += UpdatePowerOrb;
	}

	private void UpdatePowerOrb(float powerAsPercentage)
	{
		orbFill.fillAmount = powerAsPercentage;
		fillLip.fillAmount = powerAsPercentage + 0.01f;
	}
}

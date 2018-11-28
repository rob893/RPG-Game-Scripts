using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAbilitySlot : MonoBehaviour {

	public AbilityConfig ability;
	public Image unavailableIcon;
	public Image availableIcon;
	public KeyCode key;

	private PlayerManager playerManager;
	private UIManager uiManager;

	private void Start()
	{
		uiManager = UIManager.Instance;
		playerManager = PlayerManager.Instance;
		if(ability != null)
		{
			availableIcon.sprite = ability.icon;
			availableIcon.enabled = true;
		}
		
	}

	public void SelectAbility()
	{
		if(ability != null)
		{
			uiManager.PlaySelectSound();
			if (!playerManager.selectedAbilities.ContainsKey(key) || !playerManager.selectedAbilities[key].GetOnCooldown() || Input.GetKey(KeyCode.LeftShift))
			{
				if(ability.requiredLevel <= playerManager.playerStats.GetLevel() || Input.GetKey(KeyCode.LeftShift))
				{
					playerManager.ChangeAbility(ability, key);
				}
				else
				{
					uiManager.ShowMessage("You are not high enough level to use this ability yet!");
				}
			}
			else
			{
				uiManager.ShowMessage("You cannot change abilities when your current ability is on on cooldown!");
			}
		}
		
	}

	public void ShowToolTip()
	{
		if (ability != null)
		{
			uiManager.ShowFixedToolTip(ability.GetSOToolTip());
		}
	}
}

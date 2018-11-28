using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityBarSlot : MonoBehaviour {

	public Image offCDIcon;
	public Image icon;
	public KeyCode key;
	public Ability ability;

	private bool onCooldown = false;
	private bool onGCD = false;
	private bool hasAbility = false;
	private float gcd;
	private float gcdTimer;
	private float abilityCooldown;
	private float abilityTimer;
	private PlayerCombat playerCombat;
	private UIManager uiManager;

	private void Start()
	{
		if(!hasAbility)
		{
			ClearSlot();
		}
		
		uiManager = UIManager.Instance;
		playerCombat = PlayerManager.Instance.player.GetComponent<PlayerCombat>();
		playerCombat.OnAttack += TriggerGCD;
		gcd = playerCombat.globalCooldown;
		offCDIcon.fillAmount = 1;
	}

	private void Update()
	{
		if(hasAbility)
		{
			if (onGCD)
			{
				gcdTimer += Time.deltaTime;
			}

			if (onCooldown)
			{
				abilityTimer += Time.deltaTime;
			}

			if(onCooldown && onGCD && (abilityCooldown - abilityTimer < gcd - gcdTimer))
			{
				offCDIcon.fillAmount = gcdTimer / gcd;
				if(gcdTimer >= gcd)
				{
					gcdTimer = 0;
					onGCD = false;
				}
				if (abilityTimer >= abilityCooldown)
				{
					abilityTimer = 0;
					onCooldown = false;
				}
			}
			else if(onCooldown)
			{
				offCDIcon.fillAmount = abilityTimer / abilityCooldown;
				if(abilityTimer >= abilityCooldown)
				{
					abilityTimer = 0;
					onCooldown = false;
				}
				if (gcdTimer >= gcd)
				{
					gcdTimer = 0;
					onGCD = false;
				}
			}
			else if (onGCD)
			{
				offCDIcon.fillAmount = gcdTimer / gcd;
				if (gcdTimer >= gcd)
				{
					gcdTimer = 0;
					onGCD = false;
				}
			}
		}
	}

	public void AddAbilityToSlot(Ability newAbility)
	{
		ability = newAbility;
		abilityCooldown = newAbility.GetAbilityCooldown();
		newAbility.onAbilityUsedCallback += TriggerAbilityCooldown;
		icon.sprite = ability.GetIcon();
		offCDIcon.sprite = ability.GetIcon();
		offCDIcon.enabled = true;
		icon.enabled = true;
		hasAbility = true;
	}

	public void ClearSlot()
	{
		ability = null;
		hasAbility = false;

		icon.sprite = null;
		icon.enabled = false;

		offCDIcon.sprite = null;
		offCDIcon.enabled = false;
	}


	public void UseAbility()
	{
		if (hasAbility)
		{
			playerCombat.EnterQueue(ability);
		}
	}

	private void TriggerGCD()
	{
		gcdTimer = 0;
		onGCD = true;
	}

	private void TriggerAbilityCooldown()
	{
		abilityTimer = 0;
		onCooldown = true;
	}

	public void ShowToolTip()
	{
		if (hasAbility)
		{
			uiManager.ShowFixedToolTip(ability.GetToolTip());
		}
	}
}

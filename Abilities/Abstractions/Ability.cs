using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour {

	public delegate void OnAbilityUsed();
	public OnAbilityUsed onAbilityUsedCallback;

	protected AbilityConfig config;
	protected string abilityName;
	protected int abilityId;
	protected string description;
	protected Sprite icon = null;
	protected AnimationClip[] abilityAnimationClips;
	protected AudioSource audioSource;
	protected AudioClip[] voiceSounds;
	protected AudioClip initialSound;
	protected AudioClip secondarySound;
	protected List<EffectConfig> effects = new List<EffectConfig>();
	protected float attackPowerPercent;
	protected float abilityManaCost;
	protected float abilityRange;
	protected float abilityAnimationTime = 1.25f;
	protected float abilityCooldown = 1;
	protected float lastUseTime;
	protected float cooldownTimer;
	protected float remainingCooldown = 0;
	protected bool onCooldown = false;
	protected bool requiresTarget = false;
	protected CharacterStats myStats;
	protected CharacterStats targetedStats;
	protected CharacterCombat characterCombat;
	
	
	#region Getters

	public float GetRemainingCooldown()
	{
		return remainingCooldown;
	}

	public int GetAbilityId()
	{
		return abilityId;
	}

	public float GetManaCost()
	{
		return abilityManaCost;
	}

	public bool GetOnCooldown()
	{
		return onCooldown;
	}

	public float GetAbilityCooldown()
	{
		return abilityCooldown;
	}

	public float GetAbilityRange()
	{
		return abilityRange;
	}

	public float GetAbilityAnimationTime()
	{
		return abilityAnimationTime;
	}

	public string GetAbilityName()
	{
		return abilityName;
	}

	public string GetAbilityDescription()
	{
		return description;
	}

	public Sprite GetIcon()
	{
		return icon;
	}

	public bool RequiresTarget
	{
		get
		{
			return requiresTarget;
		}
	}

	public AbilityConfig GetAbilityConfig()
	{
		return config;
	}

	public AnimationClip[] GetAbilityAnimations()
	{
		return abilityAnimationClips;
	}


#endregion


	protected virtual void Start()
	{
		myStats = GetComponentInParent<CharacterStats>();
		characterCombat = GetComponentInParent<CharacterCombat>();
		audioSource = GetComponentInParent<AudioSource>();
		cooldownTimer = abilityCooldown;
		attackPowerPercent = attackPowerPercent / 100;
	}

	protected virtual void Update()
	{
		if (onCooldown)
		{
			cooldownTimer += Time.deltaTime;
			remainingCooldown = abilityCooldown - cooldownTimer;

			if (cooldownTimer >= abilityCooldown)
			{
				remainingCooldown = 0;
				onCooldown = false;
			}
		}
	}

	public virtual bool Use(CharacterStats targetStats)
	{
		if (!onCooldown && myStats.HasEnoughMana(abilityManaCost))
		{
			PlayAbilityAudioClip(1);

			if (PlayVoiceSound())
			{
				PlayAbilityAudioClip(3);
			}
			

			myStats.UseMana(abilityManaCost);
			targetedStats = targetStats;
			characterCombat.CallOnAttackEvent();
			characterCombat.TriggerGCD();
			onCooldown = true;
			cooldownTimer = 0;

			if(onAbilityUsedCallback != null)
			{
				onAbilityUsedCallback.Invoke();
			}
			
			UseHook();

			return true;
		}
		return false;
	}

	protected virtual void UseHook()
	{

	}

	public virtual void AnimationEvent()
	{
		
	}

	protected virtual bool PlayVoiceSound()
	{
		return true;
	}

	protected void PlayAbilityAudioClip(int index)
	{
		if(index == 1 && initialSound != null)
		{
			audioSource.PlayOneShot(initialSound);
		}
		else if(index == 2 && secondarySound != null)
		{
			audioSource.PlayOneShot(secondarySound);
		}
		else if(index == 3 && voiceSounds != null && voiceSounds.Length > 0)
		{
			audioSource.PlayOneShot(voiceSounds[Random.Range(0, voiceSounds.Length)]);
		}
	}

	public void AddEffect(EffectConfig effect)
	{
		effects.Add(effect);
	}

	public void RemoveEffect(EffectConfig effect)
	{
		if (effects.Contains(effect))
		{
			effects.Remove(effect);
		}
	}

	public virtual void SetConfig(AbilityConfig configToSet)
	{
		config = configToSet;
		abilityName = config.abilityName;
		abilityId = config.abilityId;
		icon = config.icon;
		description = config.description;
		abilityAnimationClips = config.abilityAnimationClips;
		voiceSounds = config.voiceSounds;
		initialSound = config.initialSound;
		secondarySound = config.secondarySound;
		abilityRange = config.abilityRange;
		abilityAnimationTime = config.abilityAnimationTime;
		abilityCooldown = config.abilityCooldown;
		abilityManaCost = config.manaCost;
		attackPowerPercent = config.attackPowerPercent;

		foreach(EffectConfig effect in config.effects)
		{
			effects.Add(effect);
		}
	}

	public void IncreaseAttackPowerPercent(float amount)
	{
		attackPowerPercent += amount;
	}

	public string GetToolTip()
	{
		string toolTip = "<b>" + abilityName + "</b><size=12>";

		if(abilityManaCost > 0)
		{
			toolTip += "\nMana Cost: " + abilityManaCost;
		}
		
		if(abilityRange > 0)
		{
			toolTip += "\nRange: " + abilityRange + "m";
		}

		if(abilityCooldown > 0)
		{
			toolTip += "\nCooldown: " + abilityCooldown + " seconds";
		}

		//if(effects.Count > 0)
		//{
		//	toolTip += "\nApplies: ";
		//	for(int i = 0; i < effects.Count; i++)
		//	{
		//		if(i == effects.Count - 1)
		//		{
		//			toolTip += effects[i].name;
		//		}
		//		else
		//		{
		//			toolTip += effects[i].name + ", ";
		//		}
		//	}
		//}

		toolTip += ToolTipAbilityEffects();

		toolTip += "\n<color=yellow>" + description + "</color>";
		
		return toolTip;
	}

	protected virtual string ToolTipAbilityEffects()
	{
		if(attackPowerPercent > 0)
		{
			int damage = (int)(attackPowerPercent * myStats.attackPower.GetValue());
			return "\nDamage: " + damage;
		}
		return null;
	}
}

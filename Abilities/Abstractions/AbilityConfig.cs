using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityConfig : ScriptableObject
{
	[Header("Spcial Ability General")]
	public int abilityId;
	public string abilityName;
	public Sprite icon = null;
	[TextArea]
	public string description;
	public int requiredLevel = 1;
	public float attackPowerPercent;
	public float manaCost = 10f;
	public AnimationClip[] abilityAnimationClips;
	public AudioClip[] voiceSounds;
	public AudioClip initialSound;
	public AudioClip secondarySound;
	public float abilityRange;
	public float abilityAnimationTime = 1;
	public float abilityCooldown = 1;
	public List<EffectConfig> effects;


	protected Ability behaviour;

	public abstract Ability GetBehaviourComponent(GameObject objectToAttachTo);

	public Ability AttachAbilityTo(GameObject objectToattachTo)
	{
		Ability behaviourComponent = GetBehaviourComponent(objectToattachTo);
		behaviourComponent.SetConfig(this);
		behaviour = behaviourComponent;
		return behaviour;
	}

	public void Use(CharacterStats target)
	{
		behaviour.Use(target);
	}

	public float GetManaCost()
	{
		return manaCost;
	}

	public Ability GetAbilityBehaviour()
	{
		return behaviour;
	}

	public string GetSOToolTip()
	{
		string toolTip = "<b>" + abilityName + "</b><size=12>";

		if(PlayerManager.Instance.playerStats.GetLevel() < requiredLevel)
		{
			toolTip += "\n<color=red>Requires level " + requiredLevel + "</color>";
		}

		if(manaCost > 0)
		{
			toolTip += "\nMana Cost: " + manaCost;
		}

		if (abilityRange > 0)
		{
			toolTip += "\nRange: " + abilityRange + "m";
		}

		if(abilityCooldown > 0)
		{
			toolTip += "\nCooldown: " + abilityCooldown + " seconds";
		}

		//if (effects.Count > 0)
		//{
		//	toolTip += "\nApplies: ";
		//	for (int i = 0; i < effects.Count; i++)
		//	{
		//		if (i == effects.Count - 1)
		//		{
		//			toolTip += effects[i].name;
		//		}
		//		else
		//		{
		//			toolTip += effects[i].name + ", ";
		//		}
		//	}
		//}

		if (attackPowerPercent > 0)
		{
			toolTip += "\nDamage: " + attackPowerPercent + "% of attack power";
		}

		toolTip += "\n<color=yellow>" + description + "</color>";

		return toolTip;
	}
}

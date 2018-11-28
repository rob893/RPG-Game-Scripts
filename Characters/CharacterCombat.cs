using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(CharacterAnimator))]
public class CharacterCombat : MonoBehaviour {

	public Ability currentAbility;
	public bool CanChangeAttacks;

	public bool InCombat { get; set; }
	public event System.Action OnAttack;

	protected float lastAttackTime;
	protected const float combatCooldown = 5f;
	protected CharacterAnimator characterAnimator;


	protected virtual void Start()
	{
		CanChangeAttacks = true;
		characterAnimator = GetComponent<CharacterAnimator>();
	}

	protected virtual void Update()
	{
		if(InCombat && Time.time - lastAttackTime > combatCooldown)
		{
			InCombat = false;
		}
	}

	public virtual void UseAbility(CharacterStats targetStats)
	{
		
		if (targetStats != null)
		{
			InCombat = true;
		}

		if (currentAbility.Use(targetStats))
		{
			lastAttackTime = Time.time;
		}
		
	}

	public void SetAbility(Ability newAbility)
	{
		if(newAbility != currentAbility)
		{	
			currentAbility = newAbility;
			characterAnimator.SetAnimationClipSet(newAbility.GetAbilityAnimations(), newAbility.GetAbilityAnimationTime());
		}
	}

	public void ForceSetAbility(Ability newAbility)
	{
		currentAbility = newAbility;
		characterAnimator.SetAnimationClipSet(newAbility.GetAbilityAnimations(), newAbility.GetAbilityAnimationTime());
	}

	public void CallOnAttackEvent()
	{
		if (OnAttack != null)
		{
			OnAttack();
		}
	}

	public virtual void TriggerAbilityAnimationEvent()
	{
		currentAbility.AnimationEvent();
	}

	public virtual void TriggerGCD()
	{

	}
}

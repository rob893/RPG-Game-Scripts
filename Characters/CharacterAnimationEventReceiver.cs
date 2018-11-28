using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterAnimationEventReceiver : MonoBehaviour {

	private CharacterCombat combat;

	private AudioSource audioSource;
	private AudioClip footStepR;
	private AudioClip footStepL;


	private void Start()
	{
		combat = GetComponentInParent<CharacterCombat>();
		CharacterAnimator characterAnimator = GetComponentInParent<CharacterAnimator>();
	
		audioSource = GetComponent<AudioSource>();
		footStepL = characterAnimator.leftFoot;
		footStepR = characterAnimator.rightFoot;

		audioSource.spatialBlend = 1;
		//audioSource.minDistance = 2;
		//audioSource.maxDistance = 50;

	}

	public void AttackHitEvent()
	{
		combat.TriggerAbilityAnimationEvent();
	}

	public void Hit()
	{
		combat.TriggerAbilityAnimationEvent();
	}

	public void Cast()
	{
		combat.TriggerAbilityAnimationEvent();
	}

	public void FootL()
	{
		audioSource.PlayOneShot(footStepL);
	}

	public void FootR()
	{
		audioSource.PlayOneShot(footStepR);
	}

	public void ActivateCharacterEffect()
	{
		
	}

	public void ActivateEffect()
	{
		combat.TriggerAbilityAnimationEvent();
	}

	public void ActivateAdditionalEffect()
	{
		
	}


	public void ActivateCharacterEffect2()
	{
		
	}
}

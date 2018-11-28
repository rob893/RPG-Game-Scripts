using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(CharacterAnimator))]
public class PlayerCombat : CharacterCombat {

	[SerializeField] float inputBuffer = 0.25f;
	public float globalCooldown = 1f;

	private Ability queuedAbility = null;
	private PlayerManager playerManager;
	private CharacterStats targetedStats;
	private PlayerStats playerStats;
	private float gcdTimer;
	private bool onGCD = false;


	protected override void Start()
	{
		base.Start();
		playerManager = PlayerManager.Instance;
		gcdTimer = globalCooldown;
		playerStats = GetComponent<PlayerStats>();
	}

	protected override void Update()
	{
		base.Update();

		if (onGCD)
		{
			gcdTimer += Time.deltaTime;

			if(gcdTimer >= globalCooldown)
			{
				CanChangeAttacks = true;
				onGCD = false;
			}
		}
		
		else if (queuedAbility != null && CanChangeAttacks)
		{
			SetAbility(queuedAbility);

			if (currentAbility.Use(targetedStats))
			{
				queuedAbility = null;
			}
		}
	}

	public void EnterQueue(Ability ability, CharacterStats targetStats = null)
	{
		if (playerStats.HasEnoughMana(ability.GetManaCost()))
		{

			if (ability.GetOnCooldown())
			{
				if (globalCooldown - gcdTimer < inputBuffer && ability.GetRemainingCooldown() < inputBuffer)
				{
					queuedAbility = ability;
					targetedStats = targetStats;
				}
				else
				{
					playerManager.PlayAudioClip(2);
				}
			}

			else
			{
				if (!onGCD)
				{
					queuedAbility = ability;
					targetedStats = targetStats;
				}
				else if (globalCooldown - gcdTimer < inputBuffer)
				{
					queuedAbility = ability;
					targetedStats = targetStats;
				}
			}
		}
		else
		{
			playerManager.PlayAudioClip(1);
		}
	}

	public void ClearQueuedAbility()
	{
		queuedAbility = null;
	}

	public override void TriggerAbilityAnimationEvent()
	{
		currentAbility.AnimationEvent();
		CanChangeAttacks = true;
	}

	public override void TriggerGCD()
	{
		CanChangeAttacks = false;
		lastAttackTime = Time.time;
		gcdTimer = 0;
		onGCD = true;
	}
}

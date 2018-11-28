using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCController))]
public class ChaseState : State {

	private NPCController npcController;
	private float attackRange;
	private float attackRangeOffset;
	private float lastCheckTime = 0;
	private NavMeshAgent agent;
	private Transform target;
	private CharacterStats targetStats;
	private CharacterAnimator animator;


	private void Start()
	{
		npcController = GetComponent<NPCController>();
		npcController.TargetChanged += ChangeTarget;
		agent = GetComponent<NavMeshAgent>();
		animator = GetComponent<CharacterAnimator>();
	}

	public override void PerformBehavior()
	{
		if(targetStats == null || targetStats.IsDead)
		{
			npcController.SetState(npcController.GetResetState());
			return;
		}

		if (Time.time - lastCheckTime > 0.75f)
		{
			lastCheckTime = Time.time;
			if (npcController.CanAttack())
			{
				agent.ResetPath();
				npcController.SetState(npcController.GetAttackState());
				return;
			}
		}

		if(Vector3.Distance(target.position, transform.position) > attackRange + attackRangeOffset)
		{
			agent.SetDestination(target.position);
		}
		else
		{
			agent.ResetPath();
			npcController.SetState(npcController.GetAttackState());
		}
	}

	protected void ChangeTarget(Transform newTarget, CharacterStats newStats)
	{
		target = newTarget;
		targetStats = newStats;
		attackRangeOffset = newTarget == null ? 0 : newTarget.GetComponent<CapsuleCollider>().radius;
	}

	public override void Initialize()
	{
		attackRange = npcController.GetCurrentAbility().GetAbilityRange() == 0 ? 3 : npcController.GetCurrentAbility().GetAbilityRange();
		agent.stoppingDistance = attackRange;
		agent.speed = npcController.GetRunSpeed();
		animator.SetIsWalking(false);
	}
}

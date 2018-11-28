using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterCombat))]
[RequireComponent(typeof(CharacterAnimator))]
[RequireComponent(typeof(CharacterStats))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DisableIfFarAway))]
public class NPCController : MonoBehaviour
{

	public delegate void OnTargetChange(Transform target, CharacterStats targetStats);
	public event OnTargetChange TargetChanged;

	[SerializeField] private float lookRadius = 10f;
	[SerializeField] private WaypointContainer patrolPath;
	[SerializeField] private AbilityConfig[] abilityConfigs;
	[SerializeField] private LayerMask targetedLayer;
	[SerializeField] private float globalCooldown = 1.5f;
	[SerializeField] private float walkSpeed = 2f;
	[SerializeField] private float runSpeed = 4f;

	private float patrolWaitTime = 5f;
	private Dictionary<CharacterStats, float> threatTable = new Dictionary<CharacterStats, float>();

	/*
		This class uses the state design pattern to control the enemy's AI. This is the client. All states are derived from abstract State class. Each state has a PerformBehavior() 
		which defines that State's behavior. The current state's performBehavior() method is called every update frame. 
	*/
	//States section
	private State currentState;

	private State searchingState;
	private State chaseState;
	private State attackState;
	private State resetState;
	//End States section

	private Transform target;
	private CharacterStats targetStats;
	private Ability currentAbility;
	private CharacterStats npcStats;
	private bool active = false;


	#region Getters
	public AbilityConfig[] GetAbilityConfigs()
	{
		return abilityConfigs;
	}

	public LayerMask GetTargetLayerMask()
	{
		return targetedLayer;
	}

	public float GetPatrolWaitTime()
	{
		return patrolWaitTime;
	}

	public float GetWalkSpeed()
	{
		return walkSpeed;
	}

	public float GetRunSpeed()
	{
		return runSpeed;
	}

	public float GetGlobalCooldown()
	{
		return globalCooldown;
	}

	public float GetLookRadius()
	{
		return lookRadius;
	}

	public Transform GetTarget()
	{
		return target;
	}

	public Ability GetCurrentAbility()
	{
		return currentAbility;
	}

	public WaypointContainer GetPatrolPath()
	{
		return patrolPath;
	}

	public State GetSearchingState()
	{
		return searchingState;
	}

	public State GetChaseState()
	{
		return chaseState;
	}

	public State GetAttackState()
	{
		return attackState;
	}

	public State GetResetState()
	{
		return resetState;
	}

	public State GetCurrentState()
	{
		return currentState;
	}

	public void SetPatrolPath(WaypointContainer path)
	{
		patrolPath = path;
	}

	public void SetPatrolWaitTime(float timeToWait)
	{
		patrolWaitTime = timeToWait;
	}

	public void SetCurrentAbility(Ability newAbility)
	{
		currentAbility = newAbility;
	}
	#endregion

	private void Start()
	{
		npcStats = GetComponent<CharacterStats>();
		npcStats.OnDamaged += UpdateThreatTable;
		npcStats.OnDead += Deactivate;
		
		StartCoroutine(LateStart());
	}

	private void Update()
	{
		if (active)
		{
			currentState.PerformBehavior();
		}
		else
		{
			if (currentState != null && !npcStats.IsDead)
			{
				active = true;
			}
		}
	}

	public void UpdateStateComponents()
	{
		searchingState = GetComponent<SearchingState>();
		chaseState = GetComponent<ChaseState>();
		attackState = GetComponent<AttackState>();
		resetState = GetComponent<ResetState>();
	}

	public void SetState(State nextState)
	{
		if (!npcStats.IsDead)
		{
			nextState.Initialize();
			currentState = nextState;
		}
	}

	public void SetTarget(Transform newTarget)
	{
		if(newTarget == null)
		{
			target = null;
			targetStats = null;

			if (TargetChanged != null)
			{
				TargetChanged(null, null);
			}
		}
		else
		{
			target = newTarget;
			if (target.GetComponent<CharacterStats>())
			{
				UpdateThreatTable(0, 0, target.GetComponent<CharacterStats>());

			}
			else
			{
				targetStats = null;
			}
		}
	}

	public bool CanAttack()
	{
		return (attackState as AttackState).SelectAbility(0);
	}

	private void UpdateThreatTable(float damageAmount, float threatAmount, CharacterStats attacker)
	{
		if (threatTable.ContainsKey(attacker))
		{
			threatTable[attacker] += threatAmount;
		}
		else
		{
			threatTable.Add(attacker, threatAmount);
		}

		if (targetStats == attacker)
		{
			return;
		}

		List<CharacterStats> removeList = new List<CharacterStats>();
		float topThreat = float.MinValue;
		CharacterStats newTarget = targetStats;
		foreach (KeyValuePair<CharacterStats, float> attackerEntry in threatTable)
		{
			if (attackerEntry.Key == null || attackerEntry.Key.IsDead)
			{
				removeList.Add(attackerEntry.Key);
				continue;
			}

			if (attackerEntry.Value > topThreat && !attackerEntry.Key.IsDead)
			{
				topThreat = attackerEntry.Value;
				newTarget = attackerEntry.Key;
			}
		}

		if (targetStats != newTarget)
		{
			targetStats = newTarget;
			target = targetStats.transform;
			if (TargetChanged != null)
			{
				TargetChanged(targetStats.transform, targetStats);
			}
		}

		foreach (CharacterStats nullTarget in removeList)
		{
			threatTable.Remove(nullTarget);
		}
	}

	public void ClearThreatTable()
	{
		threatTable.Clear();
	}

	private void Deactivate()
	{
		target = null;
		targetStats = null;
		currentState = null;
		active = false;
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		UpdateStateComponents();
		SetState(searchingState);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, lookRadius);
	}
}


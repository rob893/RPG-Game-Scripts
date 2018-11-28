using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class AttackState : State {

	private NPCController npcController;
	private Transform target;
	private CharacterStats myStats;
	private CharacterStats targetStats;
	private CharacterCombat characterCombat;
	private Ability currentAbility;
	private Ability[] abilities;
	private LayerMask targetedLayer;
	private float attackRange;
	private float attackRangeOffset;
	private float lastAttackTime;
	private float globalCooldown;
	private float lookRadius;
	

	private void Awake ()
	{
		npcController = GetComponent<NPCController>();
		myStats = GetComponent<CharacterStats>();
		characterCombat = GetComponent<CharacterCombat>();
		lastAttackTime = 0;

		foreach (AbilityConfig config in npcController.GetAbilityConfigs())
		{
			config.AttachAbilityTo(gameObject);
		}

		abilities = GetComponents<Ability>();
	}

	private void Start()
	{
		npcController.TargetChanged += ChangeTarget;
		StartCoroutine(LateStart());
	}

	public override void PerformBehavior()
	{
		if (targetStats == null || targetStats.IsDead)
		{
			
			if (targetedLayer != 1 << 12)
			{
				Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, targetedLayer);

				foreach (Collider collider in colliders)
				{
					if (collider.GetComponent<CharacterStats>() && !collider.GetComponent<CharacterStats>().IsDead)
					{
						npcController.SetTarget(collider.transform);
						target = collider.transform;
						targetStats = collider.GetComponent<CharacterStats>();
						return;
					}
				}
			}
			
			npcController.SetState(npcController.GetResetState());
			return;
		}

		float distance = Vector3.Distance(target.position, transform.position);
		if (distance <= attackRange + attackRangeOffset)
		{
			if (targetStats != null && Time.time - lastAttackTime >= globalCooldown)
			{
				int ran = Random.Range(0, abilities.Length);
				SelectAbility(ran);

				if(distance <= attackRange + attackRangeOffset)
				{
					characterCombat.UseAbility(targetStats);
					lastAttackTime = Time.time;
				}
			}

			FaceTarget();
		}
		else
		{
			npcController.SetState(npcController.GetChaseState());
		}
	}

	public override void Initialize()
	{
		globalCooldown = npcController.GetGlobalCooldown();
		targetedLayer = npcController.GetTargetLayerMask();
		lookRadius = npcController.GetLookRadius();
	}

	protected void ChangeTarget(Transform newTarget, CharacterStats newStats)
	{
		target = newTarget;
		targetStats = newStats;
		attackRangeOffset = newTarget == null ? 0 : newTarget.GetComponent<CapsuleCollider>().radius;
	}

	public bool SelectAbility(int abilityIndex)
	{
		if(target == null)
		{
			SetAbility(abilities[abilityIndex]);
			return false;
		}

		int i = 0; //prevent accidental inf loops.
		while(abilities[abilityIndex].GetRemainingCooldown() > globalCooldown || !myStats.HasEnoughMana(abilities[abilityIndex].GetManaCost()) ||
			(attackRange = abilities[abilityIndex].GetAbilityRange() == 0 ? 3 : abilities[abilityIndex].GetAbilityRange()) + attackRangeOffset < Vector3.Distance(target.position, transform.position))
		{
			i++;
			abilityIndex = (abilityIndex + 1) % abilities.Length;

			if (i > abilities.Length)
			{
				SetAbility(abilities[0]);
				return false;
			}
		}

		SetAbility(abilities[abilityIndex]);
		return true;
	}

	private void SetAbility(Ability newAbility)
	{
		currentAbility = newAbility;
		npcController.SetCurrentAbility(currentAbility);
		characterCombat.SetAbility(currentAbility);
		attackRange = currentAbility.GetAbilityRange() == 0 ? 3 : currentAbility.GetAbilityRange();
	}

	private void FaceTarget()
	{
		Vector3 direction = (target.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
		transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
	}

	private IEnumerator LateStart()
	{
		yield return new WaitForEndOfFrame();
		SelectAbility(0);
	}
}

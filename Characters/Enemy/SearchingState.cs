using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCController))]
public class SearchingState : State {

	protected LayerMask targetedLayer; //asdf
	protected NPCController npcController;
	protected CharacterAnimator animator;
	protected float lookRadius;
	protected float timer = 0;


	protected virtual void Start()
	{
		GetComponent<CharacterStats>().OnDamaged += Aggro;
		npcController = GetComponent<NPCController>();
		animator = GetComponent<CharacterAnimator>();
	}

	public override void PerformBehavior()
	{
		if(targetedLayer != 1 << 12)
		{
			timer += Time.deltaTime;

			if (timer >= 1)
			{
				Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, targetedLayer);
				if(colliders.Length > 0)
				{
					float shortestSqrDistance = Mathf.Infinity;
					Transform nearestTarget = null;
					Vector3 currentLoction = transform.position;

					foreach (Collider collider in colliders)
					{
						if (collider.GetComponent<CharacterStats>() && !collider.GetComponent<CharacterStats>().IsDead)
						{
							float sqrDistanceToTarget = (currentLoction - collider.transform.position).sqrMagnitude;
							if(sqrDistanceToTarget < shortestSqrDistance)
							{
								shortestSqrDistance = sqrDistanceToTarget;
								nearestTarget = collider.transform;
							}
						}
					}

					if (nearestTarget.GetComponent<CharacterStats>())
					{
						npcController.SetTarget(nearestTarget);
						npcController.SetState(npcController.GetChaseState());
					}
				}
				
				timer = 0;
			}
		}

		ExtraBehavior();
	}

	protected virtual void ExtraBehavior()
	{

	}

	private void Aggro(float damageAmount, float threatAmount, CharacterStats attacker)
	{
		if(npcController.GetCurrentState() == this && targetedLayer == (targetedLayer | (1 << attacker.gameObject.layer)))
		{
			npcController.SetTarget(attacker.transform);
			npcController.SetState(npcController.GetChaseState());
		}
	}

	public override void Initialize()
	{
		targetedLayer = npcController.GetTargetLayerMask();
		lookRadius = npcController.GetLookRadius();
		animator.SetIsWalking(true);
	}
}

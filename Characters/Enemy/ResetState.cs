using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCController))]
public class ResetState : State {

	private Vector3 startingLocation;
	private Quaternion initialRotation;
	private NavMeshAgent agent;
	protected NPCController npcController;
	private LayerMask targetedLayer;
	private bool rotating = false;
	private float timer = 0;
	private float lookRadius;
	

	private void Start()
	{
		startingLocation = transform.position;
		initialRotation = transform.rotation;
		agent = GetComponent<NavMeshAgent>();
		npcController = GetComponent<NPCController>();
	}

	public override void Initialize()
	{
		npcController.SetTarget(null);
		targetedLayer = npcController.GetTargetLayerMask();
		lookRadius = npcController.GetLookRadius();
		agent.stoppingDistance = 1;
		agent.SetDestination(startingLocation);
	}

	public override void PerformBehavior()
	{ 
		timer += Time.deltaTime;
		if (targetedLayer != 1 << 12)
		{
			if (timer >= 1)
			{
				Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, targetedLayer);
				if (colliders.Length > 0)
				{
					float shortestSqrDistance = Mathf.Infinity;
					Transform nearestTarget = null;
					Vector3 currentLoction = transform.position;

					foreach (Collider collider in colliders)
					{
						if (collider.GetComponent<CharacterStats>() && !collider.GetComponent<CharacterStats>().IsDead)
						{
							float sqrDistanceToTarget = (currentLoction - collider.transform.position).sqrMagnitude;
							if (sqrDistanceToTarget < shortestSqrDistance)
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
		
		if (!rotating && !agent.hasPath)
		{
			agent.SetDestination(startingLocation);
		}

		if (!rotating && !agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
				{
					rotating = true;
				}
			}
		}

		if (rotating)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * 5);
			if(Quaternion.Angle(transform.rotation, initialRotation) <= 0.25)
			{
				rotating = false;
				npcController.ClearThreatTable();
				npcController.SetState(npcController.GetSearchingState());
			}
		}
	}
}

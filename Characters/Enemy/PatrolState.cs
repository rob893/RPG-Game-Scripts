using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCController))]
public class PatrolState : SearchingState {

	private WaypointContainer patrolPath;
	private NavMeshAgent agent;
	private Vector3 nextWaypointPosition;
	private int nextWaypointIndex = 0;
	private float walkSpeed;
	private float waitTime;
	private float waitTimer = 0;
	private bool waiting = false;

	protected override void Start()
	{
		base.Start();
		npcController = GetComponent<NPCController>();
		agent = GetComponent<NavMeshAgent>();
	}

	public override void PerformBehavior()
	{

		timer += Time.deltaTime;
		
		if (timer >= 1)
		{
			if (targetedLayer != 1 << 12)
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
			}

			if (!waiting && Vector3.Distance(transform.position, nextWaypointPosition) <= agent.stoppingDistance + 0.5f)
			{
				waiting = true;
			}

			if (!waiting && !agent.hasPath)
			{
				agent.SetDestination(nextWaypointPosition);
			}

			timer = 0;
		}

		CycleWaypointWhenClose();
	}

	private void CycleWaypointWhenClose()
	{
		if (waiting)
		{
			waitTimer += Time.deltaTime;
			if(waitTimer >= waitTime)
			{
				nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
				nextWaypointPosition = patrolPath.transform.GetChild(nextWaypointIndex).position;
				agent.SetDestination(nextWaypointPosition);
				waiting = false;
				waitTimer = 0;
			}
		}
	}

	public override void Initialize()
	{
		base.Initialize();
		waitTimer = 0;
		waiting = false;
		patrolPath = npcController.GetPatrolPath();
		agent.speed = npcController.GetWalkSpeed();
		agent.stoppingDistance = 1;
		waitTime = npcController.GetPatrolWaitTime();
		nextWaypointPosition = patrolPath.transform.GetChild(nextWaypointIndex).position;
		agent.SetDestination(nextWaypointPosition);
	}
}

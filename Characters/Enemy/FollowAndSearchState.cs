using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowAndSearchState : SearchingState {

	public Transform followTarget;

	private NavMeshAgent agent;
	private Vector3 offset = new Vector3(3, 0, 0);

	protected override void Start()
	{
		base.Start();
		agent = GetComponent<NavMeshAgent>();
	}

	protected override void ExtraBehavior()
	{
		agent.SetDestination(followTarget.position + offset); //TODO optimize this a bit
	}

	public override void Initialize()
	{
		base.Initialize();
		animator.SetIsWalking(false);
	}
}

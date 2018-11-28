using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PlayerMotor : MonoBehaviour {

	private NavMeshAgent agent;
	private Transform target;
	private Vector3 targetLastPosition = new Vector3();

	private void Start () {
		agent = GetComponent<NavMeshAgent>();
	}

	private void Update()
	{	
		if(target != null)
		{
			if(target.position != targetLastPosition)
			{
				targetLastPosition = target.position;
				agent.SetDestination(target.position);
			}
		}
	}

	public void MoveToPoint(Vector3 point)
	{
		agent.stoppingDistance = 1.1f;
		agent.SetDestination(point);
	}

	public void SetTarget(Transform targetTransform)
	{
		target = targetTransform;
	}


	public void StopFollowingTarget()
	{
		targetLastPosition = Vector3.zero;
		target = null;
		agent.ResetPath();
	}

	public void StartFaceTargetCoroutine(Transform newTarget, float timeToFace = 2)
	{
		StartCoroutine(FaceTargetCoroutine(newTarget, timeToFace));
	}

	private IEnumerator FaceTargetCoroutine(Transform targetTrans, float timeToFace)
	{
		float timer = 0;
		while (timer < timeToFace)
		{
			timer += Time.deltaTime;
			FaceTarget(targetTrans);
			yield return new WaitForEndOfFrame();
		}
	}

	private void FaceTarget(Transform targetTrans)
	{
		if(targetTrans != null)
		{
			Vector3 direction = (targetTrans.position - transform.position).normalized;
			Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
		}
	}
}

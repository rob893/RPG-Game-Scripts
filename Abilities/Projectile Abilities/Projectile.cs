using UnityEngine;

//Attach this to the projectile effect to make it move
public class Projectile : MonoBehaviour {

	public float speed = 30f;
	public GameObject impactEffect;
	public Vector3 offSet = new Vector3(0, 10, 0);

	public TargetedRangedAbility Attack { get; set; }
	public CharacterStats TargetStats { get; set; }

	private Vector3 prevPos;


	private void Start()
	{
		prevPos = transform.position;
	}

	private void Update()
	{
		if (TargetStats != null && TargetStats.CurrentHealth > 0)
		{
			MoveToTarget();
		}
		else
		{
			MoveStraight();
		}
	}

	private void MoveToTarget()
	{
		Vector3 dir = (TargetStats.transform.position + offSet) - transform.position;
		float distanceThisFrame = speed * Time.deltaTime;

		if (dir.magnitude <= distanceThisFrame)
		{
			HitTarget(TargetStats);
		}
		transform.Translate(dir.normalized * distanceThisFrame, Space.World);
	}

	private void MoveStraight()
	{
		float distanceThisFrame = speed * Time.deltaTime;

		RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude , 1 << 10);
		foreach(RaycastHit hit in hits)
		{
			if (hit.collider.GetComponent<CharacterStats>() != null)
			{
				HitTarget(hit.collider.GetComponent<CharacterStats>());
				break;
			}
		}

		transform.position += transform.forward * distanceThisFrame;
	}

	private void HitTarget(CharacterStats collidedStats)
	{
		if (impactEffect != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(impactEffect, transform.position, transform.rotation);
		}

		if(Attack != null)
		{
			Attack.OnCollision(collidedStats);
		}
		
		gameObject.SetActive(false);
	}


	
}

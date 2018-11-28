using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Spell_Tornado : MonoBehaviour
{
	public float pullForce;
	public float pullRadius;
	public float spellDamage;
	public float spellDuration;
	public float spellRadius;

	private float timer;
	private float durationTimer;
	private List<Rigidbody> rigidBodies;


	private void Start()
	{
		rigidBodies = new List<Rigidbody>();
	}

	private void OnEnable()
	{
		GetComponent<ParticleSystemScript>().Lifetime = spellDuration;
		StartCoroutine(TurnOnKinematic(spellDuration - 0.25f));
		durationTimer = 0;
		timer = 0;
	}

	private void FixedUpdate()
	{
		durationTimer += Time.deltaTime;
		timer += Time.deltaTime;

		if (timer >= 1)
		{
			TornadoEffect(transform.position, spellRadius);
		}

		if(durationTimer < spellDuration - 0.5f)
		{
			foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius, 1 << 10))
			{
				Vector3 forceDirection = transform.position - collider.transform.position;
				Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();
				targetRigidbody.isKinematic = false;
				targetRigidbody.AddForce(forceDirection * pullForce * Time.fixedDeltaTime);
				if (!rigidBodies.Contains(targetRigidbody))
				{
					rigidBodies.Add(targetRigidbody);
				}
			}
		}
		
		//Turn this on if you want the tornado to affect environment objects in the PhysicsAffects Layer.
		/*foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius, LayerMask.GetMask("PhysicsAffects")))
		{
			Vector3 forceDirection = transform.position - collider.transform.position;
			Rigidbody targetRigidbody = collider.GetComponent<Rigidbody>();
			targetRigidbody.AddForce(forceDirection * pullForce * Time.fixedDeltaTime);
		}*/
	}

	private void TornadoEffect(Vector3 center, float radius)
	{
		timer = 0f;
		Collider[] hitColliders = Physics.OverlapSphere(center, radius, 1 << 10);
		int i = 0;
		while (i < hitColliders.Length)
		{
			hitColliders[i].GetComponent<CharacterStats>().TakeDamage(spellDamage, PlayerManager.Instance.playerStats);
			i++;
		}
	}

	private IEnumerator TurnOnKinematic(float delay)
	{
		yield return new WaitForSeconds(delay);
		foreach(Rigidbody rb in rigidBodies)
		{
			if(rb != null) //delete this if using object pool. Want to enable kine if object is not active
			{
				rb.isKinematic = true;
			}
		}

		rigidBodies.Clear();
	}
}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Trap : MonoBehaviour {

	public float damage;
	public float damageRadius;
	public float armTime = 3;
	public CharacterStats myStats;
	public LayerMask hitLayers;
	public GameObject trapTriggerGraphic;
	public GameObject initialHitGraphic;
	public Vector3 trapTriggerGraphicOffset = new Vector3(0, 0.5f, 0);
	public List<EffectConfig> effects = new List<EffectConfig>();

	private bool armed = false;

	private void Start()
	{
		GetComponent<SphereCollider>().isTrigger = true;
	}

	private void OnEnable()
	{
		armed = false;
		StartCoroutine(ArmTrap());
	}

	private IEnumerator ArmTrap()
	{
		yield return new WaitForSeconds(armTime);

		armed = true;

		if(Physics.OverlapSphere(transform.position, damageRadius, hitLayers).Length > 0)
		{
			SpringTrap();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!armed)
		{
			return;
		}

		if(((1 << other.gameObject.layer) & hitLayers) != 0)
		{
			SpringTrap();
		}
	}

	private void SpringTrap()
	{
		if (trapTriggerGraphic != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(trapTriggerGraphic, transform.position + trapTriggerGraphicOffset, transform.rotation, 3);
		}

		if(myStats == null)
		{
			myStats = PlayerManager.Instance.playerStats; //figure out a way to do this better. World placed traps need a stats
		}

		Collider[] colliders = Physics.OverlapSphere(transform.position, damageRadius, hitLayers);
		foreach (Collider collider in colliders)
		{
			if (collider.GetComponent<CharacterStats>())
			{
				CharacterStats collidedStats = collider.GetComponent<CharacterStats>();

				if (initialHitGraphic != null)
				{
					ObjectPooler.Instance.ActivatePooledObject(initialHitGraphic, collidedStats.transform.position, transform.rotation, 3);
				}

				if (damage > 0)
				{
					collidedStats.TakeDamage(damage, myStats);
				}

				foreach (EffectConfig effect in effects)
				{
					Effect effectInstance = effect.AttachEffectTo(collidedStats.gameObject);
					effectInstance.MyStats = myStats;
					effectInstance.TheirStats = collidedStats;
				}
			}
		}

		DisableTrap();
	}

	private void DisableTrap()
	{
		armed = false;
		gameObject.SetActive(false);
	}
}

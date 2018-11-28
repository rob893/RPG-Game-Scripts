using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChanneledAbility : Ability {

	protected GameObject spawnedEffect;
	protected float effectRadius;
	protected float tickFreq;
	protected Transform spawnPoint;
	protected GameObject tickPrefabGraphic;
	protected CameraRaycaster cameraRaycaster;
	protected bool channeling = false;
	protected float timer = 0;
	protected GameObject effectInstance;
	protected PlayerController playerController;
	protected PlayerMotor playerMotor;
	protected Animator animator;
	


	protected override void Start()
	{
		base.Start();
		cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
		playerController = GetComponent<PlayerController>();
		playerMotor = GetComponent<PlayerMotor>();
		animator = GetComponentInChildren<Animator>();
	}

	protected override void Update()
	{
		base.Update();
		if (channeling)
		{
			if (Input.GetKey(KeyCode.Mouse1))
			{
				TargetingLogic();
				timer += Time.deltaTime;
				if(timer >= tickFreq)
				{
					if (myStats.HasEnoughMana(abilityManaCost))
					{
						timer = 0;
						myStats.UseMana(abilityManaCost);
						AbilityLogic();
					}
					else
					{
						timer = 0;
						channeling = false;
						effectInstance.SetActive(false);
						effectInstance = null;
						animator.SetBool("channeling", false);
						StartCoroutine(StopChanneling());
						PlayerManager.Instance.PlayAudioClip(1);
					}
				}
			}
			else
			{
				timer = 0;
				channeling = false;
				effectInstance.SetActive(false);
				effectInstance = null;
				animator.SetBool("channeling", false);
				StartCoroutine(StopChanneling());
			}
		}
	}


	protected override void UseHook()
	{
		cameraRaycaster.enabled = false;
		channeling = true;
		playerController.enabled = false;
		spawnPoint = myStats.hand;
		effectInstance = ObjectPooler.Instance.ActivatePooledObject(spawnedEffect, spawnPoint.position + new Vector3(0, 1.5f, 0), Quaternion.identity, 0, transform);
		animator.SetBool("channeling", true);
		playerMotor.MoveToPoint(transform.position);
		AbilityLogic();
	}

	protected virtual void TargetingLogic()
	{

	}

	protected virtual void AbilityLogic()
	{

	}

	private IEnumerator StopChanneling()
	{
		yield return new WaitForSeconds(0.1f);
		cameraRaycaster.enabled = true;
		characterCombat.CanChangeAttacks = true;
		playerController.enabled = true;
	}
}

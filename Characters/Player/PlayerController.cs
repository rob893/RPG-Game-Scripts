using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(CharacterCombat))]
public class PlayerController : MonoBehaviour {

	
	[SerializeField] private AbilityConfig rightClickAbility;
	[SerializeField] private AbilityConfig leftClickAbility;
	[SerializeField] private AbilityConfig ab1Ability;
	[SerializeField] private AbilityConfig ab2Ability;
	[SerializeField] private AbilityConfig ab3Ability;
	[SerializeField] private AbilityConfig ab4Ability;
	[SerializeField] private AbilityConfig ab5Ability;

	private KeyCode[] keys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5 };
	private PlayerMotor motor;
	private PlayerCombat playerCombat;
	private PlayerManager playerManager;
	private Dictionary<KeyCode, Ability> currentAbilities;


	private void Start () {
		Camera cam = Camera.main;
		CameraRaycaster cameraRaycaster = cam.GetComponent<CameraRaycaster>();
		cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
		cameraRaycaster.onMouseOverInteractable += OnMouseOverInteractable;
		cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;

		motor = GetComponent<PlayerMotor>();
		playerCombat = GetComponent<PlayerCombat>();
		playerManager = PlayerManager.Instance;
		currentAbilities = playerManager.selectedAbilities;

		LoadInitialAbilities();
	}

	private void Update()
	{
		foreach(KeyCode key in keys)
		{
			if (Input.GetKeyUp(key))
			{
				if (currentAbilities.ContainsKey(key))
				{
					StopAllCoroutines();
					motor.StopFollowingTarget();
					playerCombat.EnterQueue(currentAbilities[key]);
				}
			}
		}

		if (Input.GetMouseButtonDown(1) && !currentAbilities[KeyCode.Mouse1].RequiresTarget)
		{
			StopAllCoroutines();
			motor.StopFollowingTarget();
			playerCombat.EnterQueue(currentAbilities[KeyCode.Mouse1]);
		}

		if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			PlayerManager.Instance.EnableGodMode();
		}
	}

	private void OnMouseOverPotentiallyWalkable(Vector3 destination)
	{
		if (Input.GetMouseButton(0) && enabled)
		{
			StopAllCoroutines();
			playerCombat.ClearQueuedAbility();
			motor.StopFollowingTarget();
			motor.MoveToPoint(destination);
		}
	}

	private void OnMouseOverInteractable(Interactable interactable)
	{
		if (Input.GetMouseButtonUp(0) && enabled)
		{
			StopAllCoroutines();
			playerCombat.ClearQueuedAbility();

			if (IsTargetInRange(interactable.gameObject, interactable.radius * 0.8f))
			{
				interactable.Interact(transform);
				motor.StopFollowingTarget();
				motor.StartFaceTargetCoroutine(interactable.transform, 0.5f);
			}
			else
			{
				StartCoroutine(MoveAndInteract(interactable, interactable.radius * 0.8f));
			}
		}
	}

	private void OnMouseOverEnemy(CharacterStats enemy)
	{
		if (!enabled)
		{
			return;
		}

		if (currentAbilities[KeyCode.Mouse1].RequiresTarget)
		{
			if (Input.GetMouseButtonDown(1) && !enemy.IsDead && IsTargetInRange(enemy.gameObject, currentAbilities[KeyCode.Mouse1].GetAbilityRange()))
			{
				motor.StopFollowingTarget();
				motor.StartFaceTargetCoroutine(enemy.transform, 0.5f);
				playerCombat.EnterQueue(currentAbilities[KeyCode.Mouse1], enemy);
			}
			else if (Input.GetMouseButtonDown(1) && !enemy.IsDead && !IsTargetInRange(enemy.gameObject, currentAbilities[KeyCode.Mouse1].GetAbilityRange()))
			{
				motor.StopFollowingTarget();
				StopAllCoroutines();
				StartCoroutine(MoveAndAttack(enemy, currentAbilities[KeyCode.Mouse1]));
			}
		}

		if (Input.GetMouseButtonDown(0) && !enemy.IsDead && IsTargetInRange(enemy.gameObject, currentAbilities[KeyCode.Mouse0].GetAbilityRange() + enemy.GetComponent<CapsuleCollider>().radius))
		{
			motor.StopFollowingTarget();
			motor.StartFaceTargetCoroutine(enemy.transform, 0.5f);
			playerCombat.EnterQueue(currentAbilities[KeyCode.Mouse0], enemy);
		}
		else if (Input.GetMouseButtonDown(0) && !enemy.IsDead && !IsTargetInRange(enemy.gameObject, currentAbilities[KeyCode.Mouse0].GetAbilityRange() + enemy.GetComponent<CapsuleCollider>().radius))
		{
			motor.StopFollowingTarget();
			StopAllCoroutines();
			StartCoroutine(MoveAndAttack(enemy, currentAbilities[KeyCode.Mouse0]));
		}
	}

	private bool IsTargetInRange(GameObject target, float range)
	{
		float distanceToTarget = (target.transform.position - transform.position).magnitude;
		return distanceToTarget <= range;
	}

	private IEnumerator MoveToTarget(GameObject target, float range)
	{
		motor.SetTarget(target.transform);
		while (!IsTargetInRange(target, range))
		{
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForEndOfFrame();
	}

	private IEnumerator MoveAndAttack(CharacterStats enemy, Ability ability)
	{
		yield return StartCoroutine(MoveToTarget(enemy.gameObject, ability.GetAbilityRange() + enemy.GetComponent<CapsuleCollider>().radius));
		motor.StopFollowingTarget();
		motor.StartFaceTargetCoroutine(enemy.transform, 0.5f);
		playerCombat.EnterQueue(ability, enemy);
	}

	private IEnumerator MoveAndInteract(Interactable interactable, float interactRange)
	{
		yield return StartCoroutine(MoveToTarget(interactable.gameObject, interactRange));
		motor.StopFollowingTarget();
		motor.StartFaceTargetCoroutine(interactable.transform, 0.5f);
		interactable.Interact(transform);
	}

	private void LoadInitialAbilities()
	{
		if (rightClickAbility != null) playerManager.ChangeAbility(rightClickAbility, KeyCode.Mouse1);
		if (leftClickAbility != null) playerManager.ChangeAbility(leftClickAbility, KeyCode.Mouse0);
		if (ab1Ability != null) playerManager.ChangeAbility(ab1Ability, KeyCode.Alpha1);
		if (ab2Ability != null) playerManager.ChangeAbility(ab2Ability, KeyCode.Alpha2);
		if (ab3Ability != null) playerManager.ChangeAbility(ab3Ability, KeyCode.Alpha3);
		if (ab4Ability != null) playerManager.ChangeAbility(ab4Ability, KeyCode.Alpha4);
		if (ab5Ability != null) playerManager.ChangeAbility(ab5Ability, KeyCode.Alpha5);
	}
}

using System.Linq;
using UnityEngine;

public class ReticalSpawnedAbility : Ability {

	public GameObject spawnedEffect;
	public float effectRadius;
	public float effectDuration;

	protected Camera cam;
	protected float range = 25f;
	protected PlayerController playerController;
	protected Vector3 spawnPoint;
	protected bool targeting = false;
	protected Projector targetingCircle;

	private Vector3 projectorOffset = new Vector3(0, 50, 0);


	protected override void Start()
	{
		base.Start();
		targetingCircle = GameManager.Instance.GetComponentInChildren<Projector>();
		targetingCircle.enabled = false;
		cam = Camera.main;
		playerController = GetComponent<PlayerController>();
	}

	protected override void Update()
	{
		base.Update();
		if (targeting)
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;


			if (Physics.Raycast(ray, out hit, 1500, 1 << 9))
			{
				float distance = Vector3.Distance(transform.position, hit.point);
				if (distance <= range)
				{
					targetingCircle.transform.position = hit.point + projectorOffset;
					spawnPoint = hit.point;
				}
				else
				{
					Vector3 direction = (hit.point - transform.position).normalized;
					
					targetingCircle.transform.position = (transform.position + (range * direction)) + projectorOffset;
					spawnPoint = transform.position + (range * direction);
				}
			}

			if (Input.GetKeyDown(KeyCode.Mouse0))
			{
				Vector3 direction = (spawnPoint - transform.position).normalized;
				Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
				transform.rotation = lookRotation;
				myStats.UseMana(abilityManaCost);
				targeting = false;
				targetingCircle.enabled = false;
				onAbilityUsedCallback.Invoke();
				cooldownTimer = 0;
				onCooldown = true;
				characterCombat.CallOnAttackEvent();
				characterCombat.TriggerGCD();
			}
			else if (!Input.GetButton("Horizontal") && !Input.GetMouseButton(0) && Input.anyKeyDown)
			{
				StopTargeting();
				characterCombat.CanChangeAttacks = true;
			}
		}
	}

	public override bool Use(CharacterStats targetStats)
	{
		if (!onCooldown && myStats.HasEnoughMana(abilityManaCost))
		{
			playerController.enabled = false;
			targeting = true;

			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000, 1 << 9))
			{
				targetingCircle.transform.position = hit.point + new Vector3(0, 100, 0);
			}
		
			targetingCircle.orthographicSize = effectRadius;
			targetingCircle.enabled = true;
			return true;
		}
		
		return false;
	}

	public override void AnimationEvent()
	{
		playerController.enabled = true;
	}

	protected void StopTargeting()
	{
		targeting = false;
		playerController.enabled = true;
		targetingCircle.enabled = false;
	}
}

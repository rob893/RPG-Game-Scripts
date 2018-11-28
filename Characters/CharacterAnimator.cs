using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

	public AudioClip leftFoot;
	public AudioClip rightFoot;

	[SerializeField] protected AnimationClip replacementAttackAnimationClip;

	protected AnimationClip[] currentAttackAnimSet;
	protected Animator animator;
	protected CharacterCombat combat;
	protected AnimatorOverrideController overrideController;
	protected float abilityCastTime = 1;
	protected bool isWalking = false;

	private AudioSource audioSource;
	private NavMeshAgent agent;
	private float speed;
	private float dampTime = 0.1f;


	protected virtual void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		audioSource = GetComponent<AudioSource>();
		animator = GetComponentInChildren<Animator>();
		combat = GetComponent<CharacterCombat>();

		audioSource.spatialBlend = 1;
		audioSource.minDistance = 4;
		audioSource.maxDistance = 500;

		if(overrideController == null)
		{
			overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
		}
		
		animator.runtimeAnimatorController = overrideController;

		combat.OnAttack += OnAttack;
	}

	protected virtual void Update()
	{
		if (!isWalking)
		{
			speed = Mathf.Clamp(agent.velocity.magnitude / agent.speed, 0, 1);
			animator.SetFloat("speedPercent", speed, dampTime, Time.deltaTime);

		}
		else
		{
			speed = Mathf.Clamp(agent.velocity.magnitude / agent.speed, 0, 0.5f);
			animator.SetFloat("speedPercent", speed, 0.1f, Time.deltaTime);
		}
	}

	public virtual void OnAttack()
	{
		int attackIndex = Random.Range(0, currentAttackAnimSet.Length);
		overrideController[replacementAttackAnimationClip.name] = currentAttackAnimSet[attackIndex];
		float attackAnimLength = currentAttackAnimSet[attackIndex].length;
		animator.SetFloat("attackSpeed", attackAnimLength / abilityCastTime);
		animator.SetTrigger("attack");

	}

	public void SetAnimationClipSet(AnimationClip[] clipSet, float newAbilityCastTime)
	{
		abilityCastTime = newAbilityCastTime;
		currentAttackAnimSet = clipSet;
	}

	public void SetIsWalking(bool isWalkingValue)
	{
		isWalking = isWalkingValue;
	}
}

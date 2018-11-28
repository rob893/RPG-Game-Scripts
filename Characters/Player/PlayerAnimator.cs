using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : CharacterAnimator {

	public AnimationClip defaultIdle;
	public AnimationClip defaultWalk;
	public AnimationClip defaultRun;


	protected override void Start()
	{
		base.Start();
		EquipmentManager.Instance.OnWeaponChanged += SetLocomotionAnimation;
	}

	private void SetLocomotionAnimation(Weapon newWeapon)
	{
		if(newWeapon != null)
		{
			if (newWeapon.idleClip != null)
			{
				overrideController[defaultIdle] = newWeapon.idleClip;
			}
			else
			{
				overrideController[defaultIdle] = defaultIdle;
			}

			if (newWeapon.walkClip != null)
			{
				overrideController[defaultWalk] = newWeapon.walkClip;
			}
			else
			{
				overrideController[defaultWalk] = defaultWalk;
			}

			if (newWeapon.runClip != null)
			{
				overrideController[defaultRun] = newWeapon.runClip;
			}
			else
			{
				overrideController[defaultRun] = defaultRun;
			}
		}
		
		else
		{
			overrideController[defaultIdle] = defaultIdle;
			overrideController[defaultWalk] = defaultWalk;
			overrideController[defaultRun] = defaultRun;
		}
	}
}

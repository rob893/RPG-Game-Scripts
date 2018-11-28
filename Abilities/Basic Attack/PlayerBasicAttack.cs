using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBasicAttack : TargetedMeleeAbility {

	protected override void Start()
	{
		base.Start();
		EquipmentManager.Instance.OnWeaponChanged += ChangeWeapons;
	}

	public override void AnimationEvent()
	{
		if (targetedStats != null)
		{
			base.AnimationEvent();
			targetedStats.TakeDamage(myStats.attackPower.GetValue() * attackPowerPercent, myStats);

			PlayAbilityAudioClip(2);

			if (targetedStats.CurrentHealth <= 0)
			{
				characterCombat.InCombat = false;
			}
		}
	}

	private void ChangeWeapons(Weapon newWeapon)
	{
		attackPowerPercent = newWeapon.attackPowerPercent / 100;
		abilityAnimationClips = newWeapon.weaponAnimations;
		initialSound = newWeapon.swingSound;
		secondarySound = newWeapon.hitSound;
		characterCombat.ForceSetAbility(this);
	}
}

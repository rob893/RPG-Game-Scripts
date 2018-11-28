using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedRangedAbility : TargetedAbility {

	protected GameObject projectile;
	protected float projectileSpeed;
	protected Transform spawnPoint;


	protected override void UseHook()
	{
		spawnPoint = myStats.hand;
	}

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		projectile = (config as TargetedRangedConfig).projectile;
		projectileSpeed = (config as TargetedRangedConfig).speed;
	}

	public override void AnimationEvent()
	{
		GameObject newProjectile = ObjectPooler.Instance.ActivatePooledObject(projectile, spawnPoint.position, transform.rotation);
		Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
		projectileComponent.TargetStats = targetedStats;
		projectileComponent.Attack = this;
		projectileComponent.speed = projectileSpeed;
	}

	public virtual void OnCollision(CharacterStats collidedStats)
	{
		if (collidedStats != null)
		{
			foreach (EffectConfig effect in effects)
			{
				Effect effectInstance = effect.AttachEffectTo(collidedStats.gameObject);
				effectInstance.MyStats = myStats;
				effectInstance.TheirStats = collidedStats;
			}
			
			collidedStats.TakeDamage(attackPowerPercent * myStats.attackPower.GetValue(), myStats);
		}
	}
}

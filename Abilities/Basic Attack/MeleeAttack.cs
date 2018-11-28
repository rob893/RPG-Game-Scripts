public class MeleeAttack : TargetedMeleeAbility {


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
}

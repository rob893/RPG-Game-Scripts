using UnityEngine;

public class SelfCastAbility : TargetedAbility {

	protected GameObject graphic;

	public override void SetConfig(AbilityConfig configToSet)
	{
		base.SetConfig(configToSet);
		graphic = (config as SelfCastConfig).graphic;
	}

	public override void AnimationEvent()
	{
		if(graphic != null)
		{
			ObjectPooler.Instance.ActivatePooledObject(graphic, myStats.transform.position, Quaternion.identity, 0, myStats.transform);
		}

		foreach (EffectConfig effect in effects)
		{
			Effect effectInstance = effect.AttachEffectTo(myStats.gameObject);
			effectInstance.MyStats = myStats;
			effectInstance.TheirStats = myStats;
			
		}
	}

}

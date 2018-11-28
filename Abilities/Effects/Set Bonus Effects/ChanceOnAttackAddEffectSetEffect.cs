using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanceOnAttackAddEffectSetEffect : SetBonusEffect {
	private List<EffectConfig> effectsToAdd;
	private int chanceOnAttack;
	private float buffDuration;
	private float internalCD;
	private bool onCD = false;


	public override void SetConfig(EffectConfig configToSet)
	{
		base.SetConfig(configToSet);
		effectsToAdd = (configToSet as ChanceOnAttackAddEffectSetEffectConfig).effectsToAdd;
		chanceOnAttack = (configToSet as ChanceOnAttackAddEffectSetEffectConfig).chanceOnAttack;
		buffDuration = (configToSet as ChanceOnAttackAddEffectSetEffectConfig).buffDuration;
		internalCD = (configToSet as ChanceOnAttackAddEffectSetEffectConfig).internalCD;
	}

	protected override void SetBonusLogic()
	{
		GetComponent<CharacterCombat>().OnAttack += ChanceOnHit;
	}

	protected override void RemoveAndDestroySetBonusLogic()
	{
		GetComponent<CharacterCombat>().OnAttack -= ChanceOnHit;
	}

	private void ChanceOnHit()
	{
		if (!onCD)
		{
			int ranRoll = Random.Range(1, 101);
			if (ranRoll <= chanceOnAttack)
			{
				onCD = true;
				foreach (EffectConfig effect in effectsToAdd)
				{
					Effect effectInstance = effect.AttachEffectTo(MyStats.gameObject);
					effectInstance.MyStats = MyStats;
					effectInstance.TheirStats = MyStats;
					effectInstance.duration = buffDuration;
				}
				StartCoroutine(CDTimer());
			}
		}
	}

	private IEnumerator CDTimer()
	{
		yield return new WaitForSeconds(internalCD);

		onCD = false;
	}
}

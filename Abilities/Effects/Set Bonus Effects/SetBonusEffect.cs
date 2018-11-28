using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SetBonusEffect : Effect {

	protected sealed override IEnumerator EffectLogic()
	{
		SetBonusLogic();
		yield return null;
	}

	public void RemoveAndDestroySetBonus()
	{
		RemoveAndDestroySetBonusLogic();

		RemoveAndDestroy();
	}

	protected abstract void SetBonusLogic();

	protected abstract void RemoveAndDestroySetBonusLogic();

	protected sealed override void Update() {}

	protected sealed override void LogicBeforeDestroy() {}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonCreatureEffect : Effect
{
	protected override IEnumerator EffectLogic()
	{
		GameObject summonedCreature = Instantiate(effectPrefab, transform.position + new Vector3(2, 0, 0), Quaternion.identity);

		Destroy(summonedCreature.GetComponent<SearchingState>());
		Destroy(summonedCreature.GetComponent<ResetState>());

		summonedCreature.AddComponent<FollowAndSearchState>();
		summonedCreature.AddComponent<FollowResetState>();

		summonedCreature.GetComponent<FollowAndSearchState>().followTarget = transform;

		summonedCreature.GetComponent<NPCController>().UpdateStateComponents();

		summonedCreature.GetComponent<CharacterStats>().ScaleStatsToPlayerLevel(PlayerManager.Instance.playerStats.GetLevel());

		yield return new WaitForSeconds(duration);

		Destroy(summonedCreature);
		RemoveAndDestroy();
	}

	protected override void LogicBeforeDestroy()
	{

	}
}

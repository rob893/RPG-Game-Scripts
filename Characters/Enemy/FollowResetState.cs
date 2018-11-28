using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowResetState : ResetState {

	public override void Initialize()
	{
		base.Initialize();
		npcController.ClearThreatTable();
	}

	public override void PerformBehavior()
	{
		npcController.SetState(npcController.GetSearchingState());
	}
}

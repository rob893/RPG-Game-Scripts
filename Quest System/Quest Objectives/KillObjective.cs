using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterStats))]
public class KillObjective : QuestObjective {

	protected override void Start ()
	{
		base.Start();
		GetComponent<CharacterStats>().OnDead += UpdateObjective;
	}
	
}

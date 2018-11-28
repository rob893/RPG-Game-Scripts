using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractObjective : QuestObjective {

	
	protected override void Start ()
	{
		base.Start();
		GetComponent<Interactable>().OnInteracted += UpdateObjective;	
	}
}

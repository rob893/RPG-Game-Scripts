using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AreaObjective : QuestObjective {

	protected override void Start()
	{
		base.Start();
		GetComponent<BoxCollider>().isTrigger = true;
		gameObject.layer = 2;
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			UpdateObjective();
		}
	}
}

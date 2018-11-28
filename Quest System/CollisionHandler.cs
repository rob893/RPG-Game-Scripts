using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHandler : MonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Player")
		{
			QuestManager.Instance.AddQuestItem("Leave Town", 1);
		}
		
	}
}

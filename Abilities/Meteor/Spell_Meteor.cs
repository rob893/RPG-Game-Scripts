using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_Meteor : MonoBehaviour {

	public MeteorAbility abilityReference;
	

	private void Start()
	{
		var tm = GetComponentInChildren<RFX4_TransformMotion>(true);
		if (tm != null) tm.CollisionEnter += Tm_CollisionEnter;
		else
		{
			Debug.Log("No transform Motion script attached!");
		}
	}

	private void Tm_CollisionEnter(object sender, RFX4_TransformMotion.RFX4_CollisionInfo e)
	{
		abilityReference.OnImpact(e.Hit.point);
	}
}

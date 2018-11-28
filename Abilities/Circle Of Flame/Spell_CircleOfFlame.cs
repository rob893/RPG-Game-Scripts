using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell_CircleOfFlame : MonoBehaviour {

	public CircleOfFlameAbility abilityReference;
	public float tickFreq;

	private float timer = 0;


	private void Update () {
		timer += Time.deltaTime;

		if(timer >= tickFreq)
		{
			abilityReference.Tick(transform.position);
			timer = 0;
		}
	}
}

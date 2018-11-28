using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Effect/Increase Stat"))]
public class IncreaseStatEffectConfig : EffectConfig {

	[Header("Increase Stat specific")]
	public bool armor;
	public bool attackPowerPercentIncrease;
	public bool health;
	public bool power;
	public bool critChance;
	public bool powerRegainPerSecond;
	public bool percHealthRegainPerFive;

	public int amountIncrease;

	public override Effect GetBehaviourComponent(GameObject objectToAttachTo)
	{
		return objectToAttachTo.AddComponent<IncreaseStatEffect>();
	}
}

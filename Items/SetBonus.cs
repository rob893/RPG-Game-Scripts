using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Set Bonus", menuName = "Inventory/Set Bonus")]
public class SetBonus : ScriptableObject {

	public int numSetItemsReq;
	[TextArea] public string setBonusDescription;
	public List<SetBonusEffectConfig> setBonusEffects = new List<SetBonusEffectConfig>();

	public string GetSetBonusToolTip()
	{
		return "(" + numSetItemsReq + ") Set: " + setBonusDescription;
	}
}

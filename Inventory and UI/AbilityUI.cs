using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityUI : MonoBehaviour {

	public Transform itemsParent;

	private PlayerManager playerManager;
	private AbilityBarSlot[] slots;
	private Dictionary<KeyCode, Ability> selectedAbilities;


	private void Start()
	{
		playerManager = PlayerManager.Instance;
		playerManager.onAbilityChangedCallback += UpdateUI;
		selectedAbilities = PlayerManager.Instance.selectedAbilities;
		slots = itemsParent.GetComponentsInChildren<AbilityBarSlot>();
		UpdateUI();
	}

	private void UpdateUI()
	{
		foreach(AbilityBarSlot slot in slots)
		{
			if(selectedAbilities.ContainsKey(slot.key))
			{
				if(slot.ability == null)
				{
					slot.AddAbilityToSlot(selectedAbilities[slot.key]);
				}
				else if (slot.ability != selectedAbilities[slot.key])
				{
					slot.AddAbilityToSlot(selectedAbilities[slot.key]);
				}
			}
		}
	}
}

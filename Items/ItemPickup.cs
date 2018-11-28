using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : Interactable {

	public Item item;

	public override void Interact(Transform interacter)
	{
		base.Interact(interacter);

		PickUp();
	}

	private void PickUp()
	{
		bool wasPickedUp = Inventory.Instance.Add(item);

		if (wasPickedUp)
		{
			Destroy(gameObject);
		}
	}
}

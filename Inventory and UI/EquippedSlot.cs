using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot: MonoBehaviour {

	public Image icon;
	public EquipmentSlot slot;

	private Item item;
	private UIManager uiManager;
	private RectTransform rt;
	private EquipmentManager equipmentManager;


	private void Start()
	{
		uiManager = UIManager.Instance;
		rt = GetComponent<RectTransform>();
		equipmentManager = EquipmentManager.Instance;
	}

	public void AddItem(Item newItem)
	{
		item = newItem;

		icon.sprite = item.icon;
		icon.enabled = true;
	}

	public void ClearSlot()
	{
		item = null;

		icon.sprite = null;
		icon.enabled = false;
		uiManager.HideToolTip();
	}

	public void Unequip()
	{
		equipmentManager.Unequip((int)slot);
		ClearSlot();
	}

	public void ShowToolTip()
	{
		if (item != null)
		{
			uiManager.SetToolTipLocation(new Vector2(transform.position.x - (rt.rect.width / 2), transform.position.y + (rt.rect.height / 2)));
			uiManager.ShowItemToolTip(item.GetToolTip());
		}
	}
}

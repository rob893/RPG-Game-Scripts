using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingButton : MonoBehaviour
{
	[SerializeField] private Sprite notSelectedImage;
	[SerializeField] private Sprite selectedImage;
	[SerializeField] private TextMeshProUGUI itemName;

	private Item item;
	private Image buttonImage;
	private CraftingUI craftingUI;


	private void Start()
	{
		craftingUI = GetComponentInParent<CraftingUI>();
		buttonImage = GetComponent<Image>();
	}

	public void SetItem(Item newItem, bool setName = true)
	{
		item = newItem;

		if (setName)
		{
			itemName.text = item.itemName;
		}
	}

	public void SetNotSelectedImage()
	{
		if(buttonImage != null)
		{
			buttonImage.sprite = notSelectedImage;
		}
		
	}

	public void SelectItem()
	{
		craftingUI.SetCraftItemButton(item);
		buttonImage.sprite = selectedImage;
	}

	public void CraftItem()
	{
		item.CraftItem();
	}

}

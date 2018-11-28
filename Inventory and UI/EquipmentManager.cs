using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour {
	//TODO: refactor equip, unequip, loaditems. Lots of code duplication.
	public static EquipmentManager Instance;

	public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);
	public OnEquipmentChanged onEquipmentChanged;

	public delegate void OnWeaponChange(Weapon newWeapon);
	public OnWeaponChange OnWeaponChanged;

	public Transform rightHand;
	public Transform leftHand;
	public Equipment[] currentEquipment;

	[SerializeField] private GameObject shieldModel;
	[SerializeField] private Weapon unarmed;
	[SerializeField] private Equipment[] defaultItems;

	private Inventory inventory;
	private Dictionary<ItemSet, int> itemSets = new Dictionary<ItemSet, int>();
	private Dictionary<SetBonus, List<SetBonusEffect>> setBonusEffects = new Dictionary<SetBonus, List<SetBonusEffect>>();
	private GameObject currentWeapon;
	private IKHands ikhands;
	private PlayerStats playerStats;


	private EquipmentManager() { }

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
		currentEquipment = new Equipment[numSlots];
		inventory = Inventory.Instance;
		ikhands = FindObjectOfType<IKHands>();
		playerStats = GetComponent<PlayerStats>();

		if(leftHand == null)
		{
			leftHand = GameObject.FindGameObjectWithTag("LeftHand").transform;
		}
		if(rightHand == null)
		{
			rightHand = GameObject.FindGameObjectWithTag("RightHand").transform;
		}

		if(!GameManager.Instance.dataToLoad)
		{
			StartCoroutine(EquipDefaultsCoroutine());
		}
	}

	public void Equip(Equipment newItem)
	{
		Equipment oldItem = null;

		int slotIndex = (int)newItem.equipSlot;
		
		if(currentEquipment[slotIndex] != null)
		{
			oldItem = currentEquipment[slotIndex];
			if(oldItem != unarmed)
			{
				inventory.Add(oldItem);
			}

			if (oldItem.isSetItem)
			{
				if (itemSets.ContainsKey(oldItem.setPartOf))
				{
					itemSets[oldItem.setPartOf]--;
					
					foreach (SetBonus bonus in oldItem.setPartOf.setBonuses)
					{
						if (itemSets[oldItem.setPartOf] < bonus.numSetItemsReq && setBonusEffects.ContainsKey(bonus))
						{
							foreach (SetBonusEffect effect in setBonusEffects[bonus])
							{
								effect.RemoveAndDestroySetBonus();
							}
							setBonusEffects.Remove(bonus);
						}
					}

					if (itemSets[oldItem.setPartOf] == 0)
					{
						itemSets.Remove(oldItem.setPartOf);
					}
				}
			}
		}

		if(onEquipmentChanged != null)
		{
			onEquipmentChanged.Invoke(newItem, oldItem);
		}

		if(newItem.equipSlot == EquipmentSlot.Weapon)
		{
			if((newItem as Weapon).isTwoHanded)
			{
				Unequip((int)EquipmentSlot.Shield);
			}

			PutWeaponInHand(newItem as Weapon);
			OnWeaponChanged(newItem as Weapon);
		}

		if(newItem.equipSlot == EquipmentSlot.Shield && currentEquipment[(int)EquipmentSlot.Weapon] != null && (currentEquipment[(int)EquipmentSlot.Weapon] as Weapon).isTwoHanded)
		{
			Unequip((int)EquipmentSlot.Weapon);
		}

		if (newItem.isSetItem)
		{
			if (itemSets.ContainsKey(newItem.setPartOf))
			{
				itemSets[newItem.setPartOf]++;
			}
			else
			{
				itemSets.Add(newItem.setPartOf, 1);
			}

			foreach(SetBonus bonus in newItem.setPartOf.setBonuses)
			{
				if(itemSets[newItem.setPartOf] >= bonus.numSetItemsReq && !setBonusEffects.ContainsKey(bonus))
				{
					setBonusEffects.Add(bonus, new List<SetBonusEffect>());
					foreach(SetBonusEffectConfig effect in bonus.setBonusEffects)
					{
						
						SetBonusEffect effectInstance = (SetBonusEffect)effect.AttachEffectTo(gameObject);
						effectInstance.MyStats = playerStats;
						effectInstance.TheirStats = playerStats;
						setBonusEffects[bonus].Add(effectInstance);
					}
				}
			}
		}

		if(newItem.equipSlot == EquipmentSlot.Shield)
		{
			shieldModel.SetActive(true);
		}

		inventory.PlayLootSound(newItem, true);
		currentEquipment[slotIndex] = newItem;
	}

	public void PutWeaponInHand(Weapon newWeapon)
	{
		Destroy(currentWeapon);
		var weaponPrefab = newWeapon.weaponPrefab;
		var weapon = Instantiate(weaponPrefab, rightHand);
		if (newWeapon.isTwoHanded)
		{
			Transform attachPoint = weapon.transform.Find("LeftHandAttach");
			if (attachPoint != null)
			{
				ikhands.attachLeft = attachPoint;
				ikhands.leftHandObj = leftHand;
			}
			else
			{
				Debug.Log("this two handed weapon needs a LeftHandAttach transform child");
			}
			
		}
		else
		{
			ikhands.leftHandObj = null;
			ikhands.attachLeft = null;
		}
		currentWeapon = weapon;
		weapon.transform.localPosition = newWeapon.grip.localPosition;
		weapon.transform.localRotation = newWeapon.grip.localRotation;
	}

	public void Unequip(int slotIndex)
	{
		if(currentEquipment[slotIndex] != null)
		{
			Equipment oldItem = currentEquipment[slotIndex];

			if (oldItem.isSetItem)
			{
				if (itemSets.ContainsKey(oldItem.setPartOf))
				{
					
					itemSets[oldItem.setPartOf]--;

					foreach (SetBonus bonus in oldItem.setPartOf.setBonuses)
					{
						if (itemSets[oldItem.setPartOf] < bonus.numSetItemsReq && setBonusEffects.ContainsKey(bonus))
						{
							foreach (SetBonusEffect effect in setBonusEffects[bonus])
							{
								effect.RemoveAndDestroySetBonus();
							}
							setBonusEffects.Remove(bonus);
						}
					}

					if (itemSets[oldItem.setPartOf] == 0)
					{
						itemSets.Remove(oldItem.setPartOf);
					}
				}
			}

			currentEquipment[slotIndex] = null;
			inventory.Add(oldItem);


			if (onEquipmentChanged != null)
			{
				onEquipmentChanged.Invoke(null, oldItem);
			}

			if (oldItem.equipSlot == EquipmentSlot.Shield)
			{
				shieldModel.SetActive(false);
			}

			if (slotIndex == (int)EquipmentSlot.Weapon)
			{
				Equip(unarmed);
				return;
			}

			inventory.PlayLootSound(oldItem);
		}
	}

	public void UnequipAll()
	{
		for(int i = 0; i < currentEquipment.Length; i++)
		{
			Unequip(i);
		}
		EquipDefaultItems();
	}

	private void EquipDefaultItems()
	{
		foreach(Equipment item in defaultItems)
		{
			Equip(item);
		}
	}

	public void LoadSavedEquipment(Equipment[] loadedEquipment)
	{
		foreach (Equipment newItem in loadedEquipment)
		{
			if(newItem != null)
			{
				Equipment oldItem = null;

				int slotIndex = (int)newItem.equipSlot;

				if (currentEquipment[slotIndex] != null)
				{
					oldItem = currentEquipment[slotIndex];
				}

				if (onEquipmentChanged != null)
				{
					onEquipmentChanged.Invoke(newItem, oldItem);
				}

				if (newItem.equipSlot == EquipmentSlot.Weapon)
				{
					PutWeaponInHand(newItem as Weapon);
					OnWeaponChanged(newItem as Weapon);
				}

				if (newItem.equipSlot == EquipmentSlot.Shield)
				{
					shieldModel.SetActive(true);
				}

				if (newItem.isSetItem)
				{
					if (itemSets.ContainsKey(newItem.setPartOf))
					{
						itemSets[newItem.setPartOf]++;
					}
					else
					{
						itemSets.Add(newItem.setPartOf, 1);
					}

					foreach (SetBonus bonus in newItem.setPartOf.setBonuses)
					{
						if (itemSets[newItem.setPartOf] >= bonus.numSetItemsReq && !setBonusEffects.ContainsKey(bonus))
						{
							setBonusEffects.Add(bonus, new List<SetBonusEffect>());
							foreach (SetBonusEffectConfig effect in bonus.setBonusEffects)
							{

								SetBonusEffect effectInstance = (SetBonusEffect)effect.AttachEffectTo(gameObject);
								effectInstance.MyStats = playerStats;
								effectInstance.TheirStats = playerStats;
								setBonusEffects[bonus].Add(effectInstance);
							}
						}
					}
				}

				currentEquipment[slotIndex] = newItem;
			}
		}
	}

	IEnumerator EquipDefaultsCoroutine()
	{
		yield return new WaitForEndOfFrame();
		EquipDefaultItems();
	}

	public GameObject GetCurrentWeapon()
	{
		return currentWeapon;
	}

	public Weapon GetUnarmed()
	{
		return unarmed;
	}

	public int GetNumSetItems(ItemSet itemSet)
	{
		if (itemSets.ContainsKey(itemSet))
		{
			return itemSets[itemSet];
		}
		else
		{
			return 0;
		}
	}

	public bool HasSetItemEquipped(Equipment item)
	{
		if (!item.isSetItem)
		{
			return false;
		}

		return item == currentEquipment[(int)item.equipSlot];
	}

	public int GetNumSetItemsEquipped(ItemSet set)
	{
		if (itemSets.ContainsKey(set))
		{
			return itemSets[set];
		}

		return 0;
	}
}

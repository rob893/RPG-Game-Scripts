using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment {

	public float attackPowerPercent;
	public Transform grip;
	public AudioClip hitSound;
	public AudioClip swingSound;
	public AnimationClip[] weaponAnimations;
	public AnimationClip idleClip;
	public AnimationClip walkClip;
	public AnimationClip runClip;
	public GameObject weaponPrefab;
	public bool isTwoHanded;


	//protected override string GetWeaponDamage()
	//{
	//	return "\nDamage: " + attackPowerPercent + "% of attack power";
	//}
	protected override string GetEquipmentSlot()
	{
		if (isTwoHanded)
		{
			return "\nTwo-Hand";
		}
		else
		{
			return "\nOne-Hand";
		}
	}
}

public enum Layer { Walkable = 9, Attackable = 10, RaycastEndStop = -1, Friendly = 11, Wall = 13, IgnoreRaycast = 2 }

public enum EquipmentSlot { Head, Legs, Chest, Feet, Weapon, Shield }

public enum EquipmentMeshRegion { Torso, Arms, Legs }

public enum ItemRarity { Common, Uncommon, Rare, Epic, Legendary, Unique }

public enum CharacterType { Humanoid, Monster, Beast, Dragon, Demon, Elemental, Undead }

public enum HexColor { Gray, White, Green, Blue, Purple, Orange, Red, LightGray }

public enum Difficulty { Easy, Normal, Hard }

public static class Utility
{
	public static string GetHexColor(HexColor color)
	{
		switch (color)
		{
			case HexColor.Blue:
				return "#0070dd";
			case HexColor.Gray:
				return "#808080";
			case HexColor.White:
				return "#ffffff";
			case HexColor.Green:
				return "#1eff00";
			case HexColor.Orange:
				return "#ff8000";
			case HexColor.Purple:
				return "#a335ee";
			case HexColor.Red:
				return "red";
			case HexColor.LightGray:
				return "#A0A0A0";
			default:
				return null;
		}
	}

	public static string GetRarityColor(ItemRarity rarity)
	{
		switch (rarity)
		{
			case ItemRarity.Common:
				return GetHexColor(HexColor.White);
			case ItemRarity.Uncommon:
				return GetHexColor(HexColor.Green);
			case ItemRarity.Rare:
				return GetHexColor(HexColor.Blue);
			case ItemRarity.Epic:
				return GetHexColor(HexColor.Purple);
			case ItemRarity.Legendary:
				return GetHexColor(HexColor.Orange);
			case ItemRarity.Unique:
				return GetHexColor(HexColor.Red);
			default:
				return GetHexColor(HexColor.White);
		}
	}
}


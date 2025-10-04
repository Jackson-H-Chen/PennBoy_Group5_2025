using System;
using UnityEngine;

public enum InventoryItemType
{
	Weapon
}

[Serializable]
public class InventoryItem
{
	public string id;
	public string displayName;
	public Color displayColor = new Color(0.2f, 0.6f, 1.0f, 1f);
	public Sprite icon;
	public InventoryItemType itemType = InventoryItemType.Weapon;

	// Weapon stats (raw values per spec)
	public float baseDamage; // 0..50
	public float baseAccuracyPct; // 0..100
	public float baseShieldBypassPct; // 0..100
	public float baseArmorPenetration; // 0..50
	public float armorShred; // 0..25

	public InventoryItem(string id, string name, Color color, Sprite icon = null)
	{
		this.id = id;
		this.displayName = name;
		this.displayColor = color;
		this.icon = icon;
	}
}



using System.Collections.Generic;
using UnityEngine;

public enum ShipComponentSlot
{
	Weapon1,
	Weapon2,
	Weapon3
}

public class InventoryManager : MonoBehaviour
{
	public static InventoryManager Instance;

	private readonly List<InventoryItem> items = new List<InventoryItem>();
	private readonly Dictionary<ShipComponentSlot, InventoryItem> equipped = new Dictionary<ShipComponentSlot, InventoryItem>();

	public static InventoryManager Ensure()
	{
		if (Instance != null)
		{
			return Instance;
		}
		GameObject go = new GameObject("InventoryManager");
		Instance = go.AddComponent<InventoryManager>();
		return Instance;
	}

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public IReadOnlyList<InventoryItem> GetItems()
	{
		return items;
	}

	public void AddItem(InventoryItem item)
	{
		if (item == null)
		{
			return;
		}
		items.Add(item);
	}

	public bool RemoveItem(InventoryItem item)
	{
		if (item == null)
		{
			return false;
		}
		return items.Remove(item);
	}

	public InventoryItem GetEquipped(ShipComponentSlot slot)
	{
		InventoryItem v;
		return equipped.TryGetValue(slot, out v) ? v : null;
	}

	public bool Equip(ShipComponentSlot slot, InventoryItem item)
	{
		if (item == null)
		{
			return false;
		}
		// If item comes from bag, remove it
		RemoveItem(item);
		// If this item is already equipped somewhere else, clear that slot
		ShipComponentSlot? currentSlot = FindSlotOfItem(item);
		if (currentSlot.HasValue)
		{
			equipped.Remove(currentSlot.Value);
		}
		// If something is already equipped, send it back to bag
		InventoryItem existing;
		if (equipped.TryGetValue(slot, out existing) && existing != null)
		{
			items.Add(existing);
		}
		equipped[slot] = item;
		return true;
	}

	public bool Unequip(ShipComponentSlot slot)
	{
		InventoryItem existing;
		if (equipped.TryGetValue(slot, out existing) && existing != null)
		{
			equipped.Remove(slot);
			items.Add(existing);
			return true;
		}
		return false;
	}

	public ShipComponentSlot? FindSlotOfItem(InventoryItem item)
	{
		foreach (KeyValuePair<ShipComponentSlot, InventoryItem> kv in equipped)
		{
			if (kv.Value == item)
			{
				return kv.Key;
			}
		}
		return null;
	}
}



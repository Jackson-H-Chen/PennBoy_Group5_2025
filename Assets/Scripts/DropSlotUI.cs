using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSlotUI : MonoBehaviour, IDropHandler
{
	private ShipComponentSlot slot;
	private InventoryUIController ui;

	public void Initialize(ShipComponentSlot slot, InventoryUIController ui)
	{
		this.slot = slot;
		this.ui = ui;
	}

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData == null || eventData.pointerDrag == null)
		{
			return;
		}
		DraggableItemUI drag = eventData.pointerDrag.GetComponent<DraggableItemUI>();
		if (drag == null)
		{
			return;
		}
		InventoryItem item = drag.GetItem();
		if (item == null)
		{
			return;
		}
		InventoryManager.Ensure().Equip(slot, item);
		// Mark the dragged UI as consumed so it cleans itself up on end drag
		drag.MarkConsumed();
		ui.Refresh();
	}

	public void SetEquipped(InventoryItem item, Font font)
	{
		// Clear previous equipped visual (keep the label child if present)
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Transform child = transform.GetChild(i);
			if (child.name.StartsWith("Equipped_"))
			{
				Object.Destroy(child.gameObject);
			}
		}
		if (item == null)
		{
			return;
		}
		GameObject go = new GameObject("Equipped_" + (string.IsNullOrEmpty(item.displayName) ? item.id : item.displayName));
		go.transform.SetParent(transform, false);
		RectTransform rt = go.AddComponent<RectTransform>();
		rt.anchorMin = new Vector2(0.1f, 0.1f);
		rt.anchorMax = new Vector2(0.9f, 0.9f);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
		Image img = go.AddComponent<Image>();
		img.color = item.icon != null ? Color.white : item.displayColor;
		img.sprite = item.icon;
		img.type = Image.Type.Sliced;

		// Make the equipped item clickable and draggable to other slots or out to unequip
		DraggableItemUI drag = go.AddComponent<DraggableItemUI>();
		drag.Initialize(item, ui);
		drag.SetSourceSlot(slot);

		GameObject label = new GameObject("Name");
		label.transform.SetParent(go.transform, false);
		RectTransform lrt = label.AddComponent<RectTransform>();
		lrt.anchorMin = new Vector2(0, 0);
		lrt.anchorMax = new Vector2(1, 0);
		lrt.sizeDelta = new Vector2(0, 22);
		Text txt = label.AddComponent<Text>();
		txt.font = font;
		txt.alignment = TextAnchor.MiddleCenter;
		txt.color = Color.white;
		txt.text = item.displayName;
		txt.resizeTextForBestFit = true;
		txt.resizeTextMinSize = 8;
		txt.resizeTextMaxSize = 16;
	}
}



using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	private InventoryItem item;
	private InventoryUIController ui;
	private Transform originalParent;
	private Canvas rootCanvas;
	private CanvasGroup canvasGroup;
	private bool consumed;
	private bool isFromSlot;
	private ShipComponentSlot sourceSlot;

	public void Initialize(InventoryItem item, InventoryUIController ui)
	{
		this.item = item;
		this.ui = ui;
		canvasGroup = GetComponent<CanvasGroup>();
		if (canvasGroup == null)
		{
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
	}

	public InventoryItem GetItem()
	{
		return item;
	}

	public void SetSourceSlot(ShipComponentSlot slot)
	{
		isFromSlot = true;
		sourceSlot = slot;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (item == null)
		{
			return;
		}
		if (ui != null)
		{
			ui.ShowItemDetails(item);
		}
		originalParent = transform.parent;
		rootCanvas = GetComponentInParent<Canvas>();
		transform.SetParent(rootCanvas.transform, true);
		canvasGroup.blocksRaycasts = false;
		canvasGroup.alpha = 0.8f;
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.position = eventData.position;
		if (ui != null && item != null)
		{
			ui.ShowItemDetails(item);
		}
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1f;
		if (consumed)
		{
			Destroy(gameObject);
			return;
		}
		// If dropped on a slot, it will be handled by that slot's OnDrop (and mark consumed)
		// If this item originated from a slot and was dropped anywhere else, unequip it
		DropSlotUI overSlot = null;
		if (eventData != null && eventData.pointerEnter != null)
		{
			overSlot = eventData.pointerEnter.GetComponentInParent<DropSlotUI>();
		}
		if (isFromSlot && overSlot == null)
		{
			InventoryManager.Ensure().Unequip(sourceSlot);
			ui.Refresh();
			Destroy(gameObject);
			return;
		}
		// If dropped on nothing, return to original parent
		if (originalParent != null)
		{
			transform.SetParent(originalParent, true);
			transform.localScale = Vector3.one;
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (item != null && ui != null)
		{
			ui.ShowItemDetails(item);
		}
	}

	public void MarkConsumed()
	{
		consumed = true;
	}
}



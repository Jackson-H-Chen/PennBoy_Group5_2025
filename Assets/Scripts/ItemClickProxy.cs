using UnityEngine;
using UnityEngine.EventSystems;

public class ItemClickProxy : MonoBehaviour, IPointerClickHandler
{
	private InventoryItem item;
	private InventoryUIController ui;

	public void Initialize(InventoryItem item, InventoryUIController ui)
	{
		this.item = item;
		this.ui = ui;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (item != null && ui != null)
		{
			ui.ShowItemDetails(item);
		}
	}
}



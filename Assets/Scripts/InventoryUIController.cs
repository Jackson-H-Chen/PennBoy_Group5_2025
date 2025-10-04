using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIController : MonoBehaviour
{
	public static InventoryUIController Instance;

	private GameObject rootCanvasObject;
	private RectTransform shipArea;
	private RectTransform inventoryBar;
	private RectTransform detailsPanel;
	private Font defaultFont;

	// Maxima per spec for scaling bars
	private const float MaxDamage = 50f; // 0..50
	private const float MaxAccuracy = 100f; // 0..100
	private const float MaxShieldBypass = 100f; // 0..100
	private const float MaxArmorPen = 50f; // 0..50
	private const float MaxArmorShred = 25f; // 0..25

	private readonly Dictionary<ShipComponentSlot, DropSlotUI> slotUIs = new Dictionary<ShipComponentSlot, DropSlotUI>();

	public static InventoryUIController Ensure()
	{
		if (Instance != null)
		{
			return Instance;
		}
		GameObject go = new GameObject("InventoryUIController");
		Instance = go.AddComponent<InventoryUIController>();
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

		defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		if (defaultFont == null)
		{
			try
			{
				defaultFont = Font.CreateDynamicFontFromOSFont(new string[] { "Arial", "Helvetica", "Segoe UI", "Noto Sans" }, 16);
			}
			catch { }
		}

		BuildUIIfNeeded();
		Hide();
	}

	public void Toggle()
	{
		if (rootCanvasObject == null)
		{
			BuildUIIfNeeded();
		}
		bool next = !rootCanvasObject.activeSelf;
		rootCanvasObject.SetActive(next);
		if (next)
		{
			RenderAll();
		}
	}

	public void Hide()
	{
		if (rootCanvasObject != null)
		{
			rootCanvasObject.SetActive(false);
		}
	}

	private void RenderAll()
	{
		RenderSlots();
		RenderInventoryBar();
	}

	private void RenderSlots()
	{
		// Update visuals of equipped items on each slot
		foreach (KeyValuePair<ShipComponentSlot, DropSlotUI> kv in slotUIs)
		{
			InventoryItem eq = InventoryManager.Ensure().GetEquipped(kv.Key);
			kv.Value.SetEquipped(eq, defaultFont);
		}
	}

	private void RenderInventoryBar()
	{
		if (inventoryBar == null)
		{
			return;
		}
		// clear existing
		for (int i = inventoryBar.childCount - 1; i >= 0; i--)
		{
			Destroy(inventoryBar.GetChild(i).gameObject);
		}
		IReadOnlyList<InventoryItem> items = InventoryManager.Ensure().GetItems();
		for (int i = 0; i < items.Count; i++)
		{
			CreateInventoryItemUI(items[i], inventoryBar);
		}
	}

	private void CreateInventoryItemUI(InventoryItem item, RectTransform parent)
	{
		GameObject go = new GameObject("InvItem_" + (string.IsNullOrEmpty(item.displayName) ? item.id : item.displayName));
		go.transform.SetParent(parent, false);
		RectTransform rt = go.AddComponent<RectTransform>();
		rt.sizeDelta = new Vector2(96, 96);
		Image img = go.AddComponent<Image>();
		img.color = item.icon != null ? Color.white : item.displayColor;
		img.sprite = item.icon;
		img.type = Image.Type.Sliced;
		Outline outline = go.AddComponent<Outline>();
		outline.effectColor = new Color(0, 0, 0, 0.6f);
		outline.effectDistance = new Vector2(2, -2);
		LayoutElement le = go.AddComponent<LayoutElement>();
		le.preferredWidth = 96;
		le.preferredHeight = 96;

		GameObject label = new GameObject("Label");
		label.transform.SetParent(go.transform, false);
		RectTransform lrt = label.AddComponent<RectTransform>();
		lrt.anchorMin = new Vector2(0, 0);
		lrt.anchorMax = new Vector2(1, 0);
		lrt.sizeDelta = new Vector2(0, 24);
		Text txt = label.AddComponent<Text>();
		txt.font = defaultFont;
		txt.alignment = TextAnchor.MiddleCenter;
		txt.color = Color.white;
		txt.text = item.displayName;
		txt.resizeTextForBestFit = true;
		txt.resizeTextMinSize = 10;
		txt.resizeTextMaxSize = 18;

		DraggableItemUI drag = go.AddComponent<DraggableItemUI>();
		drag.Initialize(item, this);
	}

	private void BuildUIIfNeeded()
	{
		if (rootCanvasObject != null)
		{
			return;
		}

		// Ensure EventSystem
		if (FindFirstObjectByType<EventSystem>() == null)
		{
			GameObject es = new GameObject("EventSystem");
			es.AddComponent<EventSystem>();
			es.AddComponent<StandaloneInputModule>();
			DontDestroyOnLoad(es);
		}

		// Canvas
		rootCanvasObject = new GameObject("InventoryCanvas");
		Canvas canvas = rootCanvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		CanvasScaler scaler = rootCanvasObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1920, 1080);
		rootCanvasObject.AddComponent<GraphicRaycaster>();
		DontDestroyOnLoad(rootCanvasObject);

		// Background panel
		GameObject panel = new GameObject("InventoryPanel");
		panel.transform.SetParent(rootCanvasObject.transform, false);
		RectTransform prt = panel.AddComponent<RectTransform>();
		prt.anchorMin = new Vector2(0, 0);
		prt.anchorMax = new Vector2(1, 1);
		prt.offsetMin = Vector2.zero;
		prt.offsetMax = Vector2.zero;
		Image pimg = panel.AddComponent<Image>();
		pimg.color = new Color(0.04f, 0.05f, 0.07f, 0.95f);

		// Title
		GameObject title = new GameObject("InventoryTitle");
		title.transform.SetParent(panel.transform, false);
		RectTransform trt = title.AddComponent<RectTransform>();
		trt.anchorMin = new Vector2(0.5f, 1);
		trt.anchorMax = new Vector2(0.5f, 1);
		trt.pivot = new Vector2(0.5f, 1);
		trt.sizeDelta = new Vector2(800, 48);
		trt.anchoredPosition = new Vector2(0, -28);
		Text ttxt = title.AddComponent<Text>();
		ttxt.font = defaultFont;
		ttxt.alignment = TextAnchor.MiddleCenter;
		ttxt.color = Color.white;
		ttxt.fontStyle = FontStyle.Bold;
		ttxt.text = "Ship & Inventory";
		ttxt.resizeTextForBestFit = true;
		ttxt.resizeTextMinSize = 16;
		ttxt.resizeTextMaxSize = 36;

		// Ship area (center)
		GameObject ship = new GameObject("ShipArea");
		ship.transform.SetParent(panel.transform, false);
		shipArea = ship.AddComponent<RectTransform>();
		shipArea.anchorMin = new Vector2(0.15f, 0.25f);
		shipArea.anchorMax = new Vector2(0.70f, 0.85f);
		shipArea.offsetMin = Vector2.zero;
		shipArea.offsetMax = Vector2.zero;
		Image shipBg = ship.AddComponent<Image>();
		shipBg.color = new Color(0.15f, 0.18f, 0.22f, 1f);
		Outline shipOutline = ship.AddComponent<Outline>();
		shipOutline.effectColor = new Color(0, 0, 0, 0.5f);
		shipOutline.effectDistance = new Vector2(3, -3);

		// Placeholder Ship silhouette
		GameObject placeholder = new GameObject("ShipPlaceholder");
		placeholder.transform.SetParent(ship.transform, false);
		RectTransform ph = placeholder.AddComponent<RectTransform>();
		ph.anchorMin = new Vector2(0.35f, 0.2f);
		ph.anchorMax = new Vector2(0.65f, 0.8f);
		ph.offsetMin = Vector2.zero;
		ph.offsetMax = Vector2.zero;
		Image phImg = placeholder.AddComponent<Image>();
		phImg.color = new Color(0.2f, 0.22f, 0.28f, 1f);

		// Create three weapon slots
		CreateSlotUI(ShipComponentSlot.Weapon1, new Vector2(0.35f, 0.70f));
		CreateSlotUI(ShipComponentSlot.Weapon2, new Vector2(0.50f, 0.85f));
		CreateSlotUI(ShipComponentSlot.Weapon3, new Vector2(0.65f, 0.70f));

		// Details panel (right)
		GameObject details = new GameObject("DetailsPanel");
		details.transform.SetParent(panel.transform, false);
		detailsPanel = details.AddComponent<RectTransform>();
		detailsPanel.anchorMin = new Vector2(0.72f, 0.25f);
		detailsPanel.anchorMax = new Vector2(0.95f, 0.85f);
		detailsPanel.offsetMin = Vector2.zero;
		detailsPanel.offsetMax = Vector2.zero;
		Image detailsBg = details.AddComponent<Image>();
		detailsBg.color = new Color(0.10f, 0.12f, 0.16f, 1f);
		Outline dOutline = details.AddComponent<Outline>();
		dOutline.effectColor = new Color(0, 0, 0, 0.5f);
		dOutline.effectDistance = new Vector2(2, -2);

		// Title for details
		GameObject dtitle = new GameObject("DetailsTitle");
		dtitle.transform.SetParent(details.transform, false);
		RectTransform dtrt = dtitle.AddComponent<RectTransform>();
		dtrt.anchorMin = new Vector2(0, 1);
		dtrt.anchorMax = new Vector2(1, 1);
		dtrt.pivot = new Vector2(0.5f, 1);
		dtrt.sizeDelta = new Vector2(0, 40);
		dtrt.anchoredPosition = new Vector2(0, -10);
		Text dtt = dtitle.AddComponent<Text>();
		dtt.font = defaultFont;
		dtt.alignment = TextAnchor.MiddleCenter;
		dtt.color = Color.white;
		dtt.fontStyle = FontStyle.Bold;
		dtt.text = "Weapon Details";
		dtt.resizeTextForBestFit = true;
		dtt.resizeTextMinSize = 14;
		dtt.resizeTextMaxSize = 28;

		// Inventory bar (bottom)
		GameObject bar = new GameObject("InventoryBar");
		bar.transform.SetParent(panel.transform, false);
		inventoryBar = bar.AddComponent<RectTransform>();
		inventoryBar.anchorMin = new Vector2(0.05f, 0.02f);
		inventoryBar.anchorMax = new Vector2(0.95f, 0.18f);
		inventoryBar.offsetMin = Vector2.zero;
		inventoryBar.offsetMax = Vector2.zero;
		Image barBg = bar.AddComponent<Image>();
		barBg.color = new Color(0.10f, 0.12f, 0.15f, 1f);
		HorizontalLayoutGroup hlg = bar.AddComponent<HorizontalLayoutGroup>();
		hlg.spacing = 12f;
		hlg.padding = new RectOffset(12, 12, 12, 12);
		hlg.childForceExpandHeight = false;
		hlg.childForceExpandWidth = false;
		hlg.childControlHeight = true;
		hlg.childControlWidth = true;
	}

	private void CreateSlotUI(ShipComponentSlot slot, Vector2 anchor)
	{
		GameObject go = new GameObject("Slot_" + slot.ToString());
		go.transform.SetParent(shipArea.transform, false);
		RectTransform rt = go.AddComponent<RectTransform>();
		rt.anchorMin = anchor;
		rt.anchorMax = anchor;
		rt.pivot = new Vector2(0.5f, 0.5f);
		rt.sizeDelta = new Vector2(120, 120);
		Image img = go.AddComponent<Image>();
		img.color = new Color(0.25f, 0.28f, 0.34f, 1f);
		Outline outline = go.AddComponent<Outline>();
		outline.effectColor = new Color(0, 0, 0, 0.6f);
		outline.effectDistance = new Vector2(2, -2);
		DropSlotUI drop = go.AddComponent<DropSlotUI>();
		drop.Initialize(slot, this);
		slotUIs[slot] = drop;

		// Slot label
		GameObject label = new GameObject("Label");
		label.transform.SetParent(go.transform, false);
		RectTransform lrt = label.AddComponent<RectTransform>();
		lrt.anchorMin = new Vector2(0.5f, 0);
		lrt.anchorMax = new Vector2(0.5f, 0);
		lrt.pivot = new Vector2(0.5f, 0);
		lrt.anchoredPosition = new Vector2(0, -18);
		lrt.sizeDelta = new Vector2(160, 24);
		Text txt = label.AddComponent<Text>();
		txt.font = defaultFont;
		txt.alignment = TextAnchor.MiddleCenter;
		txt.color = new Color(1, 1, 1, 0.8f);
		txt.text = slot.ToString();
		txt.resizeTextForBestFit = true;
		txt.resizeTextMinSize = 10;
		txt.resizeTextMaxSize = 18;
	}

	// Called by slot and draggable items to refresh UI after changes
	public void Refresh()
	{
		RenderAll();
	}

	public void ShowItemDetails(InventoryItem item)
	{
		if (detailsPanel == null)
		{
			return;
		}
		for (int i = detailsPanel.childCount - 1; i >= 0; i--)
		{
			Transform child = detailsPanel.GetChild(i);
			if (child.name != "DetailsTitle")
			{
				Destroy(child.gameObject);
			}
		}
		GameObject nameGO = new GameObject("ItemName");
		nameGO.transform.SetParent(detailsPanel.transform, false);
		RectTransform nrt = nameGO.AddComponent<RectTransform>();
		nrt.anchorMin = new Vector2(0, 1);
		nrt.anchorMax = new Vector2(1, 1);
		nrt.pivot = new Vector2(0.5f, 1);
		nrt.anchoredPosition = new Vector2(0, -50);
		nrt.sizeDelta = new Vector2(0, 36);
		Text nameTxt = nameGO.AddComponent<Text>();
		nameTxt.font = defaultFont;
		nameTxt.alignment = TextAnchor.MiddleCenter;
		nameTxt.color = Color.white;
		nameTxt.text = item.displayName;
		nameTxt.resizeTextForBestFit = true;
		nameTxt.resizeTextMinSize = 12;
		nameTxt.resizeTextMaxSize = 24;

		float top = -96f;
		CreateStatBar("Base Damage", Scale(item.baseDamage, MaxDamage), new Color(1f, 0.7f, 0.25f), ref top, Mathf.RoundToInt(item.baseDamage).ToString());
		CreateStatBar("Accuracy", Scale(item.baseAccuracyPct, MaxAccuracy), new Color(0.35f, 0.65f, 1f), ref top, Mathf.RoundToInt(item.baseAccuracyPct) + "%");
		CreateStatBar("Shield Bypass", Scale(item.baseShieldBypassPct, MaxShieldBypass), new Color(0.75f, 0.45f, 1f), ref top, Mathf.RoundToInt(item.baseShieldBypassPct) + "%");
		CreateStatBar("Armor Penetration", Scale(item.baseArmorPenetration, MaxArmorPen), new Color(1f, 0.55f, 0.35f), ref top, Mathf.RoundToInt(item.baseArmorPenetration).ToString());
		CreateStatBar("Armor Shred", Scale(item.armorShred, MaxArmorShred), new Color(0.45f, 0.9f, 0.45f), ref top, Mathf.RoundToInt(item.armorShred).ToString());
	}

	private float Scale(float value, float max)
	{
		if (max <= 0f)
		{
			return 0f;
		}
		return Mathf.Clamp01(value / max);
	}

	private void CreateStatBar(string label, float scaled01, Color color, ref float top, string valueLabel)
	{
		GameObject row = new GameObject("Row_" + label);
		row.transform.SetParent(detailsPanel.transform, false);
		RectTransform rrt = row.AddComponent<RectTransform>();
		rrt.anchorMin = new Vector2(0.1f, 1);
		rrt.anchorMax = new Vector2(0.9f, 1);
		rrt.pivot = new Vector2(0.5f, 1);
		rrt.anchoredPosition = new Vector2(0, top);
		rrt.sizeDelta = new Vector2(0, 48);

		GameObject lgo = new GameObject("Label");
		lgo.transform.SetParent(row.transform, false);
		RectTransform lrt = lgo.AddComponent<RectTransform>();
		lrt.anchorMin = new Vector2(0, 1);
		lrt.anchorMax = new Vector2(1, 1);
		lrt.pivot = new Vector2(0.5f, 1);
		lrt.sizeDelta = new Vector2(0, 18);
		Text ltxt = lgo.AddComponent<Text>();
		ltxt.font = defaultFont;
		ltxt.alignment = TextAnchor.MiddleLeft;
		ltxt.color = new Color(1, 1, 1, 0.85f);
		ltxt.text = label + "  " + valueLabel;
		ltxt.resizeTextForBestFit = true;
		ltxt.resizeTextMinSize = 10;
		ltxt.resizeTextMaxSize = 18;

		GameObject bar = new GameObject("Bar");
		bar.transform.SetParent(row.transform, false);
		RectTransform brt = bar.AddComponent<RectTransform>();
		brt.anchorMin = new Vector2(0, 0);
		brt.anchorMax = new Vector2(1, 0);
		brt.pivot = new Vector2(0.5f, 0);
		brt.anchoredPosition = new Vector2(0, 0);
		brt.sizeDelta = new Vector2(0, 20);
		Image bg = bar.AddComponent<Image>();
		bg.color = new Color(0.18f, 0.2f, 0.25f, 1f);
		Outline outline = bar.AddComponent<Outline>();
		outline.effectColor = new Color(0, 0, 0, 0.6f);
		outline.effectDistance = new Vector2(1, -1);

		GameObject fill = new GameObject("Fill");
		fill.transform.SetParent(bar.transform, false);
		RectTransform frt = fill.AddComponent<RectTransform>();
		frt.anchorMin = new Vector2(0, 0);
		frt.anchorMax = new Vector2(Mathf.Clamp01(scaled01), 1);
		frt.offsetMin = new Vector2(0, 0);
		frt.offsetMax = new Vector2(0, 0);
		Image fimg = fill.AddComponent<Image>();
		fimg.color = color;


		top -= 60f;
	}
}



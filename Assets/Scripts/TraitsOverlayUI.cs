using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TraitsOverlayUI : MonoBehaviour
{
    public static TraitsOverlayUI Instance;

    private GameObject rootCanvasObject;
    private Text titleText;
    private Text traitsText;
    private Font defaultFont;

    public static TraitsOverlayUI Ensure()
    {
        if (Instance != null)
        {
            return Instance;
        }
        GameObject go = new GameObject("TraitsOverlayUI");
        Instance = go.AddComponent<TraitsOverlayUI>();
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

    public void Toggle(TraitSystem traitSystem)
    {
        if (rootCanvasObject == null)
        {
            BuildUIIfNeeded();
        }
        bool next = !rootCanvasObject.activeSelf;
        rootCanvasObject.SetActive(next);
        if (next)
        {
            Render(traitSystem);
        }
    }

    public void Hide()
    {
        if (rootCanvasObject != null)
        {
            rootCanvasObject.SetActive(false);
        }
    }

    private void Render(TraitSystem traitSystem)
    {
        if (traitsText == null)
        {
            return;
        }
        if (traitSystem == null)
        {
            traitsText.text = "No trait system found.";
            return;
        }
        StringBuilder sb = new StringBuilder();
        bool hasAny = false;
        foreach (string t in traitSystem.GetAllTraits())
        {
            hasAny = true;
            sb.AppendLine("• " + t);
        }
        traitsText.text = hasAny ? sb.ToString() : "No active traits.";
    }

    private void BuildUIIfNeeded()
    {
        if (rootCanvasObject != null)
        {
            return;
        }

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        rootCanvasObject = new GameObject("TraitsOverlayCanvas");
        Canvas canvas = rootCanvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = rootCanvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        rootCanvasObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(rootCanvasObject);

        // Full-screen panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(rootCanvasObject.transform, false);
        RectTransform prt = panel.AddComponent<RectTransform>();
        prt.anchorMin = new Vector2(0, 0);
        prt.anchorMax = new Vector2(1, 1);
        prt.offsetMin = Vector2.zero;
        prt.offsetMax = Vector2.zero;
        Image pimg = panel.AddComponent<Image>();
        pimg.color = new Color(0.06f, 0.08f, 0.10f, 0.96f);

        // Title
        GameObject titleGO = new GameObject("Title");
        titleGO.transform.SetParent(panel.transform, false);
        RectTransform trt = titleGO.AddComponent<RectTransform>();
        trt.anchorMin = new Vector2(0.5f, 1);
        trt.anchorMax = new Vector2(0.5f, 1);
        trt.pivot = new Vector2(0.5f, 1);
        trt.sizeDelta = new Vector2(800, 60);
        trt.anchoredPosition = new Vector2(0, -40);
        Text ttxt = titleGO.AddComponent<Text>();
        ttxt.font = defaultFont;
        ttxt.alignment = TextAnchor.MiddleCenter;
        ttxt.color = Color.white;
        ttxt.fontStyle = FontStyle.Bold;
        ttxt.text = "Active Traits";
        ttxt.resizeTextForBestFit = true;
        ttxt.resizeTextMinSize = 18;
        ttxt.resizeTextMaxSize = 42;
        titleText = ttxt;

        // Traits list
        GameObject traitsGO = new GameObject("TraitsText");
        traitsGO.transform.SetParent(panel.transform, false);
        RectTransform trl = traitsGO.AddComponent<RectTransform>();
        trl.anchorMin = new Vector2(0.1f, 0.1f);
        trl.anchorMax = new Vector2(0.9f, 0.8f);
        trl.offsetMin = Vector2.zero;
        trl.offsetMax = Vector2.zero;
        Text ltxt = traitsGO.AddComponent<Text>();
        ltxt.font = defaultFont;
        ltxt.alignment = TextAnchor.UpperLeft;
        ltxt.color = Color.white;
        ltxt.supportRichText = true;
        ltxt.horizontalOverflow = HorizontalWrapMode.Wrap;
        ltxt.verticalOverflow = VerticalWrapMode.Overflow;
        ltxt.text = "";
        ltxt.resizeTextForBestFit = false;
        traitsText = ltxt;

        // Close hint
        GameObject hintGO = new GameObject("Hint");
        hintGO.transform.SetParent(panel.transform, false);
        RectTransform hrt = hintGO.AddComponent<RectTransform>();
        hrt.anchorMin = new Vector2(0.5f, 0);
        hrt.anchorMax = new Vector2(0.5f, 0);
        hrt.pivot = new Vector2(0.5f, 0);
        hrt.sizeDelta = new Vector2(800, 40);
        hrt.anchoredPosition = new Vector2(0, 24);
        Text htxt = hintGO.AddComponent<Text>();
        htxt.font = defaultFont;
        htxt.alignment = TextAnchor.MiddleCenter;
        htxt.color = new Color(1, 1, 1, 0.7f);
        htxt.text = "Press C to close";
        htxt.resizeTextForBestFit = true;
        htxt.resizeTextMinSize = 14;
        htxt.resizeTextMaxSize = 24;
    }
}



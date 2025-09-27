using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogUIController : MonoBehaviour
{
    public static DialogUIController Instance;

    private GameObject rootCanvasObject;
    private Text speakerText;
    private Text bodyText;
    private RectTransform optionsContainer;
    private Font defaultFont;

    private DialogConversation currentConversation;
    private string currentNodeId;
    private TraitSystem currentTraitSystem;

    public static DialogUIController Ensure()
    {
        if (Instance != null)
        {
            return Instance;
        }
        GameObject go = new GameObject("DialogUIController");
        Instance = go.AddComponent<DialogUIController>();
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

        // Unity removed Arial.ttf from built-in resources in newer versions.
        // Use LegacyRuntime.ttf when available, and fall back to a common OS font.
        defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (defaultFont == null)
        {
            // Try to create a dynamic font from common system fonts
            string[] candidates = new string[] { "Arial", "Helvetica", "Segoe UI", "Noto Sans", "Apple SD Gothic Neo" };
            try
            {
                defaultFont = Font.CreateDynamicFontFromOSFont(candidates, 16);
            }
            catch
            {
                // ignore and keep null; Text components will still render with Unity's default if possible
            }
        }
        BuildUIIfNeeded();
        Hide();
    }

    public void ShowDialog(DialogConversation conversation, TraitSystem traitSystem)
    {
        if (conversation == null)
        {
            return;
        }

        BuildUIIfNeeded();
        currentConversation = conversation;
        currentTraitSystem = traitSystem;
        currentNodeId = conversation.startNodeId;

        rootCanvasObject.SetActive(true);
        RenderCurrentNode();
    }

    public void Hide()
    {
        if (rootCanvasObject != null)
        {
            rootCanvasObject.SetActive(false);
        }
    }

    private void RenderCurrentNode()
    {
        if (currentConversation == null)
        {
            Hide();
            return;
        }

        DialogNode node = currentConversation.GetNode(currentNodeId);
        if (node == null)
        {
            Hide();
            return;
        }

        if (speakerText != null)
        {
            speakerText.text = string.IsNullOrEmpty(node.speaker) ? "" : node.speaker;
        }
        if (bodyText != null)
        {
            bodyText.text = node.body ?? string.Empty;
        }

        // Clear old options
        for (int i = optionsContainer.childCount - 1; i >= 0; i--)
        {
            Transform child = optionsContainer.GetChild(i);
            Destroy(child.gameObject);
        }

        // Build option buttons
        if (node.options != null)
        {
            for (int i = 0; i < node.options.Count; i++)
            {
                DialogOption option = node.options[i];
                if (option == null || !option.IsAvailable(currentTraitSystem))
                {
                    continue;
                }
                CreateOptionButton(option);
            }
        }
    }

    private void CreateOptionButton(DialogOption option)
    {
        GameObject buttonGO = new GameObject("DialogOptionButton");
        buttonGO.transform.SetParent(optionsContainer, false);
        RectTransform rt = buttonGO.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(1, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.sizeDelta = new Vector2(0, 36);

        Image img = buttonGO.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0.25f);

        Button btn = buttonGO.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(1, 1, 1, 0.25f);
        cb.highlightedColor = new Color(1, 1, 1, 0.2f);
        cb.pressedColor = new Color(1, 1, 1, 0.3f);
        cb.selectedColor = cb.normalColor;
        cb.disabledColor = new Color(1, 1, 1, 0.1f);
        btn.colors = cb;

        // Ensure the VerticalLayoutGroup gives the button space
        LayoutElement le = buttonGO.AddComponent<LayoutElement>();
        le.minHeight = 36f;
        le.preferredHeight = 36f;
        le.flexibleWidth = 1f;

        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(buttonGO.transform, false);
        RectTransform textRT = textGO.AddComponent<RectTransform>();
        textRT.anchorMin = new Vector2(0, 0);
        textRT.anchorMax = new Vector2(1, 1);
        textRT.offsetMin = new Vector2(12, 6);
        textRT.offsetMax = new Vector2(-12, -6);

        Text txt = textGO.AddComponent<Text>();
        txt.font = defaultFont;
        txt.alignment = TextAnchor.MiddleLeft;
        txt.color = Color.white;
        txt.text = option.text ?? string.Empty;
        txt.resizeTextForBestFit = true;
        txt.resizeTextMinSize = 10;
        txt.resizeTextMaxSize = 24;

        btn.onClick.AddListener(() => OnOptionChosen(option));
    }

    private void OnOptionChosen(DialogOption option)
    {
        // Apply trait effects
        if (currentTraitSystem != null)
        {
            if (!string.IsNullOrEmpty(option.grantTrait))
            {
                currentTraitSystem.GrantTrait(option.grantTrait);
            }
            if (!string.IsNullOrEmpty(option.removeTrait))
            {
                currentTraitSystem.RemoveTrait(option.removeTrait);
            }
        }

        if (option.endsDialog)
        {
            Hide();
            return;
        }

        if (string.IsNullOrEmpty(option.nextNodeId))
        {
            Hide();
            return;
        }

        currentNodeId = option.nextNodeId;
        RenderCurrentNode();
    }

    private void BuildUIIfNeeded()
    {
        if (rootCanvasObject != null)
        {
            return;
        }

        // Ensure EventSystem exists
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
            DontDestroyOnLoad(es);
        }

        // Canvas
        rootCanvasObject = new GameObject("DialogCanvas");
        Canvas canvas = rootCanvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = rootCanvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        rootCanvasObject.AddComponent<GraphicRaycaster>();
        DontDestroyOnLoad(rootCanvasObject);

        // Dim overlay
        GameObject overlay = new GameObject("DialogOverlay");
        overlay.transform.SetParent(rootCanvasObject.transform, false);
        RectTransform overlayRT = overlay.AddComponent<RectTransform>();
        overlayRT.anchorMin = new Vector2(0, 0);
        overlayRT.anchorMax = new Vector2(1, 1);
        overlayRT.offsetMin = Vector2.zero;
        overlayRT.offsetMax = Vector2.zero;
        Image overlayImg = overlay.AddComponent<Image>();
        overlayImg.color = new Color(0, 0, 0, 0.4f);

        // Window
        GameObject window = new GameObject("DialogWindow");
        window.transform.SetParent(rootCanvasObject.transform, false);
        RectTransform winRT = window.AddComponent<RectTransform>();
        winRT.anchorMin = new Vector2(0.1f, 0.05f);
        winRT.anchorMax = new Vector2(0.9f, 0.35f);
        winRT.offsetMin = Vector2.zero;
        winRT.offsetMax = Vector2.zero;
        Image winImg = window.AddComponent<Image>();
        winImg.color = new Color(0.12f, 0.12f, 0.12f, 0.95f);

        // Speaker text
        GameObject speakerGO = new GameObject("DialogSpeakerText");
        speakerGO.transform.SetParent(window.transform, false);
        RectTransform speakerRT = speakerGO.AddComponent<RectTransform>();
        speakerRT.anchorMin = new Vector2(0, 1);
        speakerRT.anchorMax = new Vector2(1, 1);
        speakerRT.pivot = new Vector2(0.5f, 1);
        speakerRT.sizeDelta = new Vector2(0, 30);
        speakerRT.anchoredPosition = new Vector2(0, -12);
        Text speakerTxt = speakerGO.AddComponent<Text>();
        speakerTxt.font = defaultFont;
        speakerTxt.alignment = TextAnchor.UpperLeft;
        speakerTxt.color = new Color(1, 1, 1, 0.9f);
        speakerTxt.fontStyle = FontStyle.Bold;
        speakerTxt.resizeTextForBestFit = true;
        speakerTxt.resizeTextMinSize = 12;
        speakerTxt.resizeTextMaxSize = 22;
        speakerText = speakerTxt;

        // Body text
        GameObject bodyGO = new GameObject("DialogBodyText");
        bodyGO.transform.SetParent(window.transform, false);
        RectTransform bodyRT = bodyGO.AddComponent<RectTransform>();
        bodyRT.anchorMin = new Vector2(0, 0.35f);
        bodyRT.anchorMax = new Vector2(1, 1);
        bodyRT.offsetMin = new Vector2(16, 8);
        bodyRT.offsetMax = new Vector2(-16, -42);
        Text bodyTxt = bodyGO.AddComponent<Text>();
        bodyTxt.font = defaultFont;
        bodyTxt.alignment = TextAnchor.UpperLeft;
        bodyTxt.color = Color.white;
        bodyTxt.supportRichText = true;
        bodyTxt.horizontalOverflow = HorizontalWrapMode.Wrap;
        bodyTxt.verticalOverflow = VerticalWrapMode.Truncate;
        bodyTxt.resizeTextForBestFit = false;
        bodyText = bodyTxt;

        // Options container
        GameObject optionsGO = new GameObject("DialogOptions");
        optionsGO.transform.SetParent(window.transform, false);
        RectTransform optionsRT = optionsGO.AddComponent<RectTransform>();
        optionsRT.anchorMin = new Vector2(0, 0);
        optionsRT.anchorMax = new Vector2(1, 0.35f);
        optionsRT.offsetMin = new Vector2(12, 12);
        optionsRT.offsetMax = new Vector2(-12, -12);
        VerticalLayoutGroup vlg = optionsGO.AddComponent<VerticalLayoutGroup>();
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childControlHeight = true;
        vlg.childControlWidth = true;
        vlg.spacing = 8;
        ContentSizeFitter csf = optionsGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        optionsContainer = optionsRT;
    }
}



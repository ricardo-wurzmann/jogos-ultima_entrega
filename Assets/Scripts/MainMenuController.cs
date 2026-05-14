using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Fase1";

    private void Start()
    {
        Time.timeScale = 1f;
        EnsureCamera();
        EnsureEventSystem();
        BuildMenu();
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    private void EnsureCamera()
    {
        if (Camera.main != null) return;

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";

        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.03f, 0.07f, 0.04f);
        camera.orthographic = true;
        camera.orthographicSize = 5f;
        camera.transform.position = new Vector3(0f, 0f, -10f);
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
    }

    private void BuildMenu()
    {
        Canvas canvas = CreateCanvas();

        RectTransform root = CreateRect("Menu Root", canvas.transform);
        Stretch(root);

        Image background = root.gameObject.AddComponent<Image>();
        background.color = new Color(0.025f, 0.065f, 0.035f, 1f);

        RectTransform vignette = CreateRect("Vignette", root);
        Stretch(vignette);
        Image vignetteImage = vignette.gameObject.AddComponent<Image>();
        vignetteImage.color = new Color(0f, 0f, 0f, 0.35f);

        RectTransform panel = CreateRect("Menu Content", root);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = Vector2.zero;
        panel.sizeDelta = new Vector2(540f, 360f);

        VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 26f;
        layout.padding = new RectOffset(32, 32, 24, 24);

        ContentSizeFitter fitter = panel.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI title = CreateText("Title", panel, "Última Entrega", 54f, FontStyles.Bold);
        title.color = new Color(0.95f, 0.92f, 0.78f);

        TextMeshProUGUI subtitle = CreateText("Subtitle", panel, "Colete os objetivos, evite a patrulha e encontre a saida.", 22f, FontStyles.Normal);
        subtitle.color = new Color(0.78f, 0.86f, 0.68f);

        CreateButton("Start Button", panel, "Iniciar", StartGame);
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Main Menu Canvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static RectTransform CreateRect(string objectName, Transform parent)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(parent, false);
        return obj.AddComponent<RectTransform>();
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent, string value, float size, FontStyles style)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.sizeDelta = new Vector2(760f, 72f);

        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = true;

        LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 760f;
        layout.preferredHeight = Mathf.Max(72f, size * 1.5f);

        return text;
    }

    private static Button CreateButton(string objectName, Transform parent, string label, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.sizeDelta = new Vector2(280f, 72f);

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.78f, 0.86f, 0.58f, 1f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.92f, 1f, 0.66f, 1f);
        colors.pressedColor = new Color(0.56f, 0.66f, 0.38f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        RectTransform textRect = CreateRect("Label", rect);
        Stretch(textRect);

        TextMeshProUGUI text = textRect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 28f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.04f, 0.08f, 0.04f, 1f);

        LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 280f;
        layout.preferredHeight = 72f;

        return button;
    }
}

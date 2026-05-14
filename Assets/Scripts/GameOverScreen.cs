using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    private string _mainMenuSceneName = "MainMenu";
    private CanvasGroup _canvasGroup;
    private bool _built;

    public static GameOverScreen GetOrCreate(string mainMenuSceneName)
    {
        GameOverScreen screen = FindFirstObjectByType<GameOverScreen>(FindObjectsInactive.Include);
        if (screen == null)
        {
            GameObject screenObject = new GameObject("Game Over Screen");
            screen = screenObject.AddComponent<GameOverScreen>();
        }

        screen._mainMenuSceneName = mainMenuSceneName;
        screen.BuildIfNeeded();
        return screen;
    }

    public void Show()
    {
        BuildIfNeeded();
        EnsureEventSystem();

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuSceneName);
    }

    private void Awake()
    {
        BuildIfNeeded();
        Hide();
    }

    private void BuildIfNeeded()
    {
        if (_built) return;
        _built = true;

        Canvas canvas = CreateCanvas();
        _canvasGroup = canvas.gameObject.AddComponent<CanvasGroup>();

        RectTransform root = CreateRect("Game Over Root", canvas.transform);
        Stretch(root);

        Image overlay = root.gameObject.AddComponent<Image>();
        overlay.color = new Color(0.03f, 0.015f, 0.012f, 0.84f);

        RectTransform panel = CreateRect("Game Over Content", root);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = Vector2.zero;
        panel.sizeDelta = new Vector2(620f, 400f);

        VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 24f;
        layout.padding = new RectOffset(32, 32, 20, 20);

        TextMeshProUGUI title = CreateText("Title", panel, "Game Over", 58f, FontStyles.Bold);
        title.color = new Color(1f, 0.42f, 0.32f);

        TextMeshProUGUI subtitle = CreateText("Subtitle", panel, "Voce foi encontrado.", 24f, FontStyles.Normal);
        subtitle.color = new Color(0.95f, 0.86f, 0.76f);

        CreateButton("Retry Button", panel, "Tentar novamente", Retry);
        CreateButton("Menu Button", panel, "Menu inicial", BackToMenu);
    }

    private void Hide()
    {
        if (_canvasGroup == null) return;

        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }

    private Canvas CreateCanvas()
    {
        GameObject canvasObject = new GameObject("Game Over Canvas");
        canvasObject.transform.SetParent(transform, false);

        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<StandaloneInputModule>();
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
        rect.sizeDelta = new Vector2(760f, 78f);

        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.alignment = TextAlignmentOptions.Center;

        LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 760f;
        layout.preferredHeight = Mathf.Max(78f, size * 1.5f);

        return text;
    }

    private static Button CreateButton(string objectName, Transform parent, string label, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = CreateRect(objectName, parent);
        rect.sizeDelta = new Vector2(340f, 68f);

        Image image = rect.gameObject.AddComponent<Image>();
        image.color = new Color(0.86f, 0.75f, 0.52f, 1f);

        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);

        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(1f, 0.88f, 0.6f, 1f);
        colors.pressedColor = new Color(0.55f, 0.42f, 0.28f, 1f);
        colors.selectedColor = colors.highlightedColor;
        button.colors = colors;

        RectTransform textRect = CreateRect("Label", rect);
        Stretch(textRect);

        TextMeshProUGUI text = textRect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 25f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = new Color(0.08f, 0.045f, 0.03f, 1f);

        LayoutElement layout = rect.gameObject.AddComponent<LayoutElement>();
        layout.preferredWidth = 340f;
        layout.preferredHeight = 68f;

        return button;
    }
}

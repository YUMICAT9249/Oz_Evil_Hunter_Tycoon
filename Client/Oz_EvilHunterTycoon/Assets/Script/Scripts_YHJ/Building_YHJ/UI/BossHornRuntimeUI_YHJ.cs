using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossHornRuntimeUI_YHJ : MonoBehaviour
{
    private const string TargetSceneName = "Ingame_Scene";
    private const string BossHornButtonName = "UI_Button (BossHorn)";
    private const string BossHornPanelName = "BossHornPanel_YHJ";
    private const string BossHornListName = "BossHornButtonList";
    private const string AlarmTextName = "Text (Alarm)";
    private const string BossHornSpriteName = "sactx-2-2048x2048-ASTC 4x4-BUILDING_v1-e978436b_116";
    private const string MissingBuildingMessage = "\uBCF4\uC2A4\uC758 \uBFD4\uD53C\uB9AC\uB97C \uAC74\uC124\uD558\uC138\uC694";

    private static BossHornRuntimeUI_YHJ instance;
    private static bool sceneHookRegistered;
    private static Sprite cachedHornSprite;
    private static Sprite cachedWhiteSprite;
    private static Font cachedFont;

    private Canvas rootCanvas;
    private GameObject bossButtonRoot;
    private GameObject bossPanelRoot;
    private RectTransform bossButtonListRoot;
    private Graphic alarmGraphic;
    private Coroutine alarmCoroutine;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (!sceneHookRegistered)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            sceneHookRegistered = true;
        }

        TryCreate(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TryCreate(scene);
    }

    private static void TryCreate(Scene scene)
    {
        if (scene.name != TargetSceneName)
            return;

        if (instance != null)
            return;

        BossHornRuntimeUI_YHJ existing = FindObjectOfType<BossHornRuntimeUI_YHJ>();
        if (existing != null)
        {
            instance = existing;
            return;
        }

        GameObject go = new GameObject(nameof(BossHornRuntimeUI_YHJ));
        SceneManager.MoveGameObjectToScene(go, scene);
        instance = go.AddComponent<BossHornRuntimeUI_YHJ>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        EventBus_YHJ.RequestOpenBossUI += OpenPanelFromBuilding;
    }

    private void OnDisable()
    {
        EventBus_YHJ.RequestOpenBossUI -= OpenPanelFromBuilding;

        if (instance == this)
            instance = null;
    }

    private IEnumerator Start()
    {
        yield return null;
        SetupUI();
    }

    private void SetupUI()
    {
        rootCanvas = FindRootCanvas();
        if (rootCanvas == null)
        {
            Debug.LogWarning("[BossHornRuntimeUI_YHJ] Root canvas not found.");
            return;
        }

        alarmGraphic = FindAlarmGraphic();

        if (bossButtonRoot == null)
            bossButtonRoot = FindSceneObject(BossHornButtonName);
        if (bossPanelRoot == null)
            bossPanelRoot = FindSceneObject(BossHornPanelName);

        if (bossPanelRoot != null && bossButtonListRoot == null)
        {
            Transform existingList = bossPanelRoot.transform.Find(BossHornListName);
            if (existingList != null)
                bossButtonListRoot = existingList as RectTransform;
        }

        DestroyDuplicateObjects(BossHornButtonName, bossButtonRoot);
        DestroyDuplicateObjects(BossHornPanelName, bossPanelRoot);

        if (bossButtonRoot == null)
            bossButtonRoot = CreateMainButton(rootCanvas.transform);

        if (bossPanelRoot == null || bossButtonListRoot == null)
            bossPanelRoot = CreateBossPanel(rootCanvas.transform, out bossButtonListRoot);
    }

    private Canvas FindRootCanvas()
    {
        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas == null || !canvas.isRootCanvas)
                continue;

            if (!canvas.gameObject.scene.IsValid() || !canvas.gameObject.scene.isLoaded)
                continue;

            if (canvas.gameObject.scene.name != TargetSceneName)
                continue;

            return canvas;
        }

        return null;
    }

    private Graphic FindAlarmGraphic()
    {
        TMP_Text[] tmpTexts = Resources.FindObjectsOfTypeAll<TMP_Text>();
        foreach (TMP_Text tmp in tmpTexts)
        {
            if (tmp == null || tmp.gameObject.name != AlarmTextName)
                continue;

            if (!tmp.gameObject.scene.IsValid() || !tmp.gameObject.scene.isLoaded)
                continue;

            return tmp;
        }

        Text[] texts = Resources.FindObjectsOfTypeAll<Text>();
        foreach (Text text in texts)
        {
            if (text == null || text.gameObject.name != AlarmTextName)
                continue;

            if (!text.gameObject.scene.IsValid() || !text.gameObject.scene.isLoaded)
                continue;

            return text;
        }

        return null;
    }

    private GameObject CreateMainButton(Transform parent)
    {
        GameObject root = CreateUIObject(BossHornButtonName, parent);
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.anchoredPosition = new Vector2(788f, 229.51f);
        rect.sizeDelta = new Vector2(84f, 84f);

        Image background = root.AddComponent<Image>();
        background.sprite = GetWhiteSprite();
        background.color = new Color(0.33f, 0.22f, 0.12f, 0.98f);

        Button button = root.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(OnMainButtonClicked);

        GameObject circleObject = CreateUIObject("Circle", root.transform);
        RectTransform circleRect = circleObject.AddComponent<RectTransform>();
        circleRect.anchorMin = new Vector2(0.5f, 0.5f);
        circleRect.anchorMax = new Vector2(0.5f, 0.5f);
        circleRect.pivot = new Vector2(0.5f, 0.5f);
        circleRect.anchoredPosition = new Vector2(0f, 4f);
        circleRect.sizeDelta = new Vector2(58f, 58f);

        Image circleImage = circleObject.AddComponent<Image>();
        circleImage.sprite = GetWhiteCircleSprite();
        circleImage.color = Color.white;

        GameObject iconObject = CreateUIObject("HornIcon", root.transform);
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.pivot = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = new Vector2(0f, 4f);
        iconRect.sizeDelta = new Vector2(46f, 46f);

        Image iconImage = iconObject.AddComponent<Image>();
        iconImage.sprite = GetBossHornSprite();
        iconImage.color = Color.white;
        iconImage.preserveAspect = true;

        return root;
    }

    private GameObject CreateBossPanel(Transform parent, out RectTransform listRoot)
    {
        GameObject panel = CreateUIObject(BossHornPanelName, parent);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.sizeDelta = new Vector2(360f, 260f);

        Image panelImage = panel.AddComponent<Image>();
        panelImage.sprite = GetWhiteSprite();
        panelImage.color = new Color(0.08f, 0.08f, 0.08f, 0.94f);

        GameObject titleObject = CreateUIObject("Title", panel.transform);
        RectTransform titleRect = titleObject.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -14f);
        titleRect.sizeDelta = new Vector2(-24f, 30f);

        Text title = titleObject.AddComponent<Text>();
        title.font = GetBuiltinFont();
        title.text = "Boss Summon";
        title.alignment = TextAnchor.MiddleCenter;
        title.color = Color.white;
        title.resizeTextForBestFit = true;
        title.resizeTextMinSize = 16;
        title.resizeTextMaxSize = 24;

        GameObject closeButton = CreateUIObject("CloseButton", panel.transform);
        RectTransform closeRect = closeButton.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.anchoredPosition = new Vector2(-8f, -8f);
        closeRect.sizeDelta = new Vector2(34f, 34f);

        Image closeImage = closeButton.AddComponent<Image>();
        closeImage.sprite = GetWhiteSprite();
        closeImage.color = new Color(0.32f, 0.14f, 0.14f, 1f);

        Button close = closeButton.AddComponent<Button>();
        close.targetGraphic = closeImage;
        close.onClick.AddListener(() => panel.SetActive(false));

        GameObject closeLabelObject = CreateUIObject("Label", closeButton.transform);
        RectTransform closeLabelRect = closeLabelObject.AddComponent<RectTransform>();
        closeLabelRect.anchorMin = Vector2.zero;
        closeLabelRect.anchorMax = Vector2.one;
        closeLabelRect.offsetMin = Vector2.zero;
        closeLabelRect.offsetMax = Vector2.zero;

        Text closeLabel = closeLabelObject.AddComponent<Text>();
        closeLabel.font = GetBuiltinFont();
        closeLabel.text = "X";
        closeLabel.alignment = TextAnchor.MiddleCenter;
        closeLabel.color = Color.white;

        GameObject listObject = CreateUIObject(BossHornListName, panel.transform);
        listRoot = listObject.AddComponent<RectTransform>();
        listRoot.anchorMin = new Vector2(0f, 0f);
        listRoot.anchorMax = new Vector2(1f, 1f);
        listRoot.offsetMin = new Vector2(20f, 20f);
        listRoot.offsetMax = new Vector2(-20f, -54f);

        panel.SetActive(false);
        return panel;
    }

    private void OnMainButtonClicked()
    {
        SetupUI();

        if (bossPanelRoot != null && bossPanelRoot.activeSelf)
        {
            bossPanelRoot.SetActive(false);
            return;
        }

        OpenBossPanel();
    }

    private void OpenPanelFromBuilding()
    {
        SetupUI();
        OpenBossPanel();
    }

    private void OpenBossPanel()
    {
        if (!IsBossHornBuilt())
        {
            ShowAlarm(MissingBuildingMessage);
            if (bossPanelRoot != null)
                bossPanelRoot.SetActive(false);
            return;
        }

        List<BossEntry> entries = GatherBossEntries();
        RebuildBossButtons(entries);

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(true);
    }

    private bool IsBossHornBuilt()
    {
        BuildingWorldObject_YHJ[] buildings = FindObjectsOfType<BuildingWorldObject_YHJ>();
        foreach (BuildingWorldObject_YHJ building in buildings)
        {
            if (building != null && building.buildingType == BuildingType_YHJ.BossHorn)
                return true;
        }

        return false;
    }

    private List<BossEntry> GatherBossEntries()
    {
        List<BossEntry> entries = new List<BossEntry>();
        BossSpawner_JBJ[] spawners = FindObjectsOfType<BossSpawner_JBJ>();

        foreach (BossSpawner_JBJ spawner in spawners)
        {
            if (spawner == null || spawner.bossPrefab == null)
                continue;

            entries.Add(new BossEntry
            {
                spawner = spawner,
                bossPrefab = spawner.bossPrefab,
                displayName = GetBossDisplayName(spawner.bossPrefab)
            });
        }

        if (entries.Count == 0)
        {
            BossSpawner_JBJ fallbackSpawner = FindObjectOfType<BossSpawner_JBJ>();
            if (fallbackSpawner != null)
            {
                entries.Add(new BossEntry
                {
                    spawner = fallbackSpawner,
                    bossPrefab = fallbackSpawner.bossPrefab,
                    displayName = fallbackSpawner.bossPrefab != null ? GetBossDisplayName(fallbackSpawner.bossPrefab) : "Boss"
                });
            }
        }

        return entries;
    }

    private void RebuildBossButtons(List<BossEntry> entries)
    {
        if (bossButtonListRoot == null)
            return;

        for (int i = bossButtonListRoot.childCount - 1; i >= 0; i--)
        {
            Destroy(bossButtonListRoot.GetChild(i).gameObject);
        }

        for (int i = 0; i < entries.Count; i++)
        {
            CreateBossNameButton(entries[i], i);
        }
    }

    private void CreateBossNameButton(BossEntry entry, int index)
    {
        GameObject buttonObject = CreateUIObject(entry.displayName + "_Button", bossButtonListRoot);
        RectTransform rect = buttonObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.anchoredPosition = new Vector2(0f, -(index * 56f));
        rect.sizeDelta = new Vector2(0f, 46f);

        Image image = buttonObject.AddComponent<Image>();
        image.sprite = GetWhiteSprite();
        image.color = new Color(0.25f, 0.18f, 0.1f, 0.96f);

        Button button = buttonObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => SpawnBoss(entry));

        GameObject labelObject = CreateUIObject("Label", buttonObject.transform);
        RectTransform labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(12f, 4f);
        labelRect.offsetMax = new Vector2(-12f, -4f);

        Text label = labelObject.AddComponent<Text>();
        label.font = GetBuiltinFont();
        label.text = entry.displayName;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.resizeTextForBestFit = true;
        label.resizeTextMinSize = 15;
        label.resizeTextMaxSize = 22;
    }

    private void SpawnBoss(BossEntry entry)
    {
        if (entry.spawner == null)
            return;

        if (entry.bossPrefab != null)
            entry.spawner.bossPrefab = entry.bossPrefab;

        entry.spawner.SpawnBossFromTown();

        if (bossPanelRoot != null)
            bossPanelRoot.SetActive(false);
    }

    private void ShowAlarm(string message)
    {
        if (alarmGraphic == null)
            alarmGraphic = FindAlarmGraphic();

        if (alarmGraphic == null)
        {
            Debug.LogWarning("[BossHornRuntimeUI_YHJ] Alarm text not found: " + message);
            return;
        }

        if (alarmCoroutine != null)
            StopCoroutine(alarmCoroutine);

        alarmCoroutine = StartCoroutine(ShowAlarmRoutine(message));
    }

    private IEnumerator ShowAlarmRoutine(string message)
    {
        bool previousActive = alarmGraphic.gameObject.activeSelf;
        string previousMessage = GetGraphicText(alarmGraphic);

        SetGraphicText(alarmGraphic, message);
        alarmGraphic.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        SetGraphicText(alarmGraphic, previousMessage);
        alarmGraphic.gameObject.SetActive(previousActive);
        alarmCoroutine = null;
    }

    private static string GetGraphicText(Graphic graphic)
    {
        TMP_Text tmp = graphic as TMP_Text;
        if (tmp != null)
            return tmp.text;

        Text legacy = graphic as Text;
        return legacy != null ? legacy.text : string.Empty;
    }

    private static void SetGraphicText(Graphic graphic, string text)
    {
        TMP_Text tmp = graphic as TMP_Text;
        if (tmp != null)
        {
            tmp.text = text;
            return;
        }

        Text legacy = graphic as Text;
        if (legacy != null)
            legacy.text = text;
    }

    private GameObject FindSceneObject(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform candidate in transforms)
        {
            if (candidate == null || candidate.name != objectName)
                continue;

            if (!candidate.gameObject.scene.IsValid() || !candidate.gameObject.scene.isLoaded)
                continue;

            if (candidate.gameObject.scene.name != TargetSceneName)
                continue;

            return candidate.gameObject;
        }

        return null;
    }

    private void DestroyDuplicateObjects(string objectName, GameObject keepObject)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
        foreach (Transform candidate in transforms)
        {
            if (candidate == null || candidate.name != objectName)
                continue;

            if (!candidate.gameObject.scene.IsValid() || !candidate.gameObject.scene.isLoaded)
                continue;

            if (candidate.gameObject.scene.name != TargetSceneName)
                continue;

            if (keepObject != null && candidate.gameObject == keepObject)
                continue;

            Destroy(candidate.gameObject);
        }
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        GameObject go = new GameObject(name);
        go.layer = 5;
        go.transform.SetParent(parent, false);
        return go;
    }

    private static Sprite GetWhiteSprite()
    {
        if (cachedWhiteSprite != null)
            return cachedWhiteSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        cachedWhiteSprite = Sprite.Create(
            texture,
            new Rect(0f, 0f, 1f, 1f),
            new Vector2(0.5f, 0.5f),
            1f);

        return cachedWhiteSprite;
    }

    private static Sprite GetWhiteCircleSprite()
    {
        Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        Vector2 center = new Vector2(31.5f, 31.5f);
        float radius = 30f;

        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center);
                texture.SetPixel(x, y, distance <= radius ? Color.white : Color.clear);
            }
        }

        texture.Apply();

        return Sprite.Create(
            texture,
            new Rect(0f, 0f, 64f, 64f),
            new Vector2(0.5f, 0.5f),
            64f);
    }

    private static Sprite GetBossHornSprite()
    {
        if (cachedHornSprite != null)
            return cachedHornSprite;

        Sprite[] sprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (Sprite sprite in sprites)
        {
            if (sprite != null && sprite.name == BossHornSpriteName)
            {
                cachedHornSprite = sprite;
                return cachedHornSprite;
            }
        }

        return GetWhiteSprite();
    }

    private static Font GetBuiltinFont()
    {
        if (cachedFont != null)
            return cachedFont;

        cachedFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        return cachedFont;
    }

    private static string GetBossDisplayName(GameObject bossPrefab)
    {
        if (bossPrefab == null)
            return "Boss";

        if (bossPrefab.name == "BossLycan")
            return "Boss Lycan";

        return bossPrefab.name.Replace("_", " ");
    }

    private struct BossEntry
    {
        public BossSpawner_JBJ spawner;
        public GameObject bossPrefab;
        public string displayName;
    }
}

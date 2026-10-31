using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed class MainMenuScreen : MonoBehaviour
    {
        private MainMenuTheme theme;
        private TMP_FontAsset runtimeFont;
        private RectTransform root, stage, settings, quit, resolutionList;
        private Button startButton, settingsButton, quitButton;
        private TMP_Text status, musicLabel, soundLabel, resolutionLabel;
        private Slider music, sound;
        private Toggle fullscreen;
        private readonly List<Toggle> languageToggles = new List<Toggle>();
        private readonly Dictionary<TMP_Text, string> copy = new Dictionary<TMP_Text, string>();
        private readonly Color ink = new Color(.20f, .29f, .32f);
        private readonly Color blue = new Color(.34f, .49f, .57f);
        private CanvasGroup entrance;
        private float entered;
        private bool loading;

        public static void TryInstall(SceneNavigation owner)
        {
            if (owner.gameObject.scene.name != "MainMenu" || FindAnyObjectByType<MainMenuScreen>() != null) return;
            var data = Resources.Load<MainMenuTheme>("Menu/MainMenuTheme");
            if (data == null || data.chineseFont == null || data.background == null)
            { Debug.LogWarning("Main menu theme not imported. Use Tools/Story Prototype/Install Main Menu."); return; }
            new GameObject("Main Menu Screen").AddComponent<MainMenuScreen>().theme = data;
        }
        private RectTransform Rect(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero;
            r.anchoredPosition = pos; r.sizeDelta = size; return r;
        }
        private Image Fill(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            var r = Rect(name, parent, pos, size); var g = r.gameObject.AddComponent<Image>(); g.color = color; return g;
        }
        private MenuInkGraphic Draw(string name, Transform parent, Vector2 pos, Vector2 size, MenuInkGraphic.Shape shape, Color color)
        {
            var r = Rect(name, parent, pos, size); var g = r.gameObject.AddComponent<MenuInkGraphic>();
            g.kind = shape; g.color = color; g.raycastTarget = false; return g;
        }
        private TMP_Text Text(Transform parent, Vector2 pos, Vector2 size, string value, int points, Color? color = null)
        {
            var r = Rect("Text", parent, pos, size); var t = r.gameObject.AddComponent<TextMeshProUGUI>();
            t.font = runtimeFont != null ? runtimeFont : theme.chineseFont; t.fontSize = points; t.text = value; t.color = color ?? ink;
            t.alignment = TextAlignmentOptions.MidlineLeft; t.raycastTarget = false;
            if (parent == stage)
            { var shadow = r.gameObject.AddComponent<Shadow>(); shadow.effectColor = new Color(1, .98f, .92f, .8f); shadow.effectDistance = new Vector2(1, -1); }
            return t;
        }
        private TMP_Text Local(Transform parent, Vector2 pos, Vector2 size, string key, int points, Color? color = null)
        { var t = Text(parent, pos, size, MenuCopy.Get(key), points, color); copy[t] = key; return t; }
        private Button MakeButton(Transform parent, Vector2 pos, Vector2 size, string key, UnityEngine.Events.UnityAction action, bool center = true)
        {
            var hit = Fill(key, parent, pos, size, new Color(1, 1, 1, 0));
            var b = hit.gameObject.AddComponent<Button>(); b.targetGraphic = hit; b.transition = Selectable.Transition.None; b.onClick.AddListener(action);
            var t = Local(hit.transform, size * .5f, size - new Vector2(48, 8), key, center ? 26 : 36);
            t.alignment = center ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft;
            float lineWidth = Mathf.Min(size.x - 48, 190);
            var line = Draw("Pencil underline", hit.transform, new Vector2(center ? (size.x - lineWidth) / 2 : 24, 8), new Vector2(lineWidth, 4), MenuInkGraphic.Shape.Stroke, blue);
            line.rectTransform.pivot = new Vector2(0, .5f); line.rectTransform.localScale = new Vector3(.001f, 1, 1);
            var hover = hit.gameObject.AddComponent<MenuHoverInk>(); hover.stroke = line.rectTransform; hover.label = t; hover.resting = ink; hover.active = blue;
            return b;
        }
        private void Start()
        {
            if (theme == null) { enabled = false; return; }
            // Dynamic glyphs belong to this view, avoiding changes to the shared font asset in Play.
            if (theme.chineseFont.sourceFontFile != null)
            { runtimeFont = TMP_FontAsset.CreateFontAsset(theme.chineseFont.sourceFontFile); if (runtimeFont != null) runtimeFont.hideFlags = HideFlags.DontSave; }
            foreach (var old in FindObjectsByType<Canvas>()) if (old.gameObject.scene == gameObject.scene) old.gameObject.SetActive(false);
            if (FindAnyObjectByType<EventSystem>() == null) new GameObject("Menu EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            MenuPreferences.Apply();
            var canvas = new GameObject("Illustrated Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            root = Rect("Full classroom", canvas.transform, Vector2.zero, Vector2.zero);
            root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = root.offsetMax = Vector2.zero;
            var art = Rect("Classroom Background", root, Vector2.zero, Vector2.zero);
            art.anchorMin = Vector2.zero; art.anchorMax = Vector2.one; art.offsetMin = art.offsetMax = Vector2.zero;
            var raw = art.gameObject.AddComponent<RawImage>(); raw.texture = theme.background; raw.raycastTarget = false;
            var fit = art.gameObject.AddComponent<AspectRatioFitter>(); fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fit.aspectRatio = (float)theme.background.width / theme.background.height;
            stage = Rect("Composition", root, Vector2.zero, new Vector2(1920, 1080)); stage.anchorMin = stage.anchorMax = Vector2.one * .5f;
            entrance = stage.gameObject.AddComponent<CanvasGroup>(); entrance.alpha = 0;
            Local(stage, new Vector2(385, 884), new Vector2(490, 42), "edition", 19, blue);
            var title = Local(stage, new Vector2(400, 775), new Vector2(520, 140), "title", 77); title.characterSpacing = 7;
            Draw("Title pencil stroke", stage, new Vector2(207, 681), new Vector2(135, 3), MenuInkGraphic.Shape.Stroke, blue);
            Local(stage, new Vector2(400, 615), new Vector2(520, 90), "tagline", 25);
            startButton = MakeButton(stage, new Vector2(345, 443), new Vector2(450, 76), "start", StartGame, false);
            settingsButton = MakeButton(stage, new Vector2(345, 350), new Vector2(450, 76), "settings", () => ShowSettings(true), false);
            quitButton = MakeButton(stage, new Vector2(345, 257), new Vector2(450, 76), "quit", () => ShowQuit(true), false);
            status = Local(stage, new Vector2(410, 121), new Vector2(540, 44), "demo", 20);
            Local(stage, new Vector2(410, 78), new Vector2(540, 36), "storyNote", 17, new Color(.36f, .40f, .39f));
            BuildSettings(); BuildQuit(); Select(startButton);
        }
        private RectTransform Modal(string name, Vector2 size)
        {
            var shade = Fill(name, stage, new Vector2(960, 540), new Vector2(8000, 5000), new Color(.18f, .23f, .25f, .47f));
            Draw("Paper shadow", shade.transform, new Vector2(4008, 2491), size, MenuInkGraphic.Shape.Paper, new Color(.12f, .19f, .21f, .24f));
            var paper = Draw("Exercise paper", shade.transform, new Vector2(4000, 2500), size, MenuInkGraphic.Shape.Paper, new Color(.96f, .96f, .92f));
            // Children use the bottom-left of this rectangular paper as a stable layout origin.
            return paper.rectTransform;
        }
        private Slider MakeSlider(Transform parent, float x, float y, string key, float initial, UnityEngine.Events.UnityAction<float> changed, out TMP_Text label)
        {
            label = Local(parent, new Vector2(x, y + 50), new Vector2(480, 48), key, 28, blue);
            var track = Fill(key + " slider", parent, new Vector2(x, y), new Vector2(480, 38), new Color(1, 1, 1, 0));
            Draw("Pencil rail", track.transform, new Vector2(240, 19), new Vector2(480, 5), MenuInkGraphic.Shape.Stroke, new Color(.68f, .72f, .70f));
            var slider = track.gameObject.AddComponent<Slider>(); slider.minValue = 0; slider.maxValue = 1;
            var fill = Draw("Ink fill", track.transform, Vector2.zero, Vector2.zero, MenuInkGraphic.Shape.Stroke, blue);
            fill.rectTransform.anchorMin = Vector2.zero; fill.rectTransform.anchorMax = Vector2.one; fill.rectTransform.offsetMin = new Vector2(0, 17); fill.rectTransform.offsetMax = new Vector2(0, -17);
            var handle = Draw("Round pencil handle", track.transform, Vector2.zero, new Vector2(26, 26), MenuInkGraphic.Shape.Dot, ink);
            slider.fillRect = fill.rectTransform; slider.handleRect = handle.rectTransform; slider.targetGraphic = handle;
            slider.SetValueWithoutNotify(initial); handle.rectTransform.sizeDelta = new Vector2(26, -12);
            slider.onValueChanged.AddListener(changed); return slider;
        }
        private Toggle MakeToggle(Transform parent, Vector2 pos, Vector2 size, string value, bool initial, UnityEngine.Events.UnityAction<bool> changed)
        {
            var r = Fill(value, parent, pos, size, new Color(1, 1, 1, 0)); var t = r.gameObject.AddComponent<Toggle>();
            Draw("Checkbox", r.transform, new Vector2(27, size.y / 2), new Vector2(30, 30), MenuInkGraphic.Shape.Box, blue);
            var tick = Draw("Pencil check", r.transform, new Vector2(27, size.y / 2), new Vector2(25, 25), MenuInkGraphic.Shape.Tick, ink);
            t.targetGraphic = r; t.graphic = tick; t.SetIsOnWithoutNotify(initial);
            Text(r.transform, new Vector2(size.x / 2 + 30, size.y / 2), new Vector2(size.x - 75, size.y - 4), value, 29);
            t.onValueChanged.AddListener(changed); return t;
        }
        private void BuildSettings()
        {
            settings = Modal("Settings overlay", new Vector2(1380, 820));
            Local(settings, new Vector2(380, 727), new Vector2(520, 70), "settings", 43);
            Draw("Header rule", settings, new Vector2(690, 672), new Vector2(1140, 3), MenuInkGraphic.Shape.Stroke, new Color(.49f, .61f, .65f, .45f));
            music = MakeSlider(settings, 360, 560, "music", MenuPreferences.Music, v => { MenuPreferences.SetMusic(v); RefreshValues(); }, out musicLabel);
            sound = MakeSlider(settings, 1010, 560, "sound", MenuPreferences.Sound, v => { MenuPreferences.SetSound(v); RefreshValues(); }, out soundLabel);
            Local(settings, new Vector2(240, 446), new Vector2(240, 50), "resolution", 28, blue);
            var rb = MakeButton(settings, new Vector2(515, 446), new Vector2(300, 62), "resolution", () => resolutionList.gameObject.SetActive(!resolutionList.gameObject.activeSelf));
            resolutionLabel = rb.GetComponentInChildren<TMP_Text>(); copy.Remove(resolutionLabel);
            Draw("Resolution field outline", rb.transform, new Vector2(150, 31), new Vector2(292, 56), MenuInkGraphic.Shape.Box, blue);
            Draw("Dropdown arrow", rb.transform, new Vector2(271, 31), new Vector2(16, 9), MenuInkGraphic.Shape.Chevron, blue);
            fullscreen = MakeToggle(settings, new Vector2(1020, 446), new Vector2(470, 62), MenuCopy.Get("fullscreen"), MenuPreferences.Fullscreen, MenuPreferences.SetFullscreen);
            copy[fullscreen.GetComponentInChildren<TMP_Text>()] = "fullscreen";
            Draw("Section rule", settings, new Vector2(690, 373), new Vector2(1140, 2), MenuInkGraphic.Shape.Stroke, new Color(.49f, .61f, .65f, .25f));
            Local(settings, new Vector2(240, 310), new Vector2(240, 45), "language", 28, blue);
            var group = settings.gameObject.AddComponent<ToggleGroup>(); group.allowSwitchOff = false;
            for (int i = 0; i < 4; i++)
            {
                int index = i;
                var t = MakeToggle(settings, new Vector2(525 + (i / 2) * 500, 306 - (i % 2) * 92), new Vector2(410, 65), MenuCopy.LanguageNames[i], i == (int)MenuPreferences.Language,
                    on => { if (on) { MenuPreferences.SetLanguage((MenuLanguage)index); RefreshLanguage(); } });
                t.group = group; languageToggles.Add(t);
            }
            Local(settings, new Vector2(470, 80), new Vector2(700, 45), "saved", 19, blue);
            MakeButton(settings, new Vector2(900, 80), new Vector2(270, 64), "reset", ResetPreferences);
            MakeButton(settings, new Vector2(1175, 80), new Vector2(220, 64), "back", () => ShowSettings(false));
            BuildResolutionList(); RefreshValues(); settings.parent.gameObject.SetActive(false);
        }
        private void BuildResolutionList()
        {
            resolutionList = Fill("Resolution choices", settings, new Vector2(515, 315), new Vector2(300, 196), new Color(.96f, .96f, .92f)).rectTransform;
            var options = new List<Vector2Int>();
            int maxWidth = Mathf.Max(1280, Screen.currentResolution.width), maxHeight = Mathf.Max(720, Screen.currentResolution.height);
            foreach (var size in new[] { new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080), new Vector2Int(2560, 1440), new Vector2Int(3840, 2160) })
                if (size.x <= maxWidth && size.y <= maxHeight) options.Add(size);
            var native = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
            if (native.x >= 640 && native.y >= 360 && !options.Contains(native)) options.Add(native);
            resolutionList.sizeDelta = new Vector2(300, options.Count * 55 + 12);
            resolutionList.anchoredPosition = new Vector2(515, 415 - resolutionList.sizeDelta.y / 2);
            for (int i = 0; i < options.Count; i++)
            {
                var size = options[i]; var b = MakeButton(resolutionList, new Vector2(150, resolutionList.sizeDelta.y - 34 - i * 55), new Vector2(300, 54), "resolution",
                    () => { MenuPreferences.SetResolution(size.x, size.y); RefreshValues(); resolutionList.gameObject.SetActive(false); });
                var text = b.GetComponentInChildren<TMP_Text>(); copy.Remove(text); text.text = size.x + " × " + size.y; text.fontSize = 25;
            }
            resolutionList.gameObject.SetActive(false);
        }
        private void BuildQuit()
        {
            quit = Modal("Quit overlay", new Vector2(920, 400));
            Local(quit, new Vector2(460, 265), new Vector2(740, 90), "confirm", 38).alignment = TextAlignmentOptions.Center;
            MakeButton(quit, new Vector2(280, 105), new Vector2(360, 72), "cancel", () => ShowQuit(false));
            MakeButton(quit, new Vector2(680, 105), new Vector2(240, 72), "exit", QuitGame);
            quit.parent.gameObject.SetActive(false);
        }
        private void RefreshLanguage()
        {
            foreach (var entry in copy) if (entry.Key != null) entry.Key.text = MenuCopy.Get(entry.Value);
            RefreshValues();
        }
        private void RefreshValues()
        {
            musicLabel.text = MenuCopy.Get("music") + "  " + Mathf.RoundToInt(MenuPreferences.Music * 100) + "%";
            soundLabel.text = MenuCopy.Get("sound") + "  " + Mathf.RoundToInt(MenuPreferences.Sound * 100) + "%";
            resolutionLabel.text = MenuPreferences.Width + " × " + MenuPreferences.Height;
        }
        private void ResetPreferences()
        {
            MenuPreferences.RestoreDefaults(); music.SetValueWithoutNotify(MenuPreferences.Music); sound.SetValueWithoutNotify(MenuPreferences.Sound);
            fullscreen.SetIsOnWithoutNotify(MenuPreferences.Fullscreen); RefreshValues(); resolutionList.gameObject.SetActive(false);
        }
        private void LockMenu(bool locked) { startButton.interactable = settingsButton.interactable = quitButton.interactable = !locked; }
        private void ShowSettings(bool on)
        { settings.parent.gameObject.SetActive(on); resolutionList.gameObject.SetActive(false); LockMenu(on); if (on) Select(music); else Select(settingsButton); }
        private void ShowQuit(bool on)
        { quit.parent.gameObject.SetActive(on); LockMenu(on); if (on) Select(quit.GetComponentInChildren<Button>()); else Select(quitButton); }
        private static void Select(Selectable b) { if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(b.gameObject); }
        private void Update()
        {
            if (entrance == null) return;
            entered += Time.unscaledDeltaTime; entrance.alpha = Mathf.SmoothStep(0, 1, Mathf.Clamp01(entered / .65f));
            if (loading || Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
            if (resolutionList.gameObject.activeSelf) resolutionList.gameObject.SetActive(false);
            else if (settings.parent.gameObject.activeSelf) ShowSettings(false);
            else if (quit.parent.gameObject.activeSelf) ShowQuit(false); else ShowQuit(true);
        }
        private void StartGame()
        {
            if (loading) return;
            string target = Application.CanStreamedLevelBeLoaded("TableSlice") ? "TableSlice" : "SampleScene";
            if (!Application.CanStreamedLevelBeLoaded(target)) { status.text = MenuCopy.Get("unavailable"); return; }
            loading = true; LockMenu(true); status.text = MenuCopy.Get("loading"); SceneManager.LoadSceneAsync(target);
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            ShowQuit(false); status.text = MenuCopy.Get("editorExit");
#else
            Application.Quit();
#endif
        }
        private void OnDestroy()
        {
            if (runtimeFont == null) return;
            foreach (var texture in runtimeFont.atlasTextures) if (texture != null) Destroy(texture);
            Destroy(runtimeFont);
        }
    }
}

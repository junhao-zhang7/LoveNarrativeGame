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
    // Builds actual UGUI controls over an independent illustration (no baked-in buttons).
    public sealed class MainMenuScreen : MonoBehaviour
    {
        private MainMenuTheme theme;
        private SceneNavigation navigation;
        private RectTransform root, settings, quit;
        private Button startButton, settingsButton, quitButton;
        private TMP_Text status, volumeLabel;
        private Slider volume;
        private Toggle fullscreen, largeText;
        private readonly Dictionary<TMP_Text, float> fontSizes = new Dictionary<TMP_Text, float>();
        private readonly Color ink = new Color(.18f, .26f, .28f);
        public static void TryInstall(SceneNavigation owner)
        {
            if (owner.gameObject.scene.name != "MainMenu" || FindAnyObjectByType<MainMenuScreen>() != null) return;
            var data = Resources.Load<MainMenuTheme>("Menu/MainMenuTheme");
            if (data == null || data.chineseFont == null || data.background == null)
            { Debug.LogWarning("Main menu theme not imported yet. Use Tools/Story Prototype/Install Main Menu."); return; }
            var view = new GameObject("Main Menu Screen").AddComponent<MainMenuScreen>();
            view.theme = data; view.navigation = owner;
        }
        private RectTransform Panel(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero;
            r.anchoredPosition = pos; r.sizeDelta = size; go.GetComponent<Image>().color = color; return r;
        }
        private TMP_Text Text(Transform parent, Vector2 pos, Vector2 size, string value, int points, Color? color = null)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero; r.anchoredPosition = pos; r.sizeDelta = size;
            var t = go.GetComponent<TMP_Text>(); t.font = theme.chineseFont; t.fontSize = points * MenuPreferences.TextScale;
            t.text = value; t.color = color ?? ink; t.alignment = TextAlignmentOptions.MidlineLeft; t.raycastTarget = false;
            fontSizes[t] = points; return t;
        }
        private Button MakeButton(Transform parent, Vector2 pos, Vector2 size, string title, UnityEngine.Events.UnityAction action, bool primary = false)
        {
            var r = Panel(title, parent, pos, size, primary ? new Color(.29f, .43f, .46f) : new Color(.98f, .96f, .91f, .9f));
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = r.GetComponent<Image>();
            var colors = b.colors; colors.highlightedColor = new Color(.83f, .9f, .86f); colors.pressedColor = new Color(.68f, .79f, .75f); b.colors = colors;
            b.onClick.AddListener(action);
            var t = Text(r, size * .5f, size - new Vector2(40, 8), title, 29, primary ? Color.white : ink);
            t.alignment = TextAlignmentOptions.Center; return b;
        }
        private void Start()
        {
            if (theme == null || navigation == null) { enabled = false; return; }
            // The old scene remains intact on disk; disable only its runtime presentation.
            foreach (var old in FindObjectsByType<Canvas>())
                if (old.gameObject.scene == gameObject.scene) old.gameObject.SetActive(false);
            if (FindAnyObjectByType<EventSystem>() == null) new GameObject("Menu EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            MenuPreferences.Apply();
            var canvas = new GameObject("Illustrated Menu Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 1;
            root = Panel("Root", canvas.transform, Vector2.zero, Vector2.zero, Color.white);
            root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = root.offsetMax = Vector2.zero;
            var art = new GameObject("Classroom Background", typeof(RectTransform), typeof(RawImage), typeof(AspectRatioFitter)); art.transform.SetParent(root, false);
            var ar = (RectTransform)art.transform; ar.anchorMin = Vector2.zero; ar.anchorMax = Vector2.one; ar.offsetMin = ar.offsetMax = Vector2.zero;
            var raw = art.GetComponent<RawImage>(); raw.texture = theme.background; raw.raycastTarget = false;
            var fit = art.GetComponent<AspectRatioFitter>(); fit.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            fit.aspectRatio = (float)theme.background.width / theme.background.height;
            Panel("Paper menu", root, new Vector2(320, 540), new Vector2(640, 1080), new Color(.97f, .95f, .9f, .94f));
            Text(root, new Vector2(320, 908), new Vector2(420, 50), "原创校园叙事 · 开发版", 23, new Color(.43f, .49f, .48f));
            Text(root, new Vector2(320, 805), new Vector2(420, 105), "未定名", 68);
            Panel("Rule", root, new Vector2(173, 715), new Vector2(125, 3), new Color(.63f, .49f, .36f));
            Text(root, new Vector2(320, 653), new Vector2(420, 80), "从一次换座开始的故事", 26);
            startButton = MakeButton(root, new Vector2(320, 500), new Vector2(420, 80), "开始游戏", StartGame, true);
            settingsButton = MakeButton(root, new Vector2(320, 390), new Vector2(420, 80), "设置", () => ShowSettings(true));
            quitButton = MakeButton(root, new Vector2(320, 280), new Vector2(420, 80), "退出游戏", () => ShowQuit(true));
            status = Text(root, new Vector2(320, 146), new Vector2(420, 85), "当前可体验：数学讲题与英语互助\n完整主线制作中", 20, new Color(.43f, .49f, .48f));
            Text(root, new Vector2(320, 60), new Vector2(420, 34), "Windows · 离线演示", 18, new Color(.43f, .49f, .48f));
            BuildSettings(); BuildQuit(); Select(startButton);
        }
        private RectTransform Modal(string name)
        {
            var shade = Panel(name, root, Vector2.zero, Vector2.zero, new Color(.12f, .18f, .19f, .62f));
            shade.anchorMin = Vector2.zero; shade.anchorMax = Vector2.one; shade.offsetMin = shade.offsetMax = Vector2.zero;
            var paper = Panel("Dialog paper", shade, Vector2.zero, new Vector2(780, 660), new Color(.98f, .96f, .91f));
            paper.anchorMin = paper.anchorMax = new Vector2(.5f, .5f); return paper;
        }
        private Toggle MakeToggle(Transform parent, Vector2 pos, string label, bool initial, UnityEngine.Events.UnityAction<bool> changed)
        {
            var r = Panel(label, parent, pos, new Vector2(620, 70), new Color(.9f, .9f, .85f));
            var t = r.gameObject.AddComponent<Toggle>();
            Panel("Check background", r, new Vector2(37, 35), new Vector2(34, 34), new Color(.98f, .96f, .91f));
            var check = Panel("Check", r, new Vector2(37, 35), new Vector2(25, 25), new Color(.29f, .43f, .46f));
            t.targetGraphic = r.GetComponent<Image>(); t.graphic = check.GetComponent<Image>(); t.SetIsOnWithoutNotify(initial);
            Text(r, new Vector2(340, 35), new Vector2(515, 60), label, 26); t.onValueChanged.AddListener(changed); return t;
        }
        private void BuildSettings()
        {
            settings = Modal("Settings overlay");
            Text(settings, new Vector2(390, 585), new Vector2(620, 60), "设置", 38);
            volumeLabel = Text(settings, new Vector2(390, 493), new Vector2(620, 45), "", 26);
            var track = Panel("Volume", settings, new Vector2(390, 434), new Vector2(620, 40), new Color(.78f, .8f, .76f));
            volume = track.gameObject.AddComponent<Slider>(); volume.minValue = 0; volume.maxValue = 1;
            var fill = Panel("Fill", track, Vector2.zero, Vector2.zero, new Color(.44f, .6f, .59f));
            fill.anchorMin = Vector2.zero; fill.anchorMax = Vector2.one; fill.offsetMin = fill.offsetMax = Vector2.zero;
            var handle = Panel("Handle", track, Vector2.zero, new Vector2(22, 48), ink);
            volume.fillRect = fill; volume.handleRect = handle; volume.targetGraphic = handle.GetComponent<Image>();
            volume.SetValueWithoutNotify(MenuPreferences.Volume); UpdateVolumeLabel();
            volume.onValueChanged.AddListener(v => { MenuPreferences.SetVolume(v); UpdateVolumeLabel(); });
            fullscreen = MakeToggle(settings, new Vector2(390, 340), "全屏显示", MenuPreferences.Fullscreen, MenuPreferences.SetFullscreen);
            largeText = MakeToggle(settings, new Vector2(390, 250), "较大文字", MenuPreferences.TextScale > 1, v => { MenuPreferences.SetLargeText(v); ApplyTextScale(); });
            Text(settings, new Vector2(390, 174), new Vector2(620, 55), "设置自动保存", 19);
            MakeButton(settings, new Vector2(235, 84), new Vector2(280, 66), "恢复默认", () => {
                MenuPreferences.RestoreDefaults(); volume.SetValueWithoutNotify(MenuPreferences.Volume);
                fullscreen.SetIsOnWithoutNotify(false); largeText.SetIsOnWithoutNotify(false); UpdateVolumeLabel(); ApplyTextScale();
            });
            MakeButton(settings, new Vector2(545, 84), new Vector2(280, 66), "返回", () => ShowSettings(false), true);
            settings.parent.gameObject.SetActive(false);
        }
        private void BuildQuit()
        {
            quit = Modal("Quit overlay"); quit.sizeDelta = new Vector2(780, 380);
            Text(quit, new Vector2(390, 260), new Vector2(620, 80), "确定退出游戏？", 36);
            MakeButton(quit, new Vector2(235, 100), new Vector2(280, 72), "取消", () => ShowQuit(false));
            MakeButton(quit, new Vector2(545, 100), new Vector2(280, 72), "退出", QuitGame, true);
            quit.parent.gameObject.SetActive(false);
        }
        private void UpdateVolumeLabel() { volumeLabel.text = "总音量  " + Mathf.RoundToInt(MenuPreferences.Volume * 100) + "%"; }
        private void ApplyTextScale() { foreach (var pair in fontSizes) if (pair.Key != null) pair.Key.fontSize = pair.Value * MenuPreferences.TextScale; }
        private void LockMenu(bool value) { startButton.interactable = settingsButton.interactable = quitButton.interactable = !value; }
        private void ShowSettings(bool value)
        {
            settings.parent.gameObject.SetActive(value); LockMenu(value);
            if (value) { EventSystem.current.SetSelectedGameObject(volume.gameObject); } else Select(settingsButton);
        }
        private void ShowQuit(bool value)
        {
            quit.parent.gameObject.SetActive(value); LockMenu(value);
            if (value) Select(quit.GetComponentInChildren<Button>()); else Select(quitButton);
        }
        private static void Select(Button b) { if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(b.gameObject); }
        private void Update()
        {
            if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame) return;
            if (settings.parent.gameObject.activeSelf) ShowSettings(false);
            else if (quit.parent.gameObject.activeSelf) ShowQuit(false);
            else ShowQuit(true);
        }
        private void StartGame()
        {
            string target = Application.CanStreamedLevelBeLoaded("TableSlice") ? "TableSlice" : "SampleScene";
            if (!Application.CanStreamedLevelBeLoaded(target)) { status.text = "暂时无法开始，请检查场景是否加入构建。"; return; }
            LockMenu(true); status.text = "正在进入……"; SceneManager.LoadSceneAsync(target);
        }
        private void QuitGame()
        {
#if UNITY_EDITOR
            ShowQuit(false); status.text = "编辑器测试：请停止 Play。独立程序会正常退出。";
#else
            Application.Quit();
#endif
        }
    }
}

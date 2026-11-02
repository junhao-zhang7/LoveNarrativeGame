using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed class StoryScreen : MonoBehaviour
    {
        private MainMenuTheme theme;
        private StoryBeat[] beats;
        private StoryBook[] books;
        private int bookIndex;
        private int chapterIndex;
        private StoryReadingState saved;
        private RectTransform chapterMenu;
        private ScrollRect chapterScroll;
        private TMP_Text saveNotice;
        private RectTransform dialoguePanel, mathPanel;
        private TMP_InputField mathInput;
        private TMP_Text mathFeedback;
        private bool saveFailed;
        private string SaveKey => "SchoolPrototype.Reading.V1." + books[bookIndex].bookId;
        private RectTransform cardArea;
        private Dictionary<StoryShot, Texture2D> shots;
        private Texture2D thumbnail;
        private Action closed;
        private TMP_FontAsset font;
        private RectTransform stage, selection, playback, ending, pause;
        private RawImage art;
        private RawImage dissolve;
        private TMP_Text speaker, body, nextLabel, endingSummary, nextPreview;
        private StoryProgress progress;
        private Rect cameraTarget;
        private float typing;
        private bool transitioning, paused, reading;
        private StoryShot currentShot;
        private int lastAdvanceFrame = -1;
        private readonly Color ink = new Color(.20f, .29f, .32f);
        private readonly Color blue = new Color(.34f, .49f, .57f);

        public static bool Open(MainMenuTheme theme, Action closed)
        {
            var catalog = Resources.LoadAll<StoryBook>("Story/Catalog");
            if (catalog.Length == 0) return false;
            Array.Sort(catalog, (a, b) => string.CompareOrdinal(a.bookId, b.bookId));
            foreach (var book in catalog)
            {
                if (book.cover == null || book.chapters == null || book.chapters.Length == 0 || book.pictures == null) return false;
                var available = new Dictionary<StoryShot, Texture2D>();
                foreach (var picture in book.pictures)
                { if (picture == null || picture.texture == null || available.ContainsKey(picture.shot)) return false; available.Add(picture.shot, picture.texture); }
                foreach (var chapter in book.chapters)
                {
                    if (chapter == null || !chapter.IsValid()) return false;
                    foreach (var beat in chapter.beats) if (!available.ContainsKey(beat.shot)) return false;
                }
            }
            var view = new GameObject("Story catalog and playback").AddComponent<StoryScreen>();
            view.theme = theme; view.books = catalog; view.closed = closed;
            view.Build(); return true;
        }
        private RectTransform Rect(string name, Transform parent, Vector2 pos, Vector2 size)
        {
            var go = new GameObject(name, typeof(RectTransform)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero; r.anchoredPosition = pos; r.sizeDelta = size; return r;
        }
        private Image Fill(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        { var r = Rect(name, parent, pos, size); var image = r.gameObject.AddComponent<Image>(); image.color = color; return image; }
        private RectTransform Paper(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            var r = Rect(name, parent, pos, size); var g = r.gameObject.AddComponent<MenuInkGraphic>();
            g.kind = MenuInkGraphic.Shape.Paper; g.color = color; g.raycastTarget = false; return r;
        }
        private TMP_Text Text(Transform parent, Vector2 pos, Vector2 size, string value, int points, bool center = false, Color? color = null)
        {
            var r = Rect("Text", parent, pos, size); var t = r.gameObject.AddComponent<TextMeshProUGUI>();
            t.font = font != null ? font : theme.chineseFont; t.fontSize = points; t.color = color ?? ink; t.text = value;
            t.alignment = center ? TextAlignmentOptions.Center : TextAlignmentOptions.MidlineLeft; t.raycastTarget = false; return t;
        }
        private Button Button(Transform parent, Vector2 pos, Vector2 size, string title, UnityEngine.Events.UnityAction click, int points = 26)
        {
            var r = Fill(title, parent, pos, size, new Color(1, 1, 1, 0));
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = r; b.transition = Selectable.Transition.None; b.onClick.AddListener(click);
            var text = Text(r.transform, size / 2, size - new Vector2(18, 8), title, points, true);
            var line = Rect("Underline", r.transform, new Vector2(15, 6), new Vector2(size.x - 30, 4)); line.pivot = new Vector2(0, .5f);
            var graphic = line.gameObject.AddComponent<MenuInkGraphic>(); graphic.kind = MenuInkGraphic.Shape.Stroke; graphic.color = blue; graphic.raycastTarget = false;
            line.localScale = new Vector3(.001f, 1, 1);
            var hover = r.gameObject.AddComponent<MenuHoverInk>(); hover.label = text; hover.stroke = line; hover.resting = ink; hover.active = blue; return b;
        }
        private void Build()
        {
            if (theme.chineseFont.sourceFontFile != null) font = TMP_FontAsset.CreateFontAsset(theme.chineseFont.sourceFontFile, 120, 12, UnityEngine.TextCore.LowLevel.GlyphRenderMode.SDFAA, 2048, 2048);
            if (font != null) font.hideFlags = HideFlags.DontSave;
            var canvas = gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.sortingOrder = 20;
            var scaler = gameObject.AddComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080); scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            gameObject.AddComponent<GraphicRaycaster>();
            if (FindAnyObjectByType<EventSystem>() == null) new GameObject("Story EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            var backdrop = Fill("Paper background", transform, Vector2.zero, Vector2.zero, new Color(.97f, .96f, .92f)).rectTransform;
            backdrop.anchorMin = Vector2.zero; backdrop.anchorMax = Vector2.one; backdrop.offsetMin = backdrop.offsetMax = Vector2.zero;
            stage = Rect("Story composition", transform, Vector2.zero, new Vector2(1920, 1080)); stage.anchorMin = stage.anchorMax = Vector2.one * .5f;
            selection = Rect("Choose story", stage, new Vector2(960, 540), new Vector2(1920, 1080));
            Text(selection, new Vector2(960, 945), new Vector2(1000, 100), MenuCopy.Get("chooseStory"), 48, true, blue);
            Text(selection, new Vector2(960, 865), new Vector2(1000, 45), MenuCopy.Get("storyNote"), 20, true, blue);
            Button(selection, new Vector2(1680, 955), new Vector2(300, 64), MenuCopy.Get("back"), Close);
            cardArea = Rect("Whole story cards", selection, new Vector2(960, 540), new Vector2(1920, 1080));
            BuildCard();
            if (books.Length > 1)
            {
                Button(selection, new Vector2(400, 460), new Vector2(180, 70), "←", () => ChangeBook(-1));
                Button(selection, new Vector2(1520, 460), new Vector2(180, 70), "→", () => ChangeBook(1));
            }
            BuildPlayback(); ShowSelection();
        }
        private void ChangeBook(int delta)
        {
            bookIndex = (bookIndex + delta + books.Length) % books.Length;
            saved = null; // Do not save another story's bookmark under the newly selected book key.
            foreach (Transform child in cardArea) { child.gameObject.SetActive(false); Destroy(child.gameObject); }
            BuildCard();
        }
        private void BuildCard()
        {
            var book = books[bookIndex]; thumbnail = book.cover;
            var shadow = Paper("Card shadow", cardArea, new Vector2(970, 451), new Vector2(650, 730), new Color(.23f, .33f, .37f, .15f));
            var card = Paper("Campus story card", cardArea, new Vector2(960, 462), new Vector2(650, 730), new Color(.99f, .98f, .95f));
            // One real story is available; do not invent other routes or unlockable chapters.
            var hit = Fill("Enter campus story", card, new Vector2(325, 365), new Vector2(650, 730), new Color(1, 1, 1, 0));
            var entry = hit.gameObject.AddComponent<Button>(); entry.targetGraphic = hit; entry.onClick.AddListener(() => ShowChapters());
            var title = Text(card, new Vector2(325, 666), new Vector2(560, 65), book.title, 33, true);
            var picture = Rect("Story picture", card, new Vector2(325, 427), new Vector2(570, 321));
            var raw = picture.gameObject.AddComponent<RawImage>(); raw.texture = thumbnail; raw.uvRect = new Rect(0, 0, 1, 1); raw.raycastTarget = false;
            Text(card, new Vector2(325, 169), new Vector2(570, 60), book.subtitle, 29, true, blue);
            Text(card, new Vector2(325, 123), new Vector2(570, 100), book.description, 22, true);
            var enter = Text(card, new Vector2(325, 47), new Vector2(400, 45), MenuCopy.Get("enterStory"), 26, true, blue);
            var hoverLine = Rect("Card underline", card, new Vector2(220, 23), new Vector2(210, 4)); hoverLine.pivot = new Vector2(0, .5f);
            var g = hoverLine.gameObject.AddComponent<MenuInkGraphic>(); g.kind = MenuInkGraphic.Shape.Stroke; g.color = blue; g.raycastTarget = false;
            var hover = hit.gameObject.AddComponent<MenuHoverInk>(); hover.stroke = hoverLine; hover.label = enter; hover.resting = blue; hover.active = ink;
        }
        private void BuildPlayback()
        {
            playback = Rect("Linear story playback", stage, new Vector2(960, 540), new Vector2(1920, 1080));
            var background = Rect("Cinematic background", playback, new Vector2(960, 540), new Vector2(1920, 1080));
            art = background.gameObject.AddComponent<RawImage>(); art.raycastTarget = false;
            var overlay = Rect("Scene dissolve", playback, new Vector2(960, 540), new Vector2(1920, 1080));
            dissolve = overlay.gameObject.AddComponent<RawImage>(); dissolve.color = Color.clear; dissolve.raycastTarget = false;
            // The catalog names the whole story; keep the rear doorway unobstructed during playback.
            var back = Paper("Back paper", playback, new Vector2(1702, 1024), new Vector2(300, 60), new Color(.98f, .97f, .93f, .94f));
            Button(back, new Vector2(150, 30), new Vector2(290, 58), "章节目录", () => ShowChapters(), 23);
            var dialogue = Paper("Dialogue paper", playback, new Vector2(960, 158), new Vector2(1720, 258), new Color(.98f, .97f, .93f, .55f));
            dialoguePanel = dialogue;
            var hit = Fill("Read next", dialogue, new Vector2(860, 129), new Vector2(1720, 258), Color.clear);
            hit.gameObject.AddComponent<Button>().onClick.AddListener(Next);
            speaker = Text(dialogue, new Vector2(440, 209), new Vector2(700, 42), "", 28, false, blue);
            body = Text(dialogue, new Vector2(860, 127), new Vector2(1540, 110), "", 33);
            Text(dialogue, new Vector2(430, 36), new Vector2(680, 35), "点击 / 空格：补全文字或继续", 18, false, blue);
            var next = Button(dialogue, new Vector2(1540, 37), new Vector2(250, 60), "继续 →", Next, 25); nextLabel = next.GetComponentInChildren<TMP_Text>();
            BuildMathInput();
            ending = Rect("Act finished", playback, new Vector2(960, 540), new Vector2(1920, 1080));
            Fill("End veil", ending, new Vector2(960, 540), new Vector2(1920, 1080), new Color(.97f, .96f, .92f, .92f));
            Text(ending, new Vector2(960, 680), new Vector2(1200, 110), "当前内容已读完", 50, true);
            endingSummary = Text(ending, new Vector2(960, 575), new Vector2(1200, 95), "", 29, true, blue);
            nextPreview = Text(ending, new Vector2(960, 502), new Vector2(1200, 55), "", 24, true, blue);
            Button(ending, new Vector2(700, 340), new Vector2(300, 80), "从头阅读", Begin);
            Button(ending, new Vector2(1130, 340), new Vector2(360, 80), MenuCopy.Get("storyList"), ShowSelection);
            pause = Rect("Pause", playback, new Vector2(960, 540), new Vector2(1920, 1080));
            Fill("Pause veil", pause, new Vector2(960, 540), new Vector2(1920, 1080), new Color(.12f, .18f, .21f, .85f));
            Text(pause, new Vector2(960, 660), new Vector2(900, 90), "已暂停", 48, true, Color.white);
            var resume = Button(pause, new Vector2(960, 490), new Vector2(380, 80), "继续阅读", TogglePause);
            var list = Button(pause, new Vector2(960, 360), new Vector2(380, 80), "章节目录", () => ShowChapters());
            foreach (var button in new[] { resume, list })
            { var tint = button.GetComponent<MenuHoverInk>(); tint.resting = Color.white; tint.active = new Color(.72f, .85f, .88f); }
        }
        private void ShowSelection()
        {
            StopAllCoroutines(); reading = paused = transitioning = false;
            if (chapterMenu != null) chapterMenu.gameObject.SetActive(false);
            dissolve.color = Color.clear;
            playback.gameObject.SetActive(false); selection.gameObject.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(selection.GetComponentInChildren<Button>().gameObject);
        }
        private void Begin()
        { BeginChapter(0); }
        private void BeginChapter(int chapter, int line = 0, bool flowing = false)
        {
            if (saved == null) LoadReading();
            if (!saved.Enter(chapter, line)) return;
            chapterIndex = chapter;
            StopAllCoroutines();
            shots = new Dictionary<StoryShot, Texture2D>();
            foreach (var picture in books[bookIndex].pictures) shots.Add(picture.shot, picture.texture);
            beats = books[bookIndex].chapters[chapter].beats;
            endingSummary.text = string.IsNullOrWhiteSpace(books[bookIndex].endingSummary) ? "当前已制作内容已读完。" : books[bookIndex].endingSummary;
            nextPreview.text = string.IsNullOrWhiteSpace(books[bookIndex].nextPreview) ? "后续内容制作中" : books[bookIndex].nextPreview;
            progress = new StoryProgress(beats.Length); paused = transitioning = false; reading = true; lastAdvanceFrame = Time.frameCount;
            for (int i = 0; i < saved.Beat; i++) progress.Advance();
            if (chapterMenu != null) chapterMenu.gameObject.SetActive(false);
            selection.gameObject.SetActive(false); playback.gameObject.SetActive(true); ending.gameObject.SetActive(false); pause.gameObject.SetActive(false);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            ShowBeat(!flowing);
            SaveReading();
        }
        private void LoadReading()
        {
            StoryReadingSave data = null;
            try { if (PlayerPrefs.HasKey(SaveKey)) data = JsonUtility.FromJson<StoryReadingSave>(PlayerPrefs.GetString(SaveKey)); }
            catch (Exception e) { Debug.LogWarning("Reading save could not be loaded: " + e.Message); }
            var chapters = books[bookIndex].chapters;
            if (data != null)
            {
                foreach (var chapter in chapters)
                    if (chapter.storyId == data.chapterId)
                    {
                        if (!string.IsNullOrEmpty(data.beatId))
                        {
                            int found = Array.FindIndex(chapter.beats, b => b.id == data.beatId);
                            if (found >= 0) data.beat = found;
                        }
                        else if (data.contentVersion < 2 && data.chapterId == "campus-act-05" && data.beat >= 10)
                            data.beat = 10; // Old fifth-act bookmarks return to the revised reading/answer segment.
                    }
            }
            var ids = new string[chapters.Length]; var counts = new int[chapters.Length];
            for (int i = 0; i < chapters.Length; i++) { ids[i] = chapters[i].storyId; counts[i] = chapters[i].beats.Length; }
            saved = new StoryReadingState(ids, counts, data);
        }
        private void SaveReading()
        {
            if (saved == null) return;
            try
            {
                var data = saved.Capture(); data.contentVersion = 2;
                data.beatId = books[bookIndex].chapters[saved.Chapter].beats[saved.Beat].id;
                PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data)); PlayerPrefs.Save(); saveFailed = false;
            }
            catch (Exception e) { saveFailed = true; Debug.LogWarning("Reading save failed: " + e.Message); }
            if (saveNotice != null) saveNotice.text = saveFailed ? "进度保存失败，请稍后重试" : "阅读进度已自动保存到本机";
        }
        private void ShowChapters(string message = "")
        {
            StopAllCoroutines(); reading = paused = transitioning = false;
            playback.gameObject.SetActive(false); selection.gameObject.SetActive(false);
            LoadReading();
            if (chapterMenu != null) { chapterMenu.gameObject.SetActive(false); Destroy(chapterMenu.gameObject); }
            chapterMenu = Rect("Chapter flow", stage, new Vector2(960, 540), new Vector2(1920, 1080));
            Text(chapterMenu, new Vector2(960, 960), new Vector2(1100, 80), books[bookIndex].title, 46, true, blue);
            Text(chapterMenu, new Vector2(960, 877), new Vector2(1400, 70), string.IsNullOrEmpty(message) ? "章节目录 · 沿着那段时光，继续往前" : message, 28, true);
            Button(chapterMenu, new Vector2(1700, 970), new Vector2(290, 65), "返回故事", ShowSelection, 23);
            var sheet = Paper("Chapter paper", chapterMenu, new Vector2(960, 555), new Vector2(1820, 490), new Color(.91f, .91f, .88f));
            var view = Rect("Horizontal chapters", sheet, new Vector2(910, 245), new Vector2(1740, 390));
            view.gameObject.AddComponent<Image>().color = Color.clear; view.gameObject.AddComponent<RectMask2D>();
            chapterScroll = view.gameObject.AddComponent<ScrollRect>(); chapterScroll.viewport = view;
            chapterScroll.horizontal = true; chapterScroll.vertical = false; chapterScroll.movementType = ScrollRect.MovementType.Clamped;
            chapterScroll.scrollSensitivity = 35;
            int count = books[bookIndex].chapters.Length;
            var content = Rect("Chapter nodes", view, Vector2.zero, new Vector2(Mathf.Max(1740, count * 340), 390));
            content.anchorMin = content.anchorMax = new Vector2(0, .5f); content.pivot = new Vector2(0, .5f);
            chapterScroll.content = content;
            for (int i = 0; i < count; i++)
            {
                int index = i; bool unlocked = saved.IsUnlocked(i), done = saved.IsComplete(i);
                if (i + 1 < count)
                    Text(content, new Vector2(340 + i * 340, 195), new Vector2(36, 50), "→", 28, true, blue);
                var node = Paper("Chapter " + (i + 1), content, new Vector2(170 + i * 340, 195), new Vector2(300, 320), unlocked ? new Color(.99f,.98f,.95f) : new Color(.85f,.85f,.82f));
                Text(node, new Vector2(150, 287), new Vector2(270, 40), (i + 1).ToString("00"), 26, true, blue);
                if (unlocked)
                {
                    Texture2D picture = null;
                    var shot = books[bookIndex].chapters[i].beats[0].shot;
                    foreach (var p in books[bookIndex].pictures) if (p.shot == shot) { picture = p.texture; break; }
                    var frame = Rect("Chapter thumbnail", node, new Vector2(150, 190), new Vector2(264, 149));
                    var raw = frame.gameObject.AddComponent<RawImage>(); raw.texture = picture; raw.raycastTarget = false;
                }
                else Text(node, new Vector2(150, 190), new Vector2(270, 140), "尚未解锁\n完成前一幕后开启", 22, true, blue);
                Text(node, new Vector2(150, 87), new Vector2(280, 60), unlocked ? books[bookIndex].chapters[i].chapterTitle : "第" + (i+1) + "幕", 23, true);
                int line = i == saved.Chapter && !done ? saved.Beat : 0;
                string label = done ? "已完成 · 重看" : unlocked ? (line > 0 ? "继续本幕" : "进入剧情") : "未解锁";
                if (unlocked) Button(node, new Vector2(150, 33), new Vector2(265, 52), label, () => BeginChapter(index, line), 21);
                else Text(node, new Vector2(150,33), new Vector2(265,52),label,21,true,blue);
            }
            Canvas.ForceUpdateCanvases();
            chapterScroll.horizontalNormalizedPosition = count <= 5 ? 0 : Mathf.Clamp01(saved.Chapter / (float)(count - 1));
            if (count > 5)
            {
                Button(chapterMenu, new Vector2(100, 270), new Vector2(130, 65), "←", () => MoveChapterFlow(-1));
                Button(chapterMenu, new Vector2(1820, 270), new Vector2(130, 65), "→", () => MoveChapterFlow(1));
            }
            int resumeChapter = saved.Chapter, resumeBeat = saved.AllComplete ? 0 : saved.Beat;
            Button(chapterMenu, new Vector2(960, 220), new Vector2(610, 85), saved.AllComplete ? "重温最后阅读的章节 →" : "继续阅读 → " + books[bookIndex].chapters[resumeChapter].chapterTitle,
                () => BeginChapter(resumeChapter, resumeBeat), 29);
            saveNotice = Text(chapterMenu, new Vector2(960, 135), new Vector2(1300, 45), saveFailed ? "进度保存失败，请稍后重试" : "阅读位置自动保存到本机 · 已完成章节可随时重看", 21, true, blue);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
        }
        private void MoveChapterFlow(int direction)
        { chapterScroll.horizontalNormalizedPosition = Mathf.Clamp01(chapterScroll.horizontalNormalizedPosition + direction * .4f); }
        private static Rect Cover(Texture texture, float focus, float targetAspect = 16f / 9)
        {
            float aspect = texture.width / (float)texture.height;
            float width = Mathf.Min(1, targetAspect / aspect), height = Mathf.Min(1, aspect / targetAspect);
            return new Rect((1 - width) * Mathf.Clamp01(focus), (1 - height) * .6f, width, height);
        }
        private static bool IsWaterCup(StoryShot shot)
        { return shot >= StoryShot.EveningStudy && shot <= StoryShot.WetSleeve; }
        private static Rect PictureView(RawImage image, StoryBeat beat)
        {
            // Author requests a full 16:9 frame, stretching the third-act images without cropping.
            image.rectTransform.sizeDelta = new Vector2(1920, 1080);
            image.rectTransform.anchoredPosition = new Vector2(960, 540);
            return IsWaterCup(beat.shot) ? new Rect(0, 0, 1, 1) : Cover(image.texture, beat.focus);
        }
        private void ShowBeat(bool first = false)
        {
            var beat = beats[progress.Index];
            if (first)
            {
                currentShot = beat.shot; art.texture = shots[beat.shot];
                art.uvRect = cameraTarget = PictureView(art, beat); art.color = Color.white; dissolve.color = Color.clear; Present(beat);
            }
            else if (beat.fade || beat.shot != currentShot) StartCoroutine(ChangeShot(beat));
            else { cameraTarget = PictureView(art, beat); Present(beat); }
        }
        private IEnumerator ChangeShot(StoryBeat beat)
        {
            transitioning = true; body.text = ""; speaker.text = "";
            dissolve.texture = shots[beat.shot]; dissolve.uvRect = PictureView(dissolve, beat);
            float duration = beat.fade ? .65f : .4f;
            for (float elapsed = 0; elapsed < duration;)
            {
                if (!paused)
                {
                    elapsed += Time.unscaledDeltaTime;
                    float alpha = Mathf.SmoothStep(0, 1, elapsed / duration);
                    dissolve.color = new Color(1, 1, 1, alpha);
                }
                yield return null;
            }
            currentShot = beat.shot; art.texture = dissolve.texture; art.uvRect = cameraTarget = dissolve.uvRect;
            PictureView(art, beat); art.color = Color.white;
            dissolve.color = Color.clear; transitioning = false; Present(beat);
        }
        private void Present(StoryBeat beat)
        {
            mathPanel.gameObject.SetActive(beat.requiresMathAnswer);
            dialoguePanel.gameObject.SetActive(!beat.requiresMathAnswer);
            if (beat.requiresMathAnswer)
            {
                mathFeedback.text = ""; mathInput.text = "";
                if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(mathInput.gameObject);
                mathInput.ActivateInputField();
            }
            speaker.text = string.IsNullOrEmpty(beat.speaker) ? "" : beat.speaker;
            body.text = beat.text; typing = 0; body.maxVisibleCharacters = 0;
            nextLabel.text = progress.Index == progress.Count - 1
                ? (chapterIndex + 1 < books[bookIndex].chapters.Length ? "下一幕 →" : "本幕结束 →") : "继续 →";
        }
        private void Next()
        {
            if (!reading || paused || transitioning || progress.Complete || mathPanel.gameObject.activeSelf) return;
            if (lastAdvanceFrame == Time.frameCount) return;
            lastAdvanceFrame = Time.frameCount;
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            if (body.maxVisibleCharacters < body.text.Length) { typing = body.text.Length; body.maxVisibleCharacters = int.MaxValue; return; }
            AdvanceBeat();
        }
        private void AdvanceBeat()
        {
            if (progress.Advance()) { saved.Bookmark(progress.Index); SaveReading(); ShowBeat(); }
            else
            {
                saved.Finish(); SaveReading();
                int nextChapter = chapterIndex + 1;
                if (nextChapter < books[bookIndex].chapters.Length) BeginChapter(nextChapter, 0, true);
                else ShowChapters("当前已制作章节已读完，后续剧情制作中");
            }
        }
        private void TogglePause() { if (!reading) return; paused = !paused; pause.gameObject.SetActive(paused); lastAdvanceFrame = Time.frameCount; }
        private void Update()
        {
            if (reading && !paused && !transitioning)
            {
                typing += Time.unscaledDeltaTime * 34;
                body.maxVisibleCharacters = Mathf.Min(body.text.Length, Mathf.FloorToInt(typing));
                var r = art.uvRect; float t = 1 - Mathf.Exp(-Time.unscaledDeltaTime * 2.4f);
                art.uvRect = new Rect(Mathf.Lerp(r.x, cameraTarget.x, t), Mathf.Lerp(r.y, cameraTarget.y, t), Mathf.Lerp(r.width, cameraTarget.width, t), Mathf.Lerp(r.height, cameraTarget.height, t));
            }
            if (Keyboard.current == null) return;
            if (Keyboard.current.escapeKey.wasPressedThisFrame) { if (selection.gameObject.activeSelf) Close(); else if (chapterMenu != null && chapterMenu.gameObject.activeSelf) ShowSelection(); else TogglePause(); return; }
            if (mathPanel.gameObject.activeInHierarchy) return;
            if (reading && (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)) Next();
        }
        private void Close() { closed?.Invoke(); Destroy(gameObject); }
        private void BuildMathInput()
        {
            mathPanel = Paper("Math answer paper", playback, new Vector2(960, 190), new Vector2(1720, 330), new Color(.98f, .97f, .93f, .88f));
            Text(mathPanel, new Vector2(650, 281), new Vector2(1160, 42), "凌赫在草稿上写了几行，很快算出了结果。", 27, false, blue);
            Text(mathPanel, new Vector2(620, 185), new Vector2(1100, 128),
                "f(x)=x²-12x-2，x∈[2,14]，M为区间最大值。\nm、n为正实数，m+n=26，mn=91。求 M+m²+n²。", 26);
            Text(mathPanel, new Vector2(1370, 232), new Vector2(480, 48), "写下整道题的最终结果", 25, true, blue);
            var field = Fill("Final answer", mathPanel, new Vector2(1300, 155), new Vector2(300, 66), new Color(.91f, .94f, .93f));
            mathInput = field.gameObject.AddComponent<TMP_InputField>();
            var viewport = Rect("Answer viewport", field.transform, Vector2.zero, Vector2.zero);
            viewport.anchorMin = Vector2.zero; viewport.anchorMax = Vector2.one;
            viewport.offsetMin = new Vector2(12, 5); viewport.offsetMax = new Vector2(-12, -5);
            viewport.gameObject.AddComponent<RectMask2D>();
            var inputText = Text(viewport, Vector2.zero, Vector2.zero, "", 32);
            inputText.rectTransform.anchorMin = Vector2.zero; inputText.rectTransform.anchorMax = Vector2.one;
            inputText.rectTransform.offsetMin = inputText.rectTransform.offsetMax = Vector2.zero;
            var placeholder = Text(viewport, Vector2.zero, Vector2.zero, "输入答案", 27, false, blue);
            placeholder.rectTransform.anchorMin = Vector2.zero; placeholder.rectTransform.anchorMax = Vector2.one;
            placeholder.rectTransform.offsetMin = placeholder.rectTransform.offsetMax = Vector2.zero;
            mathInput.textViewport = viewport; mathInput.textComponent = inputText; mathInput.placeholder = placeholder;
            mathInput.fontAsset = inputText.font; mathInput.pointSize = 32;
            mathInput.lineType = TMP_InputField.LineType.SingleLine; mathInput.characterLimit = 16;
            mathInput.onSubmit.AddListener(_ => SubmitMath());
            Button(mathPanel, new Vector2(1560, 155), new Vector2(180, 66), "确认", SubmitMath, 27);
            mathFeedback = Text(mathPanel, new Vector2(860, 57), new Vector2(1580, 70), "", 25, false, blue);
            mathPanel.gameObject.SetActive(false);
        }
        private void SubmitMath()
        {
            if (!reading || paused || transitioning || !mathPanel.gameObject.activeInHierarchy || !beats[progress.Index].requiresMathAnswer) return;
            if (!StoryMathAnswer.IsCorrect(mathInput.text))
            {
                mathFeedback.text = string.IsNullOrWhiteSpace(mathInput.text) ? "李晚云：你还没写呢。再看看？" : "李晚云：是这个答案吗？好像不太对，你再算算？";
                mathInput.ActivateInputField(); return;
            }
            mathPanel.gameObject.SetActive(false); dialoguePanel.gameObject.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            lastAdvanceFrame = Time.frameCount;
            AdvanceBeat();
        }
        private void OnApplicationPause(bool value) { if (value) SaveReading(); }
        private void OnDestroy()
        {
            if (font == null) return;
            foreach (var texture in font.atlasTextures) if (texture != null) Destroy(texture); Destroy(font);
        }
    }
}

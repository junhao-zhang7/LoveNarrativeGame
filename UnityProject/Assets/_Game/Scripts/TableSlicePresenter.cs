using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed class TableSlicePresenter : MonoBehaviour
    {
        [SerializeField] private TMP_FontAsset font;
        public void SetFont(TMP_FontAsset value) { font = value; }
        private TableSliceState state;
        private RectTransform root, page, pencilCase, hand;
        private TMP_Text dialogue, instruction, pageText, annotation, caseText;
        private TMP_InputField answer;
        private Button submit, hint, solution, upper, lower, share, next;
        private bool paused;
        private Image pauseShade;
        private readonly Color ink = new Color(.16f, .23f, .25f);
        private readonly Vector2 home = new Vector2(440, 425);
        private readonly Vector2 shared = new Vector2(820, 425);

        private void Start()
        {
            if (font == null) { Debug.LogError("TableSlice requires its Chinese font."); enabled = false; return; }
            var system = FindAnyObjectByType<EventSystem>();
            if (system == null) new GameObject("Slice EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            BuildUI(); ResetSlice();
            if (Array.IndexOf(Environment.GetCommandLineArgs(), "-tableSliceSmoke") >= 0) StartCoroutine(Smoke());
        }

        private RectTransform Box(string name, Transform parent, Vector2 pos, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero; r.pivot = Vector2.one * .5f;
            r.anchoredPosition = pos; r.sizeDelta = size; go.GetComponent<Image>().color = color; return r;
        }
        private TMP_Text Label(string name, Transform parent, Vector2 pos, Vector2 size, string value, int points = 23)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)); go.transform.SetParent(parent, false);
            var r = (RectTransform)go.transform; r.anchorMin = r.anchorMax = Vector2.zero; r.anchoredPosition = pos; r.sizeDelta = size;
            var t = go.GetComponent<TMP_Text>(); t.font = font; t.fontSize = points; t.color = ink; t.text = value;
            t.alignment = TextAlignmentOptions.MidlineLeft; t.raycastTarget = false; return t;
        }
        private Button Button(string name, Vector2 pos, Vector2 size, string value, UnityEngine.Events.UnityAction action)
        {
            var r = Box(name, root, pos, size, new Color(.78f, .87f, .84f));
            var b = r.gameObject.AddComponent<Button>(); b.targetGraphic = r.GetComponent<Image>(); b.onClick.AddListener(action);
            var text = Label("Label", r, size * .5f, size - new Vector2(12, 4), value, 21); text.alignment = TextAlignmentOptions.Center;
            return b;
        }
        private void BuildUI()
        {
            var canvas = new GameObject("Table Slice Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvas.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 800); scaler.matchWidthOrHeight = 1f;
            root = Box("Desk", canvas.transform, Vector2.zero, Vector2.zero, new Color(.88f, .87f, .82f));
            root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one; root.offsetMin = root.offsetMax = Vector2.zero;
            Label("Title", root, new Vector2(470, 755), new Vector2(850, 45), "同一页 · 数学讲题与英语互助", 30);
            Label("Prototype", root, new Vector2(410, 714), new Vector2(750, 35), "交互灰盒｜几何手部与桌面为占位画面｜无正式音效", 18);
            Box("My desk", root, new Vector2(450, 450), new Vector2(800, 410), new Color(.70f, .61f, .49f));
            Box("Her desk", root, new Vector2(1060, 450), new Vector2(300, 410), new Color(.65f, .57f, .47f));
            Label("My name", root, new Vector2(210, 630), new Vector2(250, 35), "凌赫 · 你的桌面");
            Label("Her name", root, new Vector2(1060, 630), new Vector2(260, 35), "李晚云 · 邻座");
            var zone = Box("Shared space", root, shared, new Vector2(260, 280), new Color(.59f, .76f, .68f, .7f));
            Label("Zone label", zone, new Vector2(130, 244), new Vector2(235, 45), "共同区域 · 递到这里", 19);
            page = Box("Page", root, home, new Vector2(480, 260), new Color(.98f, .96f, .87f));
            pageText = Label("Problem", page, new Vector2(240, 170), new Vector2(435, 150), "", 24);
            annotation = Label("Her handwriting", page, new Vector2(240, 54), new Vector2(435, 70), "", 23);
            annotation.color = new Color(.25f, .40f, .55f);
            var drag = page.gameObject.AddComponent<TableSliceDrag>(); drag.Owner = this;
            pencilCase = Box("Pencil case", root, shared, new Vector2(190, 65), new Color(.42f, .49f, .51f));
            caseText = Label("Case label", pencilCase, new Vector2(95, 32), new Vector2(180, 60), "笔盒 · 先腾位置", 20); caseText.color = Color.white;
            var cd = pencilCase.gameObject.AddComponent<TableSliceDrag>(); cd.Owner = this; cd.IsCase = true;
            hand = Box("Her hand placeholder", root, new Vector2(1150, 450), new Vector2(100, 48), new Color(.89f, .71f, .58f));
            Label("Hand label", hand, new Vector2(50, 24), new Vector2(100, 44), "晚云的手", 16);
            dialogue = Label("Dialogue", root, new Vector2(640, 175), new Vector2(1160, 85), "", 25);
            instruction = Label("Instruction", root, new Vector2(640, 242), new Vector2(1160, 44), "", 20);
            var fieldRect = Box("Answer", root, new Vector2(135, 80), new Vector2(160, 55), Color.white);
            answer = fieldRect.gameObject.AddComponent<TMP_InputField>();
            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D)); viewport.transform.SetParent(fieldRect, false);
            var vr = (RectTransform)viewport.transform; vr.anchorMin = Vector2.zero; vr.anchorMax = Vector2.one; vr.offsetMin = new Vector2(8, 4); vr.offsetMax = new Vector2(-8, -4);
            var inputText = Label("Input text", vr, Vector2.zero, Vector2.zero, "", 25);
            var tr = (RectTransform)inputText.transform; tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one; tr.offsetMin = tr.offsetMax = Vector2.zero;
            answer.textViewport = vr; answer.textComponent = (TextMeshProUGUI)inputText; answer.lineType = TMP_InputField.LineType.SingleLine;
            answer.characterLimit = 24; answer.onSubmit.AddListener(_ => Submit());
            submit = Button("Submit", new Vector2(280, 80), new Vector2(115, 55), "提交", Submit);
            hint = Button("Hint", new Vector2(410, 80), new Vector2(115, 55), "提示", Hint);
            solution = Button("Solution", new Vector2(560, 80), new Vector2(160, 55), "查看解法", Solve);
            upper = Button("Upper", new Vector2(180, 80), new Vector2(235, 55), "笔盒放左上", () => Clear(1));
            lower = Button("Lower", new Vector2(440, 80), new Vector2(235, 55), "笔盒放左下", () => Clear(2));
            share = Button("Share", new Vector2(740, 80), new Vector2(235, 55), "递给她（替代拖放）", Share);
            next = Button("Continue", new Vector2(1040, 80), new Vector2(260, 55), "继续", Continue);
            Button("Restart", new Vector2(1070, 750), new Vector2(110, 42), "重玩", ResetSlice);
            Button("Pause", new Vector2(1190, 750), new Vector2(100, 42), "暂停", TogglePause);
            pauseShade = Box("Pause overlay", root, new Vector2(640, 400), new Vector2(1280, 800), new Color(.12f, .17f, .18f, .96f)).GetComponent<Image>();
            var pauseText = Label("Pause text", pauseShade.transform, new Vector2(640, 430), new Vector2(900, 180), "已暂停\n点击此处或按 Esc 继续；阅读速度不影响剧情。", 30); pauseText.color = Color.white; pauseText.alignment = TextAlignmentOptions.Center;
            pauseShade.gameObject.AddComponent<Button>().onClick.AddListener(TogglePause); pauseShade.gameObject.SetActive(false);
        }
        private void Update() { if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) TogglePause(); }
        private void TogglePause() { paused = !paused; pauseShade.gameObject.SetActive(paused); }
        private void ResetSlice()
        {
            StopAllCoroutines(); state = new TableSliceState(); paused = false; pauseShade.gameObject.SetActive(false);
            answer.text = ""; annotation.text = ""; page.anchoredPosition = home; page.localRotation = Quaternion.identity;
            pencilCase.anchoredPosition = shared; hand.anchoredPosition = new Vector2(1150, 450);
            pageText.text = "f(x) = x² − 12x − 2，x ∈ [2,14]\nM 为区间最大值。m+n=26，mn=91\n求 M + m² + n²";
            dialogue.text = "李晚云：这道题，你能帮我看一下吗？\n凌赫：你们先去，我这儿还有道题。";
            instruction.text = "输入结果；也可以看提示或解法。没有时间限制。"; Refresh();
        }
        private void Refresh()
        {
            bool q = state.Current == TableSliceState.Step.Question;
            answer.gameObject.SetActive(q); submit.gameObject.SetActive(q); hint.gameObject.SetActive(q); solution.gameObject.SetActive(q);
            bool clear = state.Current == TableSliceState.Step.ClearDesk || state.Current == TableSliceState.Step.ReadyToShare;
            upper.gameObject.SetActive(clear); lower.gameObject.SetActive(clear);
            bool ready = state.Current == TableSliceState.Step.ReadyToShare || state.Current == TableSliceState.Step.EnglishReady;
            share.gameObject.SetActive(ready);
            next.gameObject.SetActive(state.Current == TableSliceState.Step.MathReturned || state.Current == TableSliceState.Step.Complete);
            next.GetComponentInChildren<TMP_Text>().text = state.Current == TableSliceState.Step.Complete ? "再体验一次" : "次日 · 英语互助";
            caseText.text = clear ? "笔盒 · 可拖放" : "笔盒";
        }
        private void Submit()
        {
            if (paused || state.Current != TableSliceState.Step.Question) return;
            if (!state.TryAnswer(answer.text)) { instruction.text = "还没算对也没关系。先比较两个端点，再用和的平方；可查看解法。"; return; }
            QuestionFinished();
        }
        private void Solve() { if (!paused && state.FinishQuestion()) QuestionFinished(); }
        private void Hint()
        {
            if (paused) return;
            instruction.text = state.NextHint() == 1 ? "提示：开口向上，比较 f(2) 和 f(14)。" : "提示：M=26；m²+n²=(m+n)²−2mn=494。";
        }
        private void QuestionFinished()
        {
            pageText.text = "M = max(f(2), f(14)) = 26\nm²+n² = 26² − 2×91 = 494\n26 + 494 = 520";
            dialogue.text = "凌赫：不用分别解 m 和 n。和的平方，减两倍的积。\n李晚云把本子转过来，想看清你写的这一步。";
            instruction.text = "先腾出共同区域：把笔盒拖到左上或左下，或用下面按钮。"; Refresh();
        }
        private void Clear(int layout)
        {
            if (paused || !state.Clear(layout)) return;
            pencilCase.anchoredPosition = new Vector2(180, layout == 1 ? 580 : 290);
            instruction.text = "把纸页拖到右侧绿色区域，或点击“递给她”。两种摆法都可以。"; Refresh();
        }
        public bool CanDrag(bool isCase)
        {
            return !paused && (isCase ? state.Current == TableSliceState.Step.ClearDesk || state.Current == TableSliceState.Step.ReadyToShare :
                state.Current == TableSliceState.Step.ReadyToShare || state.Current == TableSliceState.Step.EnglishReady);
        }
        public void Drop(bool isCase, Vector2 position)
        {
            if (!CanDrag(isCase)) return;
            if (isCase)
            {
                if (position.x < 300 && (position.y > 535 || position.y < 330)) Clear(position.y > 430 ? 1 : 2);
                else instruction.text = "笔盒可以放在左上或左下的空位。也可点击下面的替代按钮。";
            }
            else if (Vector2.Distance(position, shared) < 210) Share();
            else instruction.text = "把纸页中心放进右侧绿色区域；放错可以再试，没有惩罚。";
        }
        private void Share()
        {
            if (paused || !state.Share()) return;
            Refresh(); instruction.text = "她正在接过纸页……可以暂停，或等动作结束。"; StartCoroutine(Exchange());
        }
        private IEnumerator Move(RectTransform item, Vector2 target, float seconds)
        {
            Vector2 from = item.anchoredPosition; float elapsed = 0;
            while (elapsed < seconds)
            {
                yield return null; if (paused || !Application.isFocused && !Application.isBatchMode) continue;
                elapsed += Time.unscaledDeltaTime; item.anchoredPosition = Vector2.Lerp(from, target, Mathf.SmoothStep(0, 1, elapsed / seconds));
            }
            item.anchoredPosition = target;
        }
        private IEnumerator Exchange()
        {
            bool english = state.Current == TableSliceState.Step.EnglishResponding;
            yield return Move(page, shared, .6f);
            yield return Move(hand, new Vector2(940, 430), .45f);
            page.localRotation = Quaternion.Euler(0, 0, -6);
            dialogue.text = english ? "李晚云：你只看这一句了。往前看，前面说了时间。" : "李晚云：后面不用分别解出来……原来是这样。";
            yield return Move(hand, new Vector2(800, 430), .6f);
            annotation.text = english ? "她的圈记：earlier → 先看前文的时间\n下次别只改选项。" : "她补上的一行：494 + 26 = 520\n“谢谢，我知道哪里错了。”";
            yield return Move(hand, new Vector2(940, 430), .4f);
            page.localRotation = Quaternion.identity;
            yield return Move(page, home + new Vector2(0, state.Layout == 1 ? -15 : 15), .7f);
            yield return Move(hand, new Vector2(1150, 450), .4f);
            state.ReturnPage();
            dialogue.text = english ? "凌赫：那我前面三道是不是也……\n李晚云：嗯。（她用本子遮住笑。）" : "凌赫：今天帮你带码，你打算怎么报答我？\n李晚云：明天中午请你吃饭，行吗？";
            instruction.text = english ? "小样结束：圈记留在本子上。可重玩，换一种桌面摆法。" : "她把纸页推回你留出的空位。接下来换她帮你。";
            Refresh();
        }
        private void Continue()
        {
            if (paused) return;
            if (state.Current == TableSliceState.Step.Complete) { ResetSlice(); return; }
            if (!state.StartEnglish()) return;
            annotation.text = ""; pageText.text = "次日 · 英语练习\n前文：He had left earlier that morning.\n为什么这里不能只看当前一句？";
            dialogue.text = "凌赫：我这回真有题。这里为什么不能选 C？\n李晚云放下自己的书，朝你的练习册看过来。";
            instruction.text = "把练习册递给她。这次由她留下提示。"; Refresh();
        }
        private IEnumerator Smoke()
        {
            yield return null;
            answer.text = "519"; Submit(); Require(state.Current == TableSliceState.Step.Question, "wrong answer blocks");
            Hint(); Hint(); answer.text = " ５２０ "; Submit(); Require(state.Current == TableSliceState.Step.ClearDesk, "fullwidth answer");
            Drop(true, new Vector2(180, 580)); Require(state.Layout == 1, "upper layout");
            Drop(false, shared); Share(); // duplicate action must not start a second exchange
            float deadline = Time.realtimeSinceStartup + 20;
            while (state.Current == TableSliceState.Step.Responding && Time.realtimeSinceStartup < deadline) yield return null;
            Require(state.Current == TableSliceState.Step.MathReturned, "math exchange returned");
            Continue(); Share();
            deadline = Time.realtimeSinceStartup + 20;
            while (state.Current == TableSliceState.Step.EnglishResponding && Time.realtimeSinceStartup < deadline) yield return null;
            Require(state.Current == TableSliceState.Step.Complete && annotation.text.Contains("earlier"), "english annotation");
            // ResetSlice stops coroutines, so run the second path synchronously through state tests in the builder.
            Debug.Log("TABLE_SLICE_PLAYER_SMOKE_PASS"); Application.Quit(0);
        }
        private static void Require(bool ok, string message)
        {
            if (ok) return; Debug.LogError("TABLE_SLICE_SMOKE_FAIL: " + message); Application.Quit(2); throw new InvalidOperationException(message);
        }
    }
}

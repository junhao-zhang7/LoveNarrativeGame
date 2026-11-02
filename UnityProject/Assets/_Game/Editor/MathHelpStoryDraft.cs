using System.Collections.Generic;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class MathHelpStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/MathHelpSequence.asset";
    private const string Notes = "2026-11-02：默认两人合作推导，数值输入为可选且可随时返回合作；保留29段正文与后续对白。";
    public static readonly string[] Pictures = { "Invitation", "StayAndExplain", "ProblemNotebook", "WritingAnswer", "LunchAgreement" };
    public static readonly StoryShot[] Shots = { StoryShot.Invitation, StoryShot.StayAndExplain, StoryShot.ProblemNotebook, StoryShot.WritingAnswer, StoryShot.LunchAgreement };
    private static StoryBeat[] CreateLegacy()
    {
        return new[] {
            B("", "第五幕 · 这道题\n同日晚自习，七班。", StoryShot.Invitation, true),
            B("", "李晚云在草稿上划去一行，又抄了一遍题目。\n下课铃响，她拿着本子站到凌赫旁边。", StoryShot.Invitation),
            B("李晚云", "这道题……你能帮我看一下吗？我算了好几次。", StoryShot.Invitation),
            B("朋友", "凌赫，出去走走？闷死了。", StoryShot.Invitation),
            B("", "凌赫看看门口，又看看递来的本子。", StoryShot.Invitation),
            B("李晚云", "没事，你去——", StoryShot.Invitation),
            B("凌赫", "你们先去，我这儿还有道题。", StoryShot.Invitation),
            B("", "朋友点点头。凌赫把本子放到两人中间。", StoryShot.StayAndExplain),
            B("凌赫", "你先算出什么了？", StoryShot.StayAndExplain),
            B("李晚云", "顶点是六。我就把六代进去了，可算出来跟答案差好多。", StoryShot.StayAndExplain),
            B("凌赫", "你找的是最小值。它问的是最大值，还给了区间。", StoryShot.StayAndExplain),
            B("题面", "已知 f(x) = x² - 12x - 2，x∈[2,14]。\nM 是函数在该区间内的最大值。", StoryShot.ProblemNotebook),
            B("题面", "正实数 m、n 满足 m+n=26，mn=91。\n求 M+m²+n²。", StoryShot.ProblemNotebook),
            B("凌赫", "这里是负二十二，这里二十六。开口向上，六在中间，所以最大的是二十六。", StoryShot.ProblemNotebook),
            B("解题笔记", "f(2)=-22，f(14)=26，M=26。\n顶点横坐标为6；开口向上，比较区间两端。", StoryShot.ProblemNotebook),
            B("李晚云", "后面不用把 m 和 n 分别解出来？", StoryShot.ProblemNotebook),
            B("凌赫", "不用。和的平方，减两倍的积。", StoryShot.ProblemNotebook),
            B("解题笔记", "m²+n²=(m+n)²-2mn\n=26²-2×91=494。", StoryShot.ProblemNotebook),
            B("李晚云", "四百九十四……再加二十六，五百二十。", StoryShot.WritingAnswer),
            B("凌赫", "对。", StoryShot.WritingAnswer),
            B("", "他像是也刚注意到结果，清了清嗓子。\n她低头把最后一行描清楚。", StoryShot.WritingAnswer),
            B("李晚云", "谢谢，我知道哪里错了。", StoryShot.WritingAnswer),
            B("凌赫", "对了，今天帮你带码，你打算怎么报答我？", StoryShot.WritingAnswer),
            B("李晚云", "明天中午请你吃饭，行吗？", StoryShot.LunchAgreement),
            B("凌赫", "行。食堂？", StoryShot.LunchAgreement),
            B("李晚云", "不然呢？", StoryShot.LunchAgreement),
            B("", "两人第一次很自然地笑起来。", StoryShot.LunchAgreement)
        };
    }
    private static StoryBeat[] CreateMandatory()
    {
        var old = CreateLegacy(); var beats = new List<StoryBeat>();
        beats.AddRange(old.Take(10));
        beats.Add(B("凌赫（心声）", "f(x) = x² - 12x - 2，x∈[2,14]……", StoryShot.ProblemNotebook));
        beats.Add(B("凌赫（心声）", "M 是区间内的最大值……最大值。\n她把顶点代进去了，算成了最小值。", StoryShot.ProblemNotebook));
        beats.Add(B("凌赫（心声）", "m+n=26，mn=91，求 M+m²+n²。\n先看两端，再算后面这一项。", StoryShot.ProblemNotebook));
        var gate = B("", "凌赫在草稿上写了几行，很快算出了结果。", StoryShot.ProblemNotebook);
        gate.requiresMathAnswer = true; beats.Add(gate);
        var explanation = old[10]; explanation.shot = StoryShot.ProblemNotebook; beats.Add(explanation);
        beats.AddRange(old.Skip(13));
        for (int i = 0; i < beats.Count; i++) beats[i].id = "math-v2-" + i.ToString("D2");
        return beats.ToArray();
    }
    public static StoryBeat[] Create()
    {
        var beats=CreateMandatory();beats[13].requiresMathAnswer=false;
        beats[13].interaction=StoryInteraction.CooperateMath;
        beats[13].text="凌赫把练习本转到两人中间。先一起看看，也可以自己试着算。";
        return beats;
    }
    private static bool MatchesMandatory(StoryBeat[] actual)
    {var old=CreateMandatory();return actual!=null&&actual.Length==old.Length&&actual.Zip(old,(a,b)=>a!=null&&a.text==b.text&&a.speaker==b.speaker&&a.shot==b.shot&&a.fade==b.fade&&a.focus==b.focus&&a.requiresMathAnswer==b.requiresMathAnswer&&a.interaction==StoryInteraction.None).All(v=>v);}
    private static bool MatchesLegacy(StoryBeat[] actual)
    {
        var old = CreateLegacy();
        return actual != null && actual.Length == old.Length && actual.Zip(old, (a,b) =>
            a != null && a.text == b.text && a.speaker == b.speaker && a.shot == b.shot && a.fade == b.fade && a.focus == b.focus && !a.requiresMathAnswer).All(v => v);
    }
    private static StoryBeat B(string speaker, string text, StoryShot shot, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }

    public static void Install(StoryBook book)
    {
        var textures = Pictures.Select(p => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/MathHelp/" + p + ".png")).ToArray();
        if (book == null || textures.Any(t => t == null)) return;
        var chapter = AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if (chapter == null)
        {
            chapter = ScriptableObject.CreateInstance<StorySequence>();
            chapter.storyId = "campus-act-05"; chapter.chapterTitle = "第五幕 · 这道题";
            chapter.sourceNotes = Notes;
            chapter.beats = Create(); AssetDatabase.CreateAsset(chapter, AssetPath);
        }
        else if (MatchesLegacy(chapter.beats) || MatchesMandatory(chapter.beats))
        {
            chapter.beats = Create();
            chapter.sourceNotes = Notes;
            EditorUtility.SetDirty(chapter);
        }
        bool changed = false;
        var chapters = new List<StorySequence>(book.chapters ?? new StorySequence[0]);
        if (!chapters.Any(c => c != null && c.storyId == chapter.storyId))
        {
            chapters.Add(chapter); book.chapters = chapters.ToArray(); changed = true;
            book.endingSummary = "两人第一次很自然地笑起来，约好了明天的午饭。";
            book.nextPreview = "后续 · 鸡排饭和英语题（制作中）";
        }
        var pictures = new List<StoryPicture>(book.pictures ?? new StoryPicture[0]);
        for (int i = 0; i < Shots.Length; i++)
            if (!pictures.Any(p => p != null && p.shot == Shots[i]))
            { pictures.Add(new StoryPicture { shot = Shots[i], texture = textures[i] }); changed = true; }
        if (changed) { book.pictures = pictures.ToArray(); EditorUtility.SetDirty(book); }
    }
}

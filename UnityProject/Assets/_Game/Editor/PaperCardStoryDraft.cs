using System.Collections.Generic;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class PaperCardStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/PaperCardSequence.asset";
    public static readonly string[] Pictures = { "ForgottenCards", "GymQueue", "MissingCard", "CardReturn", "LookBack" };
    public static readonly StoryShot[] Shots = { StoryShot.ForgottenCards, StoryShot.GymQueue, StoryShot.MissingCard, StoryShot.CardReturn, StoryShot.LookBack };
    private static StoryBeat[] CreatePrevious()
    {
        return new[] {
            B("", "第四幕 · 你的核酸码\n之后的某一天。", StoryShot.ForgottenCards, true),
            B("同学", "带上核酸码，体育馆！", StoryShot.ForgottenCards),
            B("凌赫", "来了。", StoryShot.ForgottenCards),
            B("", "凌赫从自己的书下抽出纸卡，发现李晚云的还在桌面。\n她的座位已经空了。", StoryShot.ForgottenCards),
            B("凌赫心声", "这都能忘。", StoryShot.ForgottenCards),
            B("", "他把两张纸卡一起拿上。", StoryShot.ForgottenCards),
            B("", "体育馆里，队伍缓缓往前挪。李晚云站在前面看书。", StoryShot.GymQueue, true),
            B("", "凌赫看了看手里的码，没有叫她。", StoryShot.GymQueue),
            B("凌赫心声", "等会儿再给。", StoryShot.GymQueue),
            B("", "轮到她了。她把书夹到腋下，摸校服口袋，又摸另一边。\n身后的人向前挪了一步。", StoryShot.MissingCard),
            B("班主任", "晚云，你的码——", StoryShot.MissingCard),
            B("", "凌赫用双指夹起纸卡，另一只手抵着额头，故作从容。", StoryShot.CardReturn),
            B("凌赫", "小姐，你的核酸码。", StoryShot.CardReturn),
            B("李晚云", "谢……谢谢。", StoryShot.CardReturn),
            B("", "她接得很快，脸涨红了。完成检测后，她小步走开。", StoryShot.LookBack),
            B("班主任", "你自己的呢？别光顾着耍宝。", StoryShot.LookBack),
            B("凌赫", "在，在。", StoryShot.LookBack),
            B("", "他赶紧摸了摸口袋，收回摆姿势的手，跟上队伍。", StoryShot.LookBack),
            B("", "离开体育馆时，李晚云回头看了他一眼。\n他正把纸卡重新折进兜里。", StoryShot.LookBack),
            B("", "她转回去，嘴角有一点笑。", StoryShot.LookBack)
        };
    }
    public static StoryBeat[] Create()
    {
        var beats=CreatePrevious();
        for(int i=0;i<beats.Length;i++)beats[i].id="card-v2-"+i.ToString("D2");
        beats[7].text="凌赫看了看手里的码，往前靠近了一点。";
        beats[8].text="还好一起带来了。";
        beats[11].text="她的纸卡就在手里。凌赫想了想，准备递过去。";
        beats[11].shot=StoryShot.MissingCard;beats[11].interaction=StoryInteraction.CardChoice;
        return beats;
    }
    private static bool MatchesPrevious(StoryBeat[] actual)
    {var old=CreatePrevious();return actual!=null&&actual.Length==old.Length&&actual.Zip(old,(a,b)=>a!=null&&a.text==b.text&&a.speaker==b.speaker&&a.shot==b.shot&&a.fade==b.fade&&a.focus==b.focus&&a.interaction==StoryInteraction.None&&!a.requiresMathAnswer&&!a.requiresRoute).All(v=>v);}
    private static StoryBeat B(string speaker, string text, StoryShot shot, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }

    public static void Install(StoryBook book)
    {
        var textures = Pictures.Select(p => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/PaperCard/" + p + ".png")).ToArray();
        if (book == null || textures.Any(t => t == null)) return;
        var chapter = AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if (chapter == null)
        {
            chapter = ScriptableObject.CreateInstance<StorySequence>();
            chapter.storyId = "campus-act-04"; chapter.chapterTitle = "第四幕 · 你的核酸码";
            chapter.sourceNotes = "MAIN_STORY_V0.3 MS-04。保留7句角色对白与2句凌赫心声，舞台动作转为旁白；虚构纸卡，固定事件，暂不加入取卡/递卡玩法。";
            chapter.beats = Create(); AssetDatabase.CreateAsset(chapter, AssetPath);
        }
        else if(MatchesPrevious(chapter.beats))
        {chapter.beats=Create();chapter.sourceNotes="2026-11-02：直接递卡或故作从容，两者均正常归还，不设关系分数；20段正文，表达差异由播放器分开呈现。";EditorUtility.SetDirty(chapter);}
        bool changed = false;
        var chapters = new List<StorySequence>(book.chapters ?? new StorySequence[0]);
        if (!chapters.Any(c => c != null && c.storyId == chapter.storyId))
        {
            chapters.Add(chapter); book.chapters = chapters.ToArray(); changed = true;
            book.endingSummary = "她转回去，嘴角有一点笑。";
            book.nextPreview = "后续 · 这道题（制作中）";
        }
        var pictures = new List<StoryPicture>(book.pictures ?? new StoryPicture[0]);
        for (int i = 0; i < Shots.Length; i++)
            if (!pictures.Any(p => p != null && p.shot == Shots[i]))
            { pictures.Add(new StoryPicture { shot = Shots[i], texture = textures[i] }); changed = true; }
        var direct=AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/PaperCard/DirectCardReturn.png");
        if(direct!=null&&!pictures.Any(p=>p.shot==StoryShot.DirectCardReturn))
        {pictures.Add(new StoryPicture{shot=StoryShot.DirectCardReturn,texture=direct});changed=true;}
        if (changed) { book.pictures = pictures.ToArray(); EditorUtility.SetDirty(book); }
    }
}

using System.Collections.Generic;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class WaterCupStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/WaterCupSequence.asset";
    public static readonly string[] Pictures = { "EveningStudy", "SpilledWater", "EyeContact", "EyeContactCloseup", "Mopping", "WetSleeve" };
    public static readonly StoryShot[] Shots = { StoryShot.EveningStudy, StoryShot.SpilledWater, StoryShot.EyeContact, StoryShot.EyeContactCloseup, StoryShot.Mopping, StoryShot.WetSleeve };
    private static StoryBeat[] CreatePrevious()
    {
        return new[] {
            B("", "第三幕 · 水杯\n当日晚自习，七班。", StoryShot.EveningStudy, true),
            B("", "李晚云把刚接好水的杯子放在桌子左上角，杯盖还没拧上。\n凌赫写累了，伸了个懒腰。", StoryShot.EveningStudy),
            B("", "手臂碰到了杯子。水落向地面，杯盖滚了一小段。\n李晚云往后收了收腿。", StoryShot.SpilledWater),
            B("凌赫", "对不起，对不起！碰到你没有？", StoryShot.SpilledWater),
            B("李晚云", "没有。", StoryShot.SpilledWater),
            B("凌赫", "你别动，我来。", StoryShot.SpilledWater),
            B("", "两人同时弯下腰。他先拾起杯子，她抬起头，刘海从脸侧滑开。", StoryShot.EyeContact),
            B("", "视线碰上了一瞬，又各自移开。", StoryShot.EyeContact),
            B("李晚云", "杯盖……在你那边。", StoryShot.EyeContact),
            B("凌赫", "哦。", StoryShot.EyeContact),
            B("", "他把杯子放好，拿着杯盖愣了半秒，才一起放下。", StoryShot.EyeContact),
            B("凌赫", "我去拿拖把。", StoryShot.EyeContact),
            B("", "凌赫拿来拖把，把地上的水慢慢收拢。", StoryShot.Mopping, true),
            B("李晚云", "这边还有一点。", StoryShot.Mopping),
            B("凌赫", "看到了。", StoryShot.Mopping),
            B("", "收拾后，他回到座位，练习册还停在伸懒腰前的那道题。", StoryShot.WetSleeve),
            B("李晚云", "你袖子也湿了。", StoryShot.WetSleeve),
            B("", "他低下头，拿干的那只手把袖口卷了上去。", StoryShot.WetSleeve)
        };
    }
    public static StoryBeat[] Create()
    {
        var old=CreatePrevious();var list=new List<StoryBeat>();
        for(int i=0;i<old.Length;i++)
        {
            if(i==13||i==14)continue;var b=old[i];b.id="water-v2-"+i.ToString("D2");
            if(i==7){b.shot=StoryShot.EyeContactCloseup;b.text="视线碰上了一瞬，两人的脸都有些发热，又各自移开。";}
            if(i==12)b.interaction=StoryInteraction.Mop;
            list.Add(b);
        }
        return list.ToArray();
    }
    private static bool MatchesPrevious(StoryBeat[] actual)
    {var old=CreatePrevious();return actual!=null&&actual.Length==old.Length&&actual.Zip(old,(a,b)=>a!=null&&a.text==b.text&&a.speaker==b.speaker&&a.shot==b.shot&&a.fade==b.fade&&a.focus==b.focus&&a.interaction==StoryInteraction.None&&!a.requiresMathAnswer&&!a.requiresRoute).All(v=>v);}
    private static StoryBeat B(string speaker, string text, StoryShot shot, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }

    // Append only the new chapter and missing picture mappings; preserve author edits.
    public static void Install(StoryBook book)
    {
        var textures = Pictures.Select(p => AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/WaterCup/" + p + ".png")).ToArray();
        if (book == null || textures.Any(t => t == null)) return;
        var chapter = AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if (chapter == null)
        {
            chapter = ScriptableObject.CreateInstance<StorySequence>();
            chapter.storyId = "campus-act-03"; chapter.chapterTitle = "第三幕 · 水杯";
            chapter.sourceNotes = "MAIN_STORY_V0.3 MS-03。保留9句角色对白，舞台动作改为旁白；线性图片演出，不含杯盖点击或拖地玩法。";
            chapter.beats = Create(); AssetDatabase.CreateAsset(chapter, AssetPath);
        }
        else if(MatchesPrevious(chapter.beats))
        {chapter.beats=Create();chapter.sourceNotes="2026-11-02作者修订：脸红对视特写、拖把擦水，两句补擦对白进入小游戏；16段正文。";EditorUtility.SetDirty(chapter);}
        bool changed = false;
        var chapters = new List<StorySequence>(book.chapters ?? new StorySequence[0]);
        if (!chapters.Any(c => c != null && c.storyId == chapter.storyId))
        {
            chapters.Add(chapter); book.chapters = chapters.ToArray(); changed = true;
            book.endingSummary = "他翻回练习册，袖口还留着一点湿意。";
            book.nextPreview = "后续 · 你的核酸码（制作中）";
        }
        var pictures = new List<StoryPicture>(book.pictures ?? new StoryPicture[0]);
        for (int i = 0; i < Shots.Length; i++)
            if (!pictures.Any(p => p != null && p.shot == Shots[i]))
            { pictures.Add(new StoryPicture { shot = Shots[i], texture = textures[i] }); changed = true; }
        if (changed) { book.pictures = pictures.ToArray(); EditorUtility.SetDirty(book); }
    }
}

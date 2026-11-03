using System;
using System.Linq;
using System.Collections.Generic;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class NoteStoryDraft
{
    public const string AssetPath="Assets/_Game/Resources/Story/NoteSequence.asset";
    public static readonly string[] Pictures={"Prepare","Delivery","Dorm","Keep"};
    public static readonly StoryShot[] Shots={StoryShot.NotePrepare,StoryShot.NoteDelivery,StoryShot.DormNote,StoryShot.NoteKept};
    public static StoryBeat[] Create()
    {
        var b=new[]{
            B("","第七幕 · 回宿舍再看\n一段日常相处之后，晚自习结束。"),
            B("","李晚云在收书。凌赫将一张纸从本子里抽出来。"),
            B("","凌赫把纸收好，准备递给她。"),
            B("凌赫","回宿舍再看。",StoryShot.NoteDelivery),
            B("李晚云","什么啊？",StoryShot.NoteDelivery),
            B("凌赫","反正不是作业。",StoryShot.NoteDelivery),
            B("","她还想问，他已经往朋友那边走。",StoryShot.NoteDelivery),
            B("","当晚 · 女生宿舍",StoryShot.DormNote,true),
            B("","李晚云坐到书桌旁，打开那张纸。",StoryShot.DormNote),
            B("纸条","Mg + ZnSO<sub>4</sub> → MgSO<sub>4</sub> + Zn",StoryShot.DormNote),
            B("李晚云","这不是置换反应吗……",StoryShot.DormNote),
            B("室友一","什么东西，藏那么快？",StoryShot.DormNote),
            B("李晚云","没藏。",StoryShot.DormNote),
            B("室友二","化学？你们回宿舍还给彼此留题啊？",StoryShot.DormNote),
            B("室友三","这个我见过。你的镁——美，偷走了我的锌——心。",StoryShot.DormNote),
            B("","另外两人拖长一声“哦”。李晚云把纸压在本子下面。",StoryShot.DormNote),
            B("室友二","凌赫给的吧？就那个被调到讲台边上的。",StoryShot.DormNote),
            B("室友一","我看他天天找你问英语。",StoryShot.DormNote),
            B("李晚云","也没有天天……我也问他数学的。",StoryShot.DormNote),
            B("室友三","那明天我替你问他，这题什么意思？",StoryShot.DormNote),
            B("李晚云","别。",StoryShot.DormNote),
            B("","她立刻抬起头。室友笑起来，没有继续逼问。",StoryShot.DormNote),
            B("","熄灯前，她把纸从本子里取出来。",StoryShot.NoteKept,true),
            B("","她沿着原来的折痕重新折好，放回本子里。",StoryShot.NoteKept),
            B("","她的嘴角仍有一点笑。",StoryShot.NoteKept)
        };
        b[2].interaction=StoryInteraction.FoldNote;
        for(int i=0;i<b.Length;i++)b[i].id="note-v1-"+i.ToString("D2");return b;
    }
    private static StoryBeat B(string speaker,string text,StoryShot shot=StoryShot.NotePrepare,bool fade=false)
        =>new StoryBeat{speaker=speaker,text=text,shot=shot,fade=fade,focus=.5f};
    public static void Install(StoryBook book)
    {
        var textures=Pictures.Select(n=>AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/Note/"+n+".png")).ToArray();
        if(book==null||textures.Any(t=>t==null))return;
        var sequence=AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if(sequence==null){sequence=ScriptableObject.CreateInstance<StorySequence>();sequence.storyId="campus-act-07";sequence.chapterTitle="第七幕 · 回宿舍再看";sequence.sourceNotes="MAIN_STORY_V0.3 MS-07：原对白与化学谐音；舞台文字适配。作者2026-11-03要求暂缓第六幕先制作第七幕，保留时间跨度，不视为删掉第六幕。宿舍为全知线性演出；室友造型为临时视觉补充。";sequence.beats=Create();AssetDatabase.CreateAsset(sequence,AssetPath);}
        bool changed=false;
        var chapters=new List<StorySequence>(book.chapters??new StorySequence[0]);
        if(!chapters.Any(c=>c!=null&&c.storyId==sequence.storyId)){chapters.Add(sequence);book.chapters=chapters.ToArray();book.endingSummary="熄灯前，她沿原折痕收好纸条，放回本子里。";book.nextPreview="后续 · 第一张合照（待制作）";changed=true;}
        var pictures=new List<StoryPicture>(book.pictures??new StoryPicture[0]);
        for(int i=0;i<Shots.Length;i++)if(!pictures.Any(p=>p.shot==Shots[i])){pictures.Add(new StoryPicture{shot=Shots[i],texture=textures[i]});changed=true;}
        if(changed){book.pictures=pictures.ToArray();EditorUtility.SetDirty(book);}
    }
    public static void Check()
    {
        var s=AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if(s==null||!s.IsValid()||s.storyId!="campus-act-07"||s.beats.Length!=25||s.beats.Count(b=>b.interaction==StoryInteraction.FoldNote)!=1||s.beats[2].interaction!=StoryInteraction.FoldNote||s.beats[3].shot!=StoryShot.NoteDelivery||s.beats[7].shot!=StoryShot.DormNote||s.beats[9].text!="Mg + ZnSO<sub>4</sub> → MgSO<sub>4</sub> + Zn"||s.beats.Skip(3).Any(b=>b.interaction!=StoryInteraction.None))throw new Exception("Act7 order or resources invalid; author edits preserved.");
        var state=new StoryNoteState();if(state.Give()||!state.Fold()||state.Fold()||!state.Open()||state.Give()||!state.Fold()||!state.Give()||state.Give()||state.Open())throw new Exception("Note fold/give guard failed");state.Reset();if(state.Current!=StoryNoteState.Stage.Open)throw new Exception("Note replay failed");
        var book=AssetDatabase.LoadAssetAtPath<StoryBook>("Assets/_Game/Resources/Story/Catalog/BesideYou.asset");
        if(book==null||book.chapters.Any(c=>c==null)||!book.pictures.Any(p=>p.shot==StoryShot.NoteKept&&p.texture!=null))throw new Exception("Act7 catalog missing");
        var ids=book.chapters.Select(c=>c.storyId).ToArray();var counts=book.chapters.Select(c=>c.beats.Length).ToArray();int at=Array.IndexOf(ids,"campus-act-07");
        if(at<1)throw new Exception("Act7 missing");
        var old=new StoryReadingSave{completed=ids.Take(at).ToArray(),chapterId=ids[at-1],beat=counts[at-1]-1};
        var extended=new StoryReadingState(ids,counts,old);if(!extended.IsUnlocked(at)||extended.Chapter!=at)throw new Exception("Old completed reader cannot continue act7");
        if(!JsonUtility.FromJson<StoryReadingSave>(JsonUtility.ToJson(new StoryReadingSave{noteGiven=true})).noteGiven)throw new Exception("Note flag JSON failed");
        Debug.Log("ACT7_CHECK_PASS: chapter 07; 25 beats; fold before give; dorm is linear; formula and state guards; no saves changed");
    }
}

using System;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class StoryGentleChecks
{
    [MenuItem("Tools/Story Prototype/Check Acts 3 4 5 Interactions")]
    public static void Run()
    {
        CheckState();
        var water=AssetDatabase.LoadAssetAtPath<StorySequence>(WaterCupStoryDraft.AssetPath);
        var card=AssetDatabase.LoadAssetAtPath<StorySequence>(PaperCardStoryDraft.AssetPath);
        var math=AssetDatabase.LoadAssetAtPath<StorySequence>(MathHelpStoryDraft.AssetPath);
        Require(water!=null&&water.beats.Length==16&&water.beats[7].shot==StoryShot.EyeContactCloseup&&water.beats[12].interaction==StoryInteraction.Mop,"mop and closeup chapter order");
        Require(water.beats[11].text=="我去拿拖把。"&&water.beats[13].shot==StoryShot.WetSleeve,"mop before return to seat");
        Require(card!=null&&card.beats.Length==20&&card.beats[11].interaction==StoryInteraction.CardChoice&&card.beats[11].shot==StoryShot.MissingCard,"card expression choice before performance");
        Require(math!=null&&math.beats[13].interaction==StoryInteraction.CooperateMath&&!math.beats.Any(b=>b.requiresMathAnswer)&&math.beats[20].id=="math-v2-20","math no forced numerical answer");
        var book=AssetDatabase.LoadAssetAtPath<StoryBook>("Assets/_Game/Resources/Story/Catalog/BesideYou.asset");
        Require(book.pictures.Any(p=>p.shot==StoryShot.EyeContactCloseup&&p.texture!=null)&&book.pictures.Any(p=>p.shot==StoryShot.DirectCardReturn&&p.texture!=null),"new art references");
        var save=new StoryReadingSave{cardStyle=1,contentVersion=4,chapterId="campus-act-04",beatId="card-v2-12",beat=12};
        var restored=JsonUtility.FromJson<StoryReadingSave>(JsonUtility.ToJson(save));
        Require(restored.cardStyle==1&&restored.beatId==save.beatId,"card expression survives JSON roundtrip");
        Debug.Log("STORY_GENTLE_CHECK_PASS: mop two stages, drag coverage, alternative controls, card expression references/save and maths without compulsory answer; no player save modified");
    }
    public static void CheckState()
    {
        var mop=new StoryMopState();
        Require(mop.Cleaned==0&&mop.Current==StoryMopState.Stage.Cleaning&&!mop.Acknowledge(),"fresh mop");
        Require(!mop.Wipe(-1,0,0,0)&&!mop.Wipe(double.NaN,0,0,0)&&mop.Cleaned==0,"outside stroke rejected");
        for(int y=0;y<7;y++)mop.Wipe(.01,StoryMopState.Y(y*7),.99,StoryMopState.Y(y*7));
        Require(mop.Cleaned==45&&mop.Current==StoryMopState.Stage.Notice,"swept strokes clean main water and preserve final patch");
        Require(!mop.Wipe(0,0,1,1)&&mop.Cleaned==45,"notice blocks premature final wipe");
        Require(mop.Acknowledge()&&!mop.Acknowledge(),"reply only once");
        while(mop.Current!=StoryMopState.Stage.Complete)mop.CleanNext();
        Require(mop.Cleaned==49&&!mop.Wipe(0,0,1,1),"full cleanup and repeated completion");
        mop.Reset();Require(mop.Cleaned==0&&mop.Current==StoryMopState.Stage.Cleaning,"mop replay");
        for(int i=0;i<60;i++)mop.CleanNext();Require(mop.Cleaned==45,"button alternative respects late patch");
        var coop=new StoryCoopMathState();Require(coop.Advance()&&coop.Advance()&&coop.Advance()&&coop.Complete&&!coop.Advance(),"cooperation completes without answer input");
        coop.Reset();Require(coop.Step==0&&!coop.Complete,"cooperation replay");
        Require((int)CardReturnStyle.Direct==1&&(int)CardReturnStyle.Playful==2,"two expressions remain distinct");
    }
    private static void Require(bool ok,string why){if(!ok)throw new Exception("Gentle check failed: "+why);}
}

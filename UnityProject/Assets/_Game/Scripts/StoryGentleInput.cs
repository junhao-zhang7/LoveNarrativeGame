using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private RectTransform mopPanel,mopArea,mopTool,cardPanel,coopPanel;
        private StoryMopState mopState=new StoryMopState();
        private StoryCoopMathState coopState=new StoryCoopMathState();
        private TMP_Text mopFeedback,coopText,coopNextText;
        private Button mopAck,mopDone,mopAlternative;
        private bool mopPrevious; private Vector2 mopLast;
        private CardReturnStyle readingCardStyle;
        private bool GentleActive => mopPanel.gameObject.activeInHierarchy||cardPanel.gameObject.activeInHierarchy||coopPanel.gameObject.activeInHierarchy;
        private bool CanGentle(RectTransform panel) => reading&&!paused&&!transitioning&&panel.gameObject.activeInHierarchy;
        private void BuildGentleInput()
        {
            BuildMop();
            cardPanel=Paper("Return card choice",playback,new Vector2(960,185),new Vector2(1720,310),new Color(.98f,.97f,.93f,.86f));
            Text(cardPanel,new Vector2(860,245),new Vector2(1400,55),"她在找的纸卡，就在你手里。",30,true,blue);
            Button(cardPanel,new Vector2(450,135),new Vector2(700,80),"叫她一声，直接递过去",()=>ChooseCard(CardReturnStyle.Direct),29);
            Button(cardPanel,new Vector2(1270,135),new Vector2(700,80),"夹起纸卡，故作从容",()=>ChooseCard(CardReturnStyle.Playful),29);
            Text(cardPanel,new Vector2(860,55),new Vector2(1300,40),"怎样把纸卡递给她？",23,true,blue);
            cardPanel.gameObject.SetActive(false);
            coopPanel=Paper("Shared maths notebook",playback,new Vector2(960,230),new Vector2(1720,400),new Color(.98f,.97f,.93f,.87f));
            Text(coopPanel,new Vector2(860,350),new Vector2(1300,50),"把练习本转到两人中间",30,true,blue);
            coopText=Text(coopPanel,new Vector2(860,220),new Vector2(1510,180),"",29);
            var next=Button(coopPanel,new Vector2(700,65),new Vector2(1020,70),"一起看区间两端",AdvanceCoop,28);coopNextText=next.GetComponentInChildren<TMP_Text>();
            Button(coopPanel,new Vector2(1430,65),new Vector2(390,70),"我想自己算",OpenOptionalMath,25);
            coopPanel.gameObject.SetActive(false);
            Button(mathPanel,new Vector2(1370,288),new Vector2(500,50),"先不填了，一起推导",ReturnToCoop,23);
        }
        private void PresentGentle(StoryBeat beat)
        {
            mopPanel.gameObject.SetActive(beat.interaction==StoryInteraction.Mop);
            cardPanel.gameObject.SetActive(beat.interaction==StoryInteraction.CardChoice);
            coopPanel.gameObject.SetActive(beat.interaction==StoryInteraction.CooperateMath);
            if(beat.interaction==StoryInteraction.Mop) BeginMop();
            if(beat.interaction==StoryInteraction.CardChoice)readingCardStyle=CardReturnStyle.Unchosen;
            if(beat.interaction==StoryInteraction.CooperateMath){coopState.Reset();RefreshCoop();}
        }
        private void ChooseCard(CardReturnStyle style)
        {if(!CanGentle(cardPanel)||style==CardReturnStyle.Unchosen)return;readingCardStyle=style;FinishGentle();}
        private StoryBeat ResolveCardBeat(StoryBeat original)
        {
            if(readingCardStyle!=CardReturnStyle.Direct||books[bookIndex].chapters[chapterIndex].storyId!="campus-act-04")return original;
            int i=progress.Index;string text=null;
            if(i==12)text="你的核酸码。我帮你带过来了。";
            if(i==13)text="谢谢，还好你带来了。";
            if(i==14)text="她接过纸卡，松了口气，完成检测后小步走开。";
            if(i==15)text="凌赫，你自己的码也准备好。";
            if(i==16)text="在这儿。";
            if(i==17)text="他拿好自己的纸卡，跟上队伍。";
            if(text==null)return original;
            return new StoryBeat{id=original.id,speaker=original.speaker,text=text,shot=i<=13?StoryShot.DirectCardReturn:original.shot,focus=original.focus,fade=original.fade};
        }
        private void RefreshCoop()
        {
            if(coopState.Step==0){coopText.text="李晚云：我只把顶点代进去了……\n凌赫：我们一起看看，题目要的是区间里的最大值。";coopNextText.text="一起看看区间两端";}
            else if(coopState.Step==1){coopText.text="凌赫：f(2)=-22，f(14)=26。开口向上，最大值在两端比较。\n李晚云：那M就是26。我刚才算的是最小值。";coopNextText.text="把M=26写在本子上";}
            else {coopText.text="凌赫：m²+n²=(m+n)²-2mn=26²-2×91=494，不用分别解m和n。\n李晚云：还要把前面的M加上……我来写最后一步。";coopNextText.text="把本子转给她，接着算";}
        }
        private void AdvanceCoop()
        {
            if(!CanGentle(coopPanel))return;
            coopState.Advance();
            if(!coopState.Complete){RefreshCoop();return;}
            int target=System.Array.FindIndex(beats,b=>b.id=="math-v2-20");
            coopPanel.gameObject.SetActive(false);dialoguePanel.gameObject.SetActive(true);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
            while(progress.Index<target)if(!progress.Advance())break;
            saved.Bookmark(progress.Index);SaveReading();lastAdvanceFrame=Time.frameCount;ShowBeat();
        }
        private void OpenOptionalMath()
        {
            if(!CanGentle(coopPanel))return;coopPanel.gameObject.SetActive(false);mathPanel.gameObject.SetActive(true);
            mathInput.text="";mathFeedback.text="不想单独算也没关系，可以随时回到一起推导。";
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(mathInput.gameObject);mathInput.ActivateInputField();
        }
        private void ReturnToCoop()
        {if(!CanGentle(mathPanel))return;mathPanel.gameObject.SetActive(false);coopPanel.gameObject.SetActive(true);if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);RefreshCoop();}
        private void FinishGentle()
        {
            mopPanel.gameObject.SetActive(false);cardPanel.gameObject.SetActive(false);coopPanel.gameObject.SetActive(false);dialoguePanel.gameObject.SetActive(true);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);lastAdvanceFrame=Time.frameCount;AdvanceBeat();
        }
    }
}

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
        private bool GentleActive => mopPanel.gameObject.activeInHierarchy||cardPanel.gameObject.activeInHierarchy||coopPanel.gameObject.activeInHierarchy||notePanel.gameObject.activeInHierarchy;
        private bool CanGentle(RectTransform panel) => reading&&!paused&&!transitioning&&panel.gameObject.activeInHierarchy;
        private void BuildGentleInput()
        {
            BuildMop();
            BuildCardAndNotebook();
            BuildNote();
        }
        private void PresentGentle(StoryBeat beat)
        {
            mopPanel.gameObject.SetActive(beat.interaction==StoryInteraction.Mop);
            cardPanel.gameObject.SetActive(beat.interaction==StoryInteraction.CardChoice);
            coopPanel.gameObject.SetActive(beat.interaction==StoryInteraction.CooperateMath);
            if(beat.interaction==StoryInteraction.Mop) BeginMop();
            cardPerforming=false;
            if(beat.interaction==StoryInteraction.CardChoice){readingCardStyle=CardReturnStyle.Unchosen;cardCanvas.interactable=true;cardPrompt.text="她在找的纸卡，就在凌赫手里。";}
            if(beat.interaction==StoryInteraction.CooperateMath){coopState.Reset();independentMath=false;RefreshCoop();}
            PresentNotebookForBeat(beat);
            PresentNote(beat);
        }
        private void ChooseCard(CardReturnStyle style)
        {
            if(!CanGentle(cardPanel)||style==CardReturnStyle.Unchosen||cardPerforming)return;
            readingCardStyle=style;cardPerforming=true;cardWait=0;cardCanvas.interactable=false;
            cardPrompt.text=style==CardReturnStyle.Direct?"凌赫叫了她一声，将纸卡递过去。":"他捏住纸卡，指尖停了一下。";
            SaveReading();
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
        }
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
            RefreshNotebook(coopState.Step);
            optionalMathButton.gameObject.SetActive(!coopState.Complete);
            if(coopState.Step==0){coopText.text="李晚云：我只把顶点代进去了……\n凌赫：我们一起看看，题目要的是区间里的最大值。";coopNextText.text="先比较区间两端";}
            else if(coopState.Step==1){coopText.text="凌赫：两端分别是负二十二和二十六。\n李晚云：那最大的就是二十六。我刚才算的是最小值。";coopNextText.text="写下 M = 26";}
            else if(coopState.Step==2){coopText.text="李晚云：后面不用把 m 和 n 分别解出来？\n凌赫：不用。和的平方，减两倍的积。";coopNextText.text="接着算平方和";}
            else if(coopState.Step==3){coopText.text="凌赫：这一项是四百九十四，还要加上前面的 M。\n李晚云：最后一步我来写。";coopNextText.text="把笔留给她，写下最后一步";}
            else{coopText.text=independentMath?"答案写下了。把刚才的思路讲给她听。":"她接过笔，在最后一行落下了结果。";coopNextText.text=independentMath?"把思路讲给她听 →":"接着听她说 →";}
        }
        private void AdvanceCoop()
        {
            if(!CanGentle(coopPanel)||lastCoopFrame==Time.frameCount)return;
            lastCoopFrame=Time.frameCount;
            foreach(var line in notebookLines)if(line.maxVisibleCharacters<line.text.Length){foreach(var note in notebookLines)note.maxVisibleCharacters=int.MaxValue;return;}
            if(!coopState.Complete){coopState.Advance();RefreshCoop();return;}
            if(independentMath){FinishGentle();return;}
            int target=System.Array.FindIndex(beats,b=>b.id=="math-v2-20");
            coopPanel.gameObject.SetActive(false);dialoguePanel.gameObject.SetActive(true);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
            while(progress.Index<target)if(!progress.Advance())break;
            saved.Bookmark(progress.Index);SaveReading();lastAdvanceFrame=Time.frameCount;ShowBeat();
        }
        private void OpenOptionalMath()
        {
            if(!CanGentle(coopPanel))return;coopPanel.gameObject.SetActive(false);mathPanel.gameObject.SetActive(true);
            mathInput.text="";mathFeedback.text="在左页空白处写下结果；也可以随时回到一起推导。";
            notebookWriting.gameObject.SetActive(false);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(mathInput.gameObject);mathInput.ActivateInputField();
        }
        private void ReturnToCoop()
        {if(!CanGentle(mathPanel))return;mathPanel.gameObject.SetActive(false);coopPanel.gameObject.SetActive(true);if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);notebookWriting.gameObject.SetActive(true);RefreshCoop();}
        private void FinishGentle()
        {
            mopPanel.gameObject.SetActive(false);cardPanel.gameObject.SetActive(false);coopPanel.gameObject.SetActive(false);dialoguePanel.gameObject.SetActive(true);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);lastAdvanceFrame=Time.frameCount;AdvanceBeat();
        }
    }
}

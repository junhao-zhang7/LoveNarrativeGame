using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private CanvasGroup cardCanvas;
        private TMP_Text cardPrompt;
        private bool cardPerforming,independentMath;
        private float cardWait,notebookInk;
        private int lastCoopFrame=-1;
        private RectTransform notebookWriting;
        private TMP_Text[] notebookLines;
        private Button optionalMathButton;
        private void BuildCardAndNotebook()
        {
            cardPanel=Paper("Card expression in dialogue",playback,new Vector2(960,158),new Vector2(1720,258),new Color(.98f,.97f,.93f,.55f));
            cardCanvas=cardPanel.gameObject.AddComponent<CanvasGroup>();
            cardPrompt=Text(cardPanel,new Vector2(860,208),new Vector2(1510,48),"她在找的纸卡，就在凌赫手里。",28,false,blue);
            Button(cardPanel,new Vector2(440,111),new Vector2(720,72),"叫她一声，直接递过去",()=>ChooseCard(CardReturnStyle.Direct),28);
            Button(cardPanel,new Vector2(1260,111),new Vector2(720,72),"夹起纸卡，故作从容",()=>ChooseCard(CardReturnStyle.Playful),28);
            Text(cardPanel,new Vector2(860,36),new Vector2(1500,35),"选择你想怎样表达，两种方式都会把纸卡还给她。",18,true,blue);
            cardPanel.gameObject.SetActive(false);
            notebookWriting=Rect("Ink on the existing notebook",playback,new Vector2(960,540),new Vector2(1920,1080));
            notebookLines=new TMP_Text[4];
            for(int i=0;i<4;i++)
            {
                notebookLines[i]=Text(notebookWriting,new Vector2(850,422-i*39),new Vector2(410,38),"",27,false,ink);
                notebookLines[i].rectTransform.localRotation=Quaternion.Euler(0,0,14);
            }
            notebookWriting.gameObject.SetActive(false);
            coopPanel=Paper("Cooperation in dialogue",playback,new Vector2(960,110),new Vector2(1720,190),new Color(.98f,.97f,.93f,.55f));
            coopText=Text(coopPanel,new Vector2(860,137),new Vector2(1530,76),"",27);
            var next=Button(coopPanel,new Vector2(650,44),new Vector2(1020,62),"先比较区间两端",AdvanceCoop,25);coopNextText=next.GetComponentInChildren<TMP_Text>();
            optionalMathButton=Button(coopPanel,new Vector2(1430,44),new Vector2(360,62),"我想自己算",OpenOptionalMath,24);
            coopPanel.gameObject.SetActive(false);
            Button(mathPanel,new Vector2(490,68),new Vector2(600,62),"先不填了，一起推导",ReturnToCoop,24);
        }
        private void TickCardPerformance()
        {
            if(!cardPerforming||!CanGentle(cardPanel))return;
            cardWait+=Time.unscaledDeltaTime;
            if(cardWait<(readingCardStyle==CardReturnStyle.Direct?.25f:.80f))return;
            cardPerforming=false;FinishGentle();
        }
        private void RefreshNotebook(int step)
        {
            string[] lines={"f(2) = -22，f(14) = 26","M = 26","m²+n² = 26² - 2×91 = 494","26 + 494 = 520"};
            for(int i=0;i<4;i++)
            {
                string text=i<step?lines[i]:"";
                if(notebookLines[i].text==text)continue;
                notebookLines[i].text=text;notebookLines[i].maxVisibleCharacters=i==step-1?0:int.MaxValue;
            }
            notebookInk=0;
        }
        private void TickNotebookInk()
        {
            if(!notebookWriting.gameObject.activeInHierarchy)return;
            notebookInk+=Time.unscaledDeltaTime*44;
            foreach(var line in notebookLines)
                if(line.maxVisibleCharacters<line.text.Length)line.maxVisibleCharacters=Mathf.Min(line.text.Length,Mathf.FloorToInt(notebookInk));
        }
        private void PresentNotebookForBeat(StoryBeat beat)
        {
            int index=-1;
            if(beat.shot==StoryShot.ProblemNotebook&&beat.id!=null&&beat.id.StartsWith("math-v2-"))int.TryParse(beat.id.Substring(8),out index);
            bool explanation=index>=14&&index<=19;
            notebookWriting.gameObject.SetActive(beat.interaction==StoryInteraction.CooperateMath||explanation);
            if(explanation)RefreshNotebook(index==14?1:index<=18?2:3);
        }
        private void CompleteOptionalMath()
        {
            independentMath=true;
            while(!coopState.Complete)coopState.Advance();
            mathPanel.gameObject.SetActive(false);coopPanel.gameObject.SetActive(true);notebookWriting.gameObject.SetActive(true);
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
            RefreshCoop();lastAdvanceFrame=Time.frameCount;
        }
    }
}

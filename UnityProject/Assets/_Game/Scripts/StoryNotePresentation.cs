using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private RectTransform notePanel,noteSheet,noteCrease;
        private readonly StoryNoteState noteState=new StoryNoteState();
        private TMP_Text noteFormula,notePrompt;
        private Button noteFold,noteOpen,noteGive;
        private float noteFoldAmount;
        private bool noteAnimating,readingNoteGiven;
        private void BuildNote()
        {
            notePanel=Rect("Fold the note before giving it",playback,new Vector2(960,540),new Vector2(1920,1080));
            Fill("Focus on paper",notePanel,new Vector2(960,540),new Vector2(1920,1080),new Color(.12f,.18f,.22f,.55f)).raycastTarget=false;
            noteSheet=Paper("Chemical note",notePanel,new Vector2(960,600),new Vector2(1050,430),new Color(.98f,.97f,.92f));
            noteFormula=Text(noteSheet,new Vector2(525,240),new Vector2(950,90),"Mg + ZnSO<sub>4</sub> → MgSO<sub>4</sub> + Zn",43,true,ink);
            var crease=Rect("Saved paper crease",noteSheet,new Vector2(525,215),new Vector2(980,3));
            var stroke=crease.gameObject.AddComponent<MenuInkGraphic>();stroke.kind=MenuInkGraphic.Shape.Stroke;stroke.color=new Color(.43f,.43f,.36f,.3f);stroke.raycastTarget=false;noteCrease=crease;
            var footer=Paper("Paper action dialogue",notePanel,new Vector2(960,155),new Vector2(1720,250),new Color(.98f,.97f,.93f,.74f));
            notePrompt=Text(footer,new Vector2(860,185),new Vector2(1500,70),"",28,true,blue);
            noteFold=Button(footer,new Vector2(550,70),new Vector2(600,70),"沿着折痕折好",FoldNote,28);
            noteOpen=Button(footer,new Vector2(550,70),new Vector2(600,70),"再展开看一眼",OpenNote,27);
            noteGive=Button(footer,new Vector2(1250,70),new Vector2(500,70),"递给她 →",GiveNote,28);
            notePanel.gameObject.SetActive(false);
        }
        private void PresentNote(StoryBeat beat)
        {
            notePanel.gameObject.SetActive(beat.interaction==StoryInteraction.FoldNote);
            if(beat.interaction!=StoryInteraction.FoldNote)return;
            noteState.Reset();noteFoldAmount=0;noteAnimating=false;noteSheet.sizeDelta=new Vector2(1050,430);noteFormula.rectTransform.anchoredPosition=new Vector2(525,240);RefreshNote();
        }
        private void FoldNote(){if(CanGentle(notePanel)&&!noteAnimating&&noteState.Fold()){noteAnimating=true;RefreshNote();}}
        private void OpenNote(){if(CanGentle(notePanel)&&!noteAnimating&&noteState.Open()){noteAnimating=true;RefreshNote();}}
        private void GiveNote()
        {
            if(!CanGentle(notePanel)||noteAnimating||!noteState.Give())return;
            readingNoteGiven=true;notePanel.gameObject.SetActive(false);FinishGentle();
        }
        private void RefreshNote()
        {
            bool folded=noteState.Current==StoryNoteState.Stage.Folded;
            noteFormula.gameObject.SetActive(!folded&&!noteAnimating);
            noteFold.gameObject.SetActive(!folded);noteOpen.gameObject.SetActive(folded);noteGive.gameObject.SetActive(folded);noteGive.interactable=folded&&!noteAnimating;
            notePrompt.text=folded?"折痕留在纸上。还没有递出去。":"凌赫把纸放平，沿着中间的折痕收好。";
        }
        private void TickNote()
        {
            if(!CanGentle(notePanel))return;
            float target=noteState.Current==StoryNoteState.Stage.Open?0:1;
            noteFoldAmount=Mathf.MoveTowards(noteFoldAmount,target,Time.unscaledDeltaTime/.35f);
            noteSheet.sizeDelta=new Vector2(Mathf.Lerp(1050,680,noteFoldAmount),Mathf.Lerp(430,230,noteFoldAmount));
            noteCrease.anchoredPosition=noteSheet.sizeDelta/2;noteCrease.sizeDelta=new Vector2(noteSheet.sizeDelta.x-50,3);
            noteFormula.rectTransform.anchoredPosition=noteSheet.sizeDelta/2+new Vector2(0,25);
            if(noteAnimating&&Mathf.Approximately(noteFoldAmount,target)){noteAnimating=false;RefreshNote();}
        }
    }
}

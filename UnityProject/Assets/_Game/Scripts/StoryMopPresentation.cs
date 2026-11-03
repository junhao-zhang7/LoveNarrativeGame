using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private float[] mopOpacity, mopDamp, mopVisual;
        private MopWetSurface mopSurface;
        private Material mopCutoutMaterial;

        private void BuildMop()
        {
            mopPanel=Rect("Classroom floor mopping",playback,new Vector2(960,540),new Vector2(1920,1080));
            mopArea=Rect("Original classroom floor",mopPanel,new Vector2(960,540),new Vector2(1920,1080));
            var plate=mopArea.gameObject.AddComponent<RawImage>();
            plate.texture=Resources.Load<Texture2D>("Story/WaterCup/MopOverheadDry");plate.raycastTarget=true;
            mopOpacity=new float[mopState.Count];mopDamp=new float[mopState.Count];mopVisual=new float[mopState.Count];
            var locations=new Vector2[mopState.Count];for(int i=0;i<locations.Length;i++)locations[i]=MopFloorPoint(i);
            var surface=Rect("Illustrated puddle",mopArea,new Vector2(960,540),new Vector2(1920,1080));
            mopSurface=surface.gameObject.AddComponent<MopWetSurface>();mopSurface.raycastTarget=false;
            mopSurface.Configure(Resources.Load<Texture2D>("Story/WaterCup/MopOverheadWet"),locations,mopVisual);
            mopTool=Rect("Overhead mop",mopArea,new Vector2(420,350),new Vector2(260,310));
            mopTool.pivot=new Vector2(.5f,.74f);
            var mopSprite=mopTool.gameObject.AddComponent<RawImage>();
            mopSprite.texture=Resources.Load<Texture2D>("Story/WaterCup/MopOverheadTool");mopSprite.raycastTarget=false;
            mopSprite.uvRect=new Rect(.22f,0,.56f,1);
            var cutout=Resources.Load<Shader>("Story/WaterCup/MopCutout");
            if(cutout!=null){mopCutoutMaterial=new Material(cutout);mopSprite.material=mopCutoutMaterial;}
            var caption=Paper("Quiet instruction",mopPanel,new Vector2(960,1010),new Vector2(1160,80),new Color(.98f,.97f,.93f,.83f));
            Text(caption,new Vector2(580,40),new Vector2(1100,60),"按住鼠标，拖动拖把收干净地上的水。",27,true,blue);
            var feedback=Paper("Mopping response",mopPanel,new Vector2(960,110),new Vector2(1720,180),new Color(.98f,.97f,.93f,.76f));
            mopFeedback=Text(feedback,new Vector2(860,130),new Vector2(1500,65),"",27,true,blue);
            mopAlternative=Button(feedback,new Vector2(370,48),new Vector2(300,65),"帮我擦下一片",()=>{if(!CanGentle(mopPanel))return;for(int i=0;i<5;i++)mopState.CleanNext();RefreshMop();},23);
            mopAck=Button(feedback,new Vector2(1200,48),new Vector2(500,65),"凌赫：看到了。",()=>{if(!CanGentle(mopPanel))return;mopState.Acknowledge();mopPrevious=false;RefreshMop();},26);
            mopDone=Button(feedback,new Vector2(1200,48),new Vector2(500,65),"放好拖把，回到座位",()=>{if(CanGentle(mopPanel)&&mopState.Current==StoryMopState.Stage.Complete)FinishGentle();},26);
            mopPanel.gameObject.SetActive(false);
        }
        private static Vector2 MopFloorPoint(int i)
        {
            if(i>=45)return new Vector2(1400+(i-45)*22,765+(i%2)*20);
            float radius=Mathf.Sqrt((i+.5f)/45f),angle=i*2.399963f;
            return new Vector2(700+Mathf.Cos(angle)*radius*440,540+Mathf.Sin(angle)*radius*200);
        }
        private void BeginMop()
        {
            mopState.Reset();mopPrevious=false;
            for(int i=0;i<mopOpacity.Length;i++){mopOpacity[i]=mopState.Visible(i)?1:0;mopDamp[i]=0;mopVisual[i]=mopOpacity[i];}
            mopTool.anchoredPosition=new Vector2(420,350);mopTool.localScale=Vector3.one;mopSurface.RefreshSurface();RefreshMop();
        }
        private void TickMop()
        {
            if(!CanGentle(mopPanel))return;
            float dt=Time.unscaledDeltaTime;
            for(int i=0;i<mopOpacity.Length;i++)
            {
                float target=mopState.Visible(i) || (i>=45 && mopState.Current==StoryMopState.Stage.Notice) ? 1:0;
                if(target==0 && mopOpacity[i]>.7f)mopDamp[i]=1;
                mopOpacity[i]=Mathf.MoveTowards(mopOpacity[i],target,dt*3.2f);
                mopDamp[i]=Mathf.Max(0,mopDamp[i]-dt*.65f);
                mopVisual[i]=Mathf.Max(mopOpacity[i],mopDamp[i]*.10f);
            }
            mopSurface.RefreshSurface();
            if(Mouse.current==null||!Mouse.current.leftButton.isPressed){mopPrevious=false;return;}
            Vector2 local;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(mopArea,Mouse.current.position.ReadValue(),null,out local)){mopPrevious=false;return;}
            Vector2 point=local+new Vector2(960,540);
            if(point.x<0||point.x>1920||point.y<220||point.y>940){mopPrevious=false;return;}
            mopTool.anchoredPosition=point;
            Vector2 previous=mopPrevious?mopLast:point; bool changed=false;
            for(int i=0;i<mopState.Count;i++)
            {
                if(!mopState.Visible(i))continue;
                Vector2 delta=point-previous;float t=delta.sqrMagnitude<.001f?0:Mathf.Clamp01(Vector2.Dot(MopFloorPoint(i)-previous,delta)/delta.sqrMagnitude);
                if(Vector2.Distance(MopFloorPoint(i),previous+delta*t)<115)
                    changed=mopState.CleanPatch(i)||changed;
            }
            if(changed)RefreshMop();mopLast=point;mopPrevious=true;
        }
        private void RefreshMop()
        {
            bool notice=mopState.Current==StoryMopState.Stage.Notice,done=mopState.Current==StoryMopState.Stage.Complete;
            mopAck.gameObject.SetActive(notice);mopDone.gameObject.SetActive(done);mopAlternative.interactable=!notice&&!done;
            mopFeedback.text=notice?"李晚云：这边还有一点。":done?"水收干净了。把拖把放回去吧。":mopState.Current==StoryMopState.Stage.LastSpot?"凌赫：看到了。\n椅脚旁还剩一小片水。":"沿着地上的水光慢慢擦，不用着急。";
        }
    }
}

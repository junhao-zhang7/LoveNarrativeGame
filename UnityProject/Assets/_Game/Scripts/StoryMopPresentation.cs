using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed partial class StoryScreen
    {
        private MenuInkGraphic[] mopWater, mopGleam;
        private float[] mopOpacity, mopDamp;
        private MopFurnitureGraphic mopFurniture;
        private float mopTime;

        private void BuildMop()
        {
            mopPanel=Rect("Classroom floor mopping",playback,new Vector2(960,540),new Vector2(1920,1080));
            mopArea=Rect("Original classroom floor",mopPanel,new Vector2(960,540),new Vector2(1920,1080));
            var plate=mopArea.gameObject.AddComponent<RawImage>();
            plate.texture=Resources.Load<Texture2D>("Story/WaterCup/MopFloorPlate");plate.raycastTarget=true;
            wetSpots=new GameObject[mopState.Count];mopWater=new MenuInkGraphic[mopState.Count];
            mopGleam=new MenuInkGraphic[mopState.Count];mopOpacity=new float[mopState.Count];mopDamp=new float[mopState.Count];
            for(int i=0;i<wetSpots.Length;i++)
            {
                Vector2 p=MopFloorPoint(i);
                var spot=Rect("Water on terrazzo",mopArea,p,new Vector2(110+Mathf.Sin(i*7)*24,31+Mathf.Cos(i*3)*9));
                wetSpots[i]=spot.gameObject;
                var water=spot.gameObject.AddComponent<MenuInkGraphic>();water.kind=MenuInkGraphic.Shape.Dot;water.raycastTarget=false;mopWater[i]=water;
                var gleam=Rect("Window reflection on water",spot,new Vector2(spot.sizeDelta.x*.58f,spot.sizeDelta.y*.55f),new Vector2(spot.sizeDelta.x*.15f,2));
                mopGleam[i]=gleam.gameObject.AddComponent<MenuInkGraphic>();mopGleam[i].kind=MenuInkGraphic.Shape.Stroke;mopGleam[i].raycastTarget=false;
            }
            mopTool=Rect("Moving cotton mop",mopArea,new Vector2(430,210),new Vector2(300,400));
            mopTool.pivot=new Vector2(.54f,.08f);
            var mopSprite=mopTool.gameObject.AddComponent<RawImage>();
            mopSprite.texture=Resources.Load<Texture2D>("Story/WaterCup/MopTool");mopSprite.raycastTarget=false;
            var foreground=Rect("Furniture in front of mop",mopArea,new Vector2(960,540),new Vector2(1920,1080));
            mopFurniture=foreground.gameObject.AddComponent<MopFurnitureGraphic>();mopFurniture.plate=plate.texture;mopFurniture.raycastTarget=false;
            var caption=Paper("Quiet instruction",mopPanel,new Vector2(960,1010),new Vector2(1160,80),new Color(.98f,.97f,.93f,.83f));
            Text(caption,new Vector2(580,40),new Vector2(1100,60),"按住鼠标，拖动拖把收干净地上的水。",27,true,blue);
            var feedback=Paper("Mopping response",mopPanel,new Vector2(1350,850),new Vector2(1000,220),new Color(.98f,.97f,.93f,.76f));
            mopFeedback=Text(feedback,new Vector2(500,160),new Vector2(920,85),"",27,true,blue);
            mopAlternative=Button(feedback,new Vector2(210,57),new Vector2(300,65),"帮我擦下一片",()=>{if(!CanGentle(mopPanel))return;for(int i=0;i<5;i++)mopState.CleanNext();RefreshMop();},23);
            mopAck=Button(feedback,new Vector2(655,57),new Vector2(500,65),"凌赫：看到了。",()=>{if(!CanGentle(mopPanel))return;mopState.Acknowledge();mopPrevious=false;RefreshMop();},26);
            mopDone=Button(feedback,new Vector2(655,57),new Vector2(500,65),"放好拖把，回到座位",()=>{if(CanGentle(mopPanel)&&mopState.Current==StoryMopState.Stage.Complete)FinishGentle();},26);
            mopPanel.gameObject.SetActive(false);
        }
        private static Vector2 MopFloorPoint(int i)
        {
            // Main spill is on the exposed left floor; the last patch lies by the front chair foot.
            if(i>=45)return new Vector2(1230+(i-45)*18,240+(i%2)*10);
            float row=i/7; float width=.68f+.32f*Mathf.Sin(row*Mathf.PI/6);
            return new Vector2(370+((float)StoryMopState.X(i)-.5f)*440*width+Mathf.Sin(i*7)*18,55+(float)StoryMopState.Y(i)*300+Mathf.Cos(i*11)*12);
        }
        private void BeginMop()
        {
            mopState.Reset();mopPrevious=false;mopTime=0;
            for(int i=0;i<mopOpacity.Length;i++){mopOpacity[i]=mopState.Visible(i)?1:0;mopDamp[i]=0;}
            mopTool.anchoredPosition=new Vector2(430,210);RefreshMop();
        }
        private void TickMop()
        {
            if(!CanGentle(mopPanel))return;
            float dt=Time.unscaledDeltaTime;mopTime+=dt;
            for(int i=0;i<mopOpacity.Length;i++)
            {
                float target=mopState.Visible(i) || (i>=45 && mopState.Current==StoryMopState.Stage.Notice) ? 1:0;
                if(target==0 && mopOpacity[i]>.7f)mopDamp[i]=1;
                mopOpacity[i]=Mathf.MoveTowards(mopOpacity[i],target,dt*3.2f);
                mopDamp[i]=Mathf.Max(0,mopDamp[i]-dt*.65f);
                float pulse=i>=45 && (mopState.Current==StoryMopState.Stage.Notice || mopState.Current==StoryMopState.Stage.LastSpot) ? .75f+.25f*Mathf.Sin(mopTime*4):1;
                mopWater[i].color=new Color(.30f,.40f,.42f,mopOpacity[i]*.16f+mopDamp[i]*.035f);
                mopGleam[i].color=new Color(.98f,.91f,.70f,(i%7==0 || i>=45 ? 1:0)*(mopOpacity[i]*.42f+mopDamp[i]*.08f)*pulse);
                wetSpots[i].SetActive(mopOpacity[i]+mopDamp[i]>.005f);
            }
            if(Mouse.current==null||!Mouse.current.leftButton.isPressed){mopPrevious=false;return;}
            Vector2 local;
            if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(mopArea,Mouse.current.position.ReadValue(),null,out local)){mopPrevious=false;return;}
            Vector2 point=local+new Vector2(960,540);
            if(point.x<0||point.x>1920||point.y<0||point.y>620||mopFurniture.Covers(point/ new Vector2(1920,1080))){mopPrevious=false;return;}
            mopTool.anchoredPosition=point;mopTool.localScale=Vector3.one*Mathf.Clamp(1.25f-point.y/1080f,.65f,1.2f);
            Vector2 previous=mopPrevious?mopLast:point; bool changed=false;
            for(int i=0;i<mopState.Count;i++)
            {
                if(!mopState.Visible(i))continue;
                Vector2 delta=point-previous;float t=delta.sqrMagnitude<.001f?0:Mathf.Clamp01(Vector2.Dot(MopFloorPoint(i)-previous,delta)/delta.sqrMagnitude);
                if(Vector2.Distance(MopFloorPoint(i),previous+delta*t)<65)
                    changed=mopState.Wipe(StoryMopState.X(i),StoryMopState.Y(i),StoryMopState.X(i),StoryMopState.Y(i))||changed;
            }
            if(changed)RefreshMop();mopLast=point;mopPrevious=true;
        }
        private void RefreshMop()
        {
            bool notice=mopState.Current==StoryMopState.Stage.Notice,done=mopState.Current==StoryMopState.Stage.Complete;
            mopAck.gameObject.SetActive(notice);mopDone.gameObject.SetActive(done);mopAlternative.interactable=!notice&&!done;
            mopFeedback.text=notice?"李晚云：这边还有一点。":done?"水收干净了。把拖把放回去吧。":mopState.Current==StoryMopState.Stage.LastSpot?"凌赫：看到了。\n椅脚旁的水光轻轻闪了一下。":"沿着地上的水光慢慢擦，不用着急。";
        }
    }
}

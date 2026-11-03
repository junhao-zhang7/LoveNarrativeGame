using System;

namespace LoveNarrativeGame.Prototype
{
    public enum StoryInteraction { None, Mop, CardChoice, CooperateMath, FoldNote }
    public enum CardReturnStyle { Unchosen, Direct, Playful }
    public sealed class StoryMopState
    {
        public enum Stage { Cleaning, Notice, LastSpot, Complete }
        private readonly bool[] clean = new bool[49];
        public Stage Current { get; private set; }
        public int Count => clean.Length;
        public static double X(int i) => .14 + (i % 7) * .115 + ((i / 7) % 2) * .018;
        public static double Y(int i) => .14 + (i / 7) * .115;
        public bool IsClean(int i) => clean[i];
        public bool Visible(int i) => !clean[i] && (i < 45 || Current == Stage.LastSpot);
        public int Cleaned { get { int n=0;foreach(bool v in clean)if(v)n++;return n; } }
        // Presentation may place patches on an irregular illustrated puddle instead of the original grid.
        public bool CleanPatch(int i)
        {
            if(i<0||i>=clean.Length||!Visible(i)||Current==Stage.Notice||Current==Stage.Complete)return false;
            clean[i]=true;
            if(Current==Stage.Cleaning&&Cleaned==45)Current=Stage.Notice;
            if(Current==Stage.LastSpot&&Cleaned==49)Current=Stage.Complete;
            return true;
        }
        public bool Wipe(double ax,double ay,double bx,double by)
        {
            if (Current == Stage.Notice || Current == Stage.Complete || double.IsNaN(ax+ay+bx+by) || double.IsInfinity(ax+ay+bx+by)) return false;
            if(ax<0||ax>1||ay<0||ay>1||bx<0||bx>1||by<0||by>1)return false;
            bool changed=false; double dx=bx-ax,dy=by-ay,length=dx*dx+dy*dy;
            for(int i=0;i<clean.Length;i++)
            {
                if(!Visible(i))continue;
                double t=length<1e-12?0:Math.Max(0,Math.Min(1,((X(i)-ax)*dx+(Y(i)-ay)*dy)/length));
                double ex=X(i)-(ax+t*dx),ey=Y(i)-(ay+t*dy);
                if(ex*ex+ey*ey<=.105*.105){clean[i]=true;changed=true;}
            }
            if(Current==Stage.Cleaning && Cleaned==45)Current=Stage.Notice;
            if(Current==Stage.LastSpot && Cleaned==49)Current=Stage.Complete;
            return changed;
        }
        public void CleanNext()
        { for(int i=0;i<clean.Length;i++)if(Visible(i)){Wipe(X(i),Y(i),X(i),Y(i));return;} }
        public bool Acknowledge()
        { if(Current!=Stage.Notice)return false;Current=Stage.LastSpot;return true; }
        public void Reset(){Array.Clear(clean,0,clean.Length);Current=Stage.Cleaning;}
    }
    public sealed class StoryCoopMathState
    {
        public int Step { get; private set; }
        public bool Complete => Step==4;
        public bool Advance(){if(Complete)return false;Step++;return true;}
        public void Reset(){Step=0;}
    }
}

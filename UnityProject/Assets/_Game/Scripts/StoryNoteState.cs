namespace LoveNarrativeGame.Prototype
{
    public sealed class StoryNoteState
    {
        public enum Stage { Open, Folded, Given }
        public Stage Current { get; private set; }
        public bool Fold(){if(Current!=Stage.Open)return false;Current=Stage.Folded;return true;}
        public bool Open(){if(Current!=Stage.Folded)return false;Current=Stage.Open;return true;}
        public bool Give(){if(Current!=Stage.Folded)return false;Current=Stage.Given;return true;}
        public void Reset(){Current=Stage.Open;}
    }
}

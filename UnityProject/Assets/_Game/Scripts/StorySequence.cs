using System;
using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    // Keep the original values so earlier draft assets retain their meaning.
    public enum StoryShot { BackRow = 0, Caught = 1, FrontSeats = 2, TeacherArrives = 3, PackBooks = 4 }
    [Serializable]
    public sealed class StoryBeat
    {
        public string speaker;
        [TextArea(2, 5)] public string text;
        public StoryShot shot;
        [Range(0, 1)] public float focus;
        public bool fade;
    }
    [CreateAssetMenu(menuName = "Story/Linear Sequence")]
    public sealed class StorySequence : ScriptableObject
    {
        public string storyId = "campus-act-01";
        public string chapterTitle = "第一幕 · 最后一排";
        [TextArea] public string sourceNotes;
        public StoryBeat[] beats;
        public bool IsValid()
        {
            if (beats == null || beats.Length == 0) return false;
            foreach (var beat in beats) if (beat == null || string.IsNullOrWhiteSpace(beat.text)) return false;
            return true;
        }
    }
    // Bounded linear progress; rapid repeated events cannot advance beyond the final line.
    public sealed class StoryProgress
    {
        public int Index { get; private set; }
        public int Count { get; }
        public bool Complete { get; private set; }
        public StoryProgress(int count) { if (count < 1) throw new ArgumentOutOfRangeException(nameof(count)); Count = count; }
        public bool Advance()
        {
            if (Complete) return false;
            if (Index == Count - 1) { Complete = true; return false; }
            Index++; return true;
        }
        public void Reset() { Index = 0; Complete = false; }
    }
}

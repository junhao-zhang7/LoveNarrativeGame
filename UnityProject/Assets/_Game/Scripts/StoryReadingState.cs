using System;
using System.Collections.Generic;

namespace LoveNarrativeGame.Prototype
{
    [Serializable]
    public sealed class StoryReadingSave
    {
        public int version = 1;
        public string[] completed = new string[0];
        public string chapterId;
        public int beat;
    }

    // Chapter identity survives additions; completion and the current bookmark are independent.
    public sealed class StoryReadingState
    {
        private readonly string[] ids;
        private readonly int[] counts;
        private readonly HashSet<string> completed = new HashSet<string>();
        public int Chapter { get; private set; }
        public int Beat { get; private set; }
        public bool AllComplete => FirstUnfinished == ids.Length;
        public int FirstUnfinished
        {
            get { int i = 0; while (i < ids.Length && completed.Contains(ids[i])) i++; return i; }
        }
        public StoryReadingState(string[] chapterIds, int[] beatCounts, StoryReadingSave save = null)
        {
            if (chapterIds == null || chapterIds.Length == 0 || beatCounts == null || beatCounts.Length != chapterIds.Length)
                throw new ArgumentException("Invalid chapter list.");
            ids = (string[])chapterIds.Clone(); counts = (int[])beatCounts.Clone();
            var unique = new HashSet<string>();
            for (int i = 0; i < ids.Length; i++)
                if (string.IsNullOrWhiteSpace(ids[i]) || !unique.Add(ids[i]) || counts[i] < 1) throw new ArgumentException("Invalid chapter identity.");
            if (save != null && save.version == 1)
            {
                if (save.completed != null)
                    foreach (string id in save.completed) if (id != null && unique.Contains(id)) completed.Add(id);
                // A missing prerequisite prevents completion records farther down the line from unlocking it.
                int boundary = FirstUnfinished;
                for (int i = boundary; i < ids.Length; i++) completed.Remove(ids[i]);
                int bookmark = Array.IndexOf(ids, save.chapterId);
                Chapter = bookmark >= 0 && IsUnlocked(bookmark) ? bookmark : Math.Min(FirstUnfinished, ids.Length - 1);
                Beat = bookmark == Chapter ? Math.Max(0, Math.Min(save.beat, counts[Chapter] - 1)) : 0;
                if (IsComplete(Chapter) && !AllComplete) { Chapter = FirstUnfinished; Beat = 0; }
            }
        }
        public bool IsUnlocked(int chapter) => chapter >= 0 && chapter < ids.Length && chapter <= FirstUnfinished;
        public bool IsComplete(int chapter) => chapter >= 0 && chapter < ids.Length && completed.Contains(ids[chapter]);
        public bool Enter(int chapter, int beat = 0)
        {
            if (!IsUnlocked(chapter)) return false;
            Chapter = chapter; Beat = Math.Max(0, Math.Min(beat, counts[chapter] - 1)); return true;
        }
        public void Bookmark(int beat) { Beat = Math.Max(0, Math.Min(beat, counts[Chapter] - 1)); }
        public bool Finish()
        {
            if (Beat != counts[Chapter] - 1) return false;
            completed.Add(ids[Chapter]);
            if (!AllComplete) { Chapter = FirstUnfinished; Beat = 0; }
            return true;
        }
        public StoryReadingSave Capture()
        {
            var done = new List<string>(); foreach (string id in ids) if (completed.Contains(id)) done.Add(id);
            return new StoryReadingSave { completed = done.ToArray(), chapterId = ids[Chapter], beat = Beat };
        }
    }
}

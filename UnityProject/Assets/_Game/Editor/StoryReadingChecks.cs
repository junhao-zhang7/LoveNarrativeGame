using System;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class StoryReadingChecks
{
    [MenuItem("Tools/Story Prototype/Check Chapter Reading")]
    public static void Run()
    {
        CheckState();
        var data = new StoryReadingSave { completed = new[] { "a" }, chapterId = "b", beat = 1 };
        var restored = JsonUtility.FromJson<StoryReadingSave>(JsonUtility.ToJson(data));
        Require(restored.chapterId == "b" && restored.beat == 1 && restored.completed[0] == "a", "JSON round trip");
        Debug.Log("STORY_READING_CHECK_PASS: unlock, resume, replay, malformed records, content extension and JSON round trip; no player saves modified");
    }
    public static void CheckState()
    {
        var ids = new[] { "a", "b", "c" }; var counts = new[] { 3, 2, 1 };
        var state = new StoryReadingState(ids, counts);
        Require(state.IsUnlocked(0) && !state.IsUnlocked(1) && !state.IsUnlocked(-1), "new save gate");
        Require(!state.Enter(2) && state.Chapter == 0 && !state.Finish(), "locked entry and premature finish");
        state.Bookmark(1); var reopen = new StoryReadingState(ids, counts, state.Capture());
        Require(reopen.Chapter == 0 && reopen.Beat == 1 && !reopen.IsUnlocked(1), "reopen bookmark without unlock");
        state.Bookmark(2); Require(state.Finish() && state.Chapter == 1 && state.Beat == 0 && state.IsUnlocked(1) && !state.IsUnlocked(2), "complete unlocks exactly one");
        state.Enter(1, 1); state.Finish(); Require(state.Chapter == 2 && state.IsUnlocked(2), "second completion");
        state.Enter(0, 2); state.Finish(); Require(state.IsComplete(1) && state.Chapter == 2 && state.Beat == 0, "replay preserves frontier");
        Require(state.Finish() && state.AllComplete, "last chapter complete");
        var extended = new StoryReadingState(new[] { "a", "b", "c", "d" }, new[] { 3, 2, 1, 2 }, state.Capture());
        Require(extended.Chapter == 3 && extended.IsUnlocked(3) && extended.Beat == 0 && !extended.AllComplete, "new content after old end");
        var invalid = new StoryReadingState(ids, counts, new StoryReadingSave { completed = new[] { "b", "c", "unknown" }, chapterId = "c", beat = 999 });
        Require(!invalid.IsUnlocked(1) && invalid.Chapter == 0 && invalid.Beat == 0, "noncontiguous and locked bookmark rejected");
        invalid = new StoryReadingState(ids, counts, new StoryReadingSave { chapterId = "a", beat = -2 });
        Require(invalid.Beat == 0, "negative bookmark bounded");
        invalid = new StoryReadingState(ids, counts, new StoryReadingSave { chapterId = "a", beat = 999 });
        Require(invalid.Beat == 2 && !invalid.IsUnlocked(1), "changed chapter length bounded");
        invalid = new StoryReadingState(ids, counts, new StoryReadingSave { version = 200, completed = ids, chapterId = "c" });
        Require(invalid.Chapter == 0 && !invalid.IsUnlocked(1), "unsupported save version");
        var other = new StoryReadingState(new[] { "other" }, new[] { 2 }, state.Capture());
        Require(!other.AllComplete && other.Chapter == 0 && other.Beat == 0, "different book IDs isolated");
        var one = new StoryReadingState(new[] { "one" }, new[] { 1 }); Require(one.Finish() && one.AllComplete, "single line chapter");
        var snapshot = extended.Capture(); snapshot.completed[0] = "edited";
        Require(extended.IsComplete(0), "snapshot independent of model");
        bool threw = false;
        try { new StoryReadingState(new[] { "a", "a" }, new[] { 1, 1 }); } catch (ArgumentException) { threw = true; }
        Require(threw, "duplicate chapter identity rejected");
        var flow = new StoryReadingState(ids, counts);
        for (int chapter = 0; chapter < ids.Length; chapter++)
        {
            Require(flow.Enter(chapter), "sequential playback entry");
            flow.Bookmark(counts[chapter] - 1); Require(flow.Finish(), "sequential playback finish");
            var restart = new StoryReadingState(ids, counts, flow.Capture());
            Require(chapter == ids.Length - 1 ? restart.AllComplete : restart.Chapter == chapter + 1 && restart.Beat == 0, "chapter boundary resume");
        }
        Require(flow.Enter(0, 2) && flow.Finish() && flow.Enter(1, 0) && flow.IsComplete(2), "replay continues to adjacent chapter without losing completion");
    }
    private static void Require(bool ok, string message) { if (!ok) throw new Exception("Reading check failed: " + message); }
}

using System;
using System.Linq;
using LoveNarrativeGame.Prototype;
using UnityEditor;
using UnityEngine;

public static class StoryMathChecks
{
    [MenuItem("Tools/Story Prototype/Check Fifth Act Answer")]
    public static void Run()
    {
        CheckAnswers();
        var actual = AssetDatabase.LoadAssetAtPath<StorySequence>(MathHelpStoryDraft.AssetPath);
        if (actual == null || actual.beats.Length != 29) throw new Exception("Fifth-act interactive content was not migrated; author edits preserved.");
        var beats = actual.beats; int gate = Array.FindIndex(beats, b => b.interaction==StoryInteraction.CooperateMath);
        if (gate != 13 || beats.Any(b=>b.requiresMathAnswer) || beats.Count(b => b.interaction==StoryInteraction.CooperateMath) != 1 || beats.Select(b => b.id).Distinct().Count() != beats.Length)
            throw new Exception("Invalid answer gate or stable beat identities.");
        if (beats.Skip(10).Take(gate - 10).Any(b => b.shot != StoryShot.ProblemNotebook || b.speaker != "凌赫（心声）") ||
            beats[gate + 1].shot != StoryShot.ProblemNotebook || beats[gate + 1].text != "你找的是最小值。它问的是最大值，还给了区间。")
            throw new Exception("Maths monologue and explanation order is invalid.");
        if (beats.Take(gate + 1).Any(b => b.text.Contains("520") || b.text.Contains("五百二十")))
            throw new Exception("The answer must not appear before submission.");
        var save = new StoryReadingSave { chapterId = "campus-act-05", beat = gate, beatId = beats[gate].id, contentVersion = 2 };
        var roundtrip = JsonUtility.FromJson<StoryReadingSave>(JsonUtility.ToJson(save));
        if (roundtrip.beatId != beats[gate].id || roundtrip.contentVersion != 2 || roundtrip.beat != gate)
            throw new Exception("Answer bookmark serialization failed.");
        Debug.Log("STORY_MATH_CHECK_PASS: cooperative route without forced input, optional answer validation, story order and bookmark JSON; no player saves changed");
    }
    public static void CheckAnswers()
    {
        foreach (string value in new[] { "520", " ５２０ ", "+520", "\t520\n" })
            if (!StoryMathAnswer.IsCorrect(value)) throw new Exception("Correct maths input rejected.");
        foreach (string value in new[] { null, "", " ", "26", "-38", "494", "519", "521", "520.0", "5e2", "520abc", "5 20", "999999999999999999999" })
            if (StoryMathAnswer.IsCorrect(value)) throw new Exception("Incorrect maths input accepted.");
    }
}

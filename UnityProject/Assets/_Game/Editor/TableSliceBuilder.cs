using System;
using System.IO;
using LoveNarrativeGame.Prototype;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class TableSliceBuilder
{
    public const string ScenePath = "Assets/_Game/Scenes/TableSlice.unity";
    [MenuItem("Tools/Story Prototype/Open Table Slice")]
    public static void Open()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play before opening the table slice."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        CreateIfMissing(); EditorSceneManager.OpenScene(ScenePath);
    }
    [MenuItem("Tools/Story Prototype/Build Table Slice")]
    public static void BuildFromMenu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play before building the table slice."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        var setup = EditorSceneManager.GetSceneManagerSetup();
        try { CreateAndBuild(); }
        finally { EditorSceneManager.RestoreSceneManagerSetup(setup); }
    }
    public static void CreateAndBuild()
    {
        TestState(); CreateIfMissing();
        var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Builds/TableSlice/TableSlice.exe"));
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { ScenePath }, locationPathName = output,
            target = BuildTarget.StandaloneWindows64, options = BuildOptions.Development
        });
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Table slice build failed: " + report.summary.result);
        Debug.Log("TABLE_SLICE_BUILD_PASS: " + output);
    }
    private static void CreateIfMissing()
    {
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null) return;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Game/Fonts/NotoSansCJKsc-Regular SDF.asset");
        if (font == null) throw new Exception("Existing Chinese TMP font was not found.");
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camera = new GameObject("Camera", typeof(Camera)).GetComponent<Camera>();
        camera.tag = "MainCamera"; camera.transform.position = new Vector3(0, 0, -10); camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.2f, .24f, .23f);
        var presenter = new GameObject("Table Slice Prototype").AddComponent<TableSlicePresenter>(); presenter.SetFont(font);
        if (!EditorSceneManager.SaveScene(scene, ScenePath)) throw new Exception("Could not save independent prototype scene.");
        AssetDatabase.SaveAssets(); Debug.Log("TABLE_SLICE_SCENE_CREATED");
    }
    [MenuItem("Tools/Story Prototype/Check Table Slice State")]
    public static void TestState()
    {
        int checks = 0;
        Action<bool, string> check = (value, message) => { checks++; if (!value) throw new Exception("Table slice state: " + message); };
        foreach (int layout in new[] { 1, 2 })
        {
            var s = new TableSliceState();
            check(!s.Share(), "cannot share before question");
            check(!s.TryAnswer(null) && !s.TryAnswer("hello") && !s.TryAnswer("519"), "invalid answers");
            check(s.Current == TableSliceState.Step.Question, "bad answers do not advance");
            s.NextHint(); s.NextHint(); s.NextHint(); check(s.Hint == 2, "hint capped");
            check(layout == 1 ? s.TryAnswer(" ５２０ ") : s.FinishQuestion(), "answer or explanation");
            check(!s.TryAnswer("520") && !s.FinishQuestion(), "question only completes once");
            check(!s.Clear(0) && !s.Clear(3), "invalid layouts rejected");
            check(s.Clear(layout) && s.Layout == layout, "both valid layouts");
            check(s.Share() && !s.Share() && !s.Clear(1), "exchange locks repeated actions");
            check(s.ReturnPage() && !s.ReturnPage(), "single math return");
            check(s.StartEnglish() && !s.StartEnglish(), "single transition");
            check(s.Share() && s.ReturnPage(), "english exchange");
            check(s.Current == TableSliceState.Step.Complete && !s.Share(), "complete is terminal");
        }
        check(new TableSliceState().Current == TableSliceState.Step.Question, "fresh replay state");
        var results = Path.GetFullPath(Path.Combine(Application.dataPath, "../../TestResults/TableSlice")); Directory.CreateDirectory(results);
        File.WriteAllText(Path.Combine(results, "state-checks.txt"), "PASS: " + checks + " assertions; both layouts; answer/explanation; duplicate events; fresh replay.");
        Debug.Log("TABLE_SLICE_STATE_PASS: " + checks + " assertions");
    }
}

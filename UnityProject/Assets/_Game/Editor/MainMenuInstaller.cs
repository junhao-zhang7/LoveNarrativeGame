using System;
using System.IO;
using System.Linq;
using LoveNarrativeGame.Prototype;
using TMPro;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class MainMenuInstaller
{
    private const string ThemePath = "Assets/_Game/Resources/Menu/MainMenuTheme.asset";
    private const string MenuPath = "Assets/_Game/Scenes/MainMenu.unity";
    static MainMenuInstaller()
    {
        EditorApplication.delayCall += Install;
        EditorApplication.playModeStateChanged += state => { if (state == PlayModeStateChange.EnteredEditMode) Install(); };
    }

    [MenuItem("Tools/Story Prototype/Install Main Menu")]
    public static void Install()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Game/Fonts/NotoSansCJKsc-Regular SDF.asset");
        PreparePicture("Assets/_Game/Resources/Menu/MainMenuBackground.png");
        foreach (var picture in new[] { "Whisper", "TeacherArrives", "Caught", "PackBooks" })
            PreparePicture("Assets/_Game/Resources/Story/RearRow/" + picture + ".png");
        var background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Menu/MainMenuBackground.png");
        if (font == null || background == null) { Debug.LogWarning("Menu art/font not imported yet; use Tools/Story Prototype/Install Main Menu after import."); return; }
        var theme = AssetDatabase.LoadAssetAtPath<MainMenuTheme>(ThemePath);
        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<MainMenuTheme>(); theme.chineseFont = font; theme.background = background;
            AssetDatabase.CreateAsset(theme, ThemePath); AssetDatabase.SaveAssets();
        }
        var story = AssetDatabase.LoadAssetAtPath<StorySequence>(RearRowStoryDraft.AssetPath);
        if (story == null && AssetDatabase.IsValidFolder("Assets/_Game/Resources/Story"))
        {
            story = ScriptableObject.CreateInstance<StorySequence>(); story.beats = RearRowStoryDraft.Create();
            story.sourceNotes = "来源：MAIN_STORY_V0.3.md MS-01。保留已认可对白，舞台动作转为少量旁白。只演出后排闲聊至下课收书，不含讲台旁初次交谈；无配音、音乐或玩法分支。独立资源，不覆盖 Opening 草稿。";
            AssetDatabase.CreateAsset(story, RearRowStoryDraft.AssetPath); AssetDatabase.SaveAssets();
        }
        // Preserve user choices on subsequent imports. Add the existing prototype once.
        var scenes = EditorBuildSettings.scenes.ToList();
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TableSliceBuilder.ScenePath) != null && !scenes.Any(s => s.path == TableSliceBuilder.ScenePath))
        { scenes.Add(new EditorBuildSettingsScene(TableSliceBuilder.ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
        Debug.Log("ILLUSTRATED_MENU_INSTALLED");
    }

    private static void PreparePicture(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        if (importer.textureType == TextureImporterType.Default && !importer.mipmapEnabled &&
            importer.textureCompression == TextureImporterCompression.Uncompressed && importer.maxTextureSize >= 2048) return;
        // RawImage uses the texture itself; old sprite rectangles must not crop replacement art.
        importer.textureType = TextureImporterType.Default; importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 2048;
        importer.SaveAndReimport();
    }

    [MenuItem("Tools/Story Prototype/Open Main Menu")]
    public static void Open()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play first."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Install(); EditorSceneManager.OpenScene(MenuPath);
    }

    [MenuItem("Tools/Story Prototype/Build Illustrated Menu")]
    public static void BuildFromMenu()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) { Debug.LogWarning("Stop Play first."); return; }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        Build();
    }
    public static void Build()
    {
        Install(); TestPreferences(); TestStory();
        if (AssetDatabase.LoadAssetAtPath<MainMenuTheme>(ThemePath) == null) throw new Exception("Menu theme is missing.");
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TableSliceBuilder.ScenePath) == null) throw new Exception("Open the Table Slice once to create its scene before building.");
        var output = Path.GetFullPath(Path.Combine(Application.dataPath, "../../Builds/MenuDemo/MenuDemo.exe"));
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { MenuPath, TableSliceBuilder.ScenePath, "Assets/Scenes/SampleScene.unity" },
            target = BuildTarget.StandaloneWindows64, locationPathName = output, options = BuildOptions.Development
        });
        if (report.summary.result != BuildResult.Succeeded) throw new Exception("Menu build failed: " + report.summary.result);
        Debug.Log("ILLUSTRATED_MENU_BUILD_PASS: " + output);
    }

    [MenuItem("Tools/Story Prototype/Check Menu Preferences")]
    public static void TestPreferences()
    {
        // Pure boundary checks: do not overwrite the user's saved preferences.
        if (MenuPreferences.NormalizeVolume(-1) != 0 || MenuPreferences.NormalizeVolume(2) != 1 ||
            MenuPreferences.NormalizeVolume(.4f) != .4f || MenuPreferences.NormalizeVolume(float.NaN) != .8f ||
            MenuPreferences.NormalizeVolume(float.PositiveInfinity) != .8f) throw new Exception("Invalid menu volume normalization.");
        if (MenuPreferences.NormalizeLanguage(-1) != MenuLanguage.SimplifiedChinese || MenuPreferences.NormalizeLanguage(4) != MenuLanguage.SimplifiedChinese)
            throw new Exception("Invalid language fallback.");
        for (int i = 0; i < 4; i++) if ((int)MenuPreferences.NormalizeLanguage(i) != i) throw new Exception("Invalid supported language.");
        if (!MenuCopy.HasCompleteTranslations()) throw new Exception("Incomplete menu translations.");
        Debug.Log("MENU_PREFERENCES_CHECK_PASS: 12 assertions; 4 complete menu languages");
    }

    [MenuItem("Tools/Story Prototype/Check First Act")]
    public static void TestStory()
    {
        var story = AssetDatabase.LoadAssetAtPath<StorySequence>(RearRowStoryDraft.AssetPath);
        if (story == null || !story.IsValid()) throw new Exception("Rear-row story is missing or contains empty lines.");
        foreach (var key in new[] { "Whisper", "TeacherArrives", "Caught", "PackBooks" })
            if (AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/RearRow/" + key + ".png") == null) throw new Exception("Missing story picture: " + key);
        var expected = new[] { StoryShot.BackRow, StoryShot.TeacherArrives, StoryShot.Caught, StoryShot.PackBooks };
        int shotIndex = 0;
        foreach (var beat in story.beats)
        {
            if (beat.shot == expected[shotIndex]) continue;
            if (shotIndex == expected.Length - 1 || beat.shot != expected[++shotIndex]) throw new Exception("Unexpected rear-row shot order.");
        }
        if (shotIndex != expected.Length - 1) throw new Exception("Rear-row story does not use all four pictures.");
        var cursor = new StoryProgress(story.beats.Length);
        if (cursor.Index != 0 || cursor.Complete) throw new Exception("Opening state is invalid.");
        for (int i = 1; i < cursor.Count; i++) if (!cursor.Advance() || cursor.Index != i) throw new Exception("Story order changed.");
        if (cursor.Advance() || !cursor.Complete || cursor.Index != cursor.Count - 1) throw new Exception("Story end is invalid.");
        for (int i = 0; i < 20; i++) if (cursor.Advance() || cursor.Index != cursor.Count - 1) throw new Exception("Repeated end event overflowed.");
        cursor.Reset(); if (cursor.Index != 0 || cursor.Complete) throw new Exception("Replay failed.");
        var single = new StoryProgress(1); if (single.Advance() || !single.Complete || single.Index != 0) throw new Exception("Single-line end failed.");
        Debug.Log("FIRST_ACT_CHECK_PASS: " + story.beats.Length + " beats; ordered progression; repeated end; replay; single-line sequence; 4 pictures");
    }
}

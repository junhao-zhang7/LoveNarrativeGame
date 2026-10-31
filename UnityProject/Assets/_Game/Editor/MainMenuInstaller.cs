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
        var background = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Menu/MainMenuBackground.png");
        if (font == null || background == null) { Debug.LogWarning("Menu art/font not imported yet; use Tools/Story Prototype/Install Main Menu after import."); return; }
        var theme = AssetDatabase.LoadAssetAtPath<MainMenuTheme>(ThemePath);
        if (theme == null)
        {
            theme = ScriptableObject.CreateInstance<MainMenuTheme>(); theme.chineseFont = font; theme.background = background;
            AssetDatabase.CreateAsset(theme, ThemePath); AssetDatabase.SaveAssets();
        }
        // Preserve user choices on subsequent imports. Add the existing prototype once.
        var scenes = EditorBuildSettings.scenes.ToList();
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TableSliceBuilder.ScenePath) != null && !scenes.Any(s => s.path == TableSliceBuilder.ScenePath))
        { scenes.Add(new EditorBuildSettingsScene(TableSliceBuilder.ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
        Debug.Log("ILLUSTRATED_MENU_INSTALLED");
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
        Install(); TestPreferences();
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
        Debug.Log("MENU_PREFERENCES_CHECK_PASS: 5 assertions");
    }
}

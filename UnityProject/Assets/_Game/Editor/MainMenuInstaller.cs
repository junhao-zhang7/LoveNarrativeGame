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
        PrepareMusic();
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Game/Fonts/NotoSansCJKsc-Regular SDF.asset");
        PreparePicture("Assets/_Game/Resources/Menu/MainMenuBackground.png");
        foreach (var picture in new[] { "Whisper", "TeacherArrives", "Caught", "PackBooks" })
            PreparePicture("Assets/_Game/Resources/Story/RearRow/" + picture + ".png");
        foreach (var picture in new[] { "NewSeat", "TenMinutes", "Schoolbag", "Goodbye" })
            PreparePicture("Assets/_Game/Resources/Story/FrontSeats/" + picture + ".png");
        foreach (var picture in WaterCupStoryDraft.Pictures)
            PreparePicture("Assets/_Game/Resources/Story/WaterCup/" + picture + ".png");
        foreach (var picture in PaperCardStoryDraft.Pictures)
            PreparePicture("Assets/_Game/Resources/Story/PaperCard/" + picture + ".png");
        PreparePicture("Assets/_Game/Resources/Story/PaperCard/DirectCardReturn.png");
        foreach (var picture in MathHelpStoryDraft.Pictures)
            PreparePicture("Assets/_Game/Resources/Story/MathHelp/" + picture + ".png");
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
        RearRowStoryDraft.UpgradeExisting();
        var second = AssetDatabase.LoadAssetAtPath<StorySequence>(FrontSeatsStoryDraft.AssetPath);
        if (second == null)
        {
            second = ScriptableObject.CreateInstance<StorySequence>(); second.storyId = "campus-act-02";
            second.chapterTitle = "第二幕 · 再等十分钟"; second.beats = FrontSeatsStoryDraft.Create();
            second.sourceNotes = "MAIN_STORY_V0.3 MS-02：原对白不变，舞台动作分为旁白。图片由作者提供，无新增玩法或音频。";
            AssetDatabase.CreateAsset(second, FrontSeatsStoryDraft.AssetPath);
        }
        const string catalog = "Assets/_Game/Resources/Story/Catalog";
        if (!AssetDatabase.IsValidFolder(catalog)) AssetDatabase.CreateFolder("Assets/_Game/Resources/Story", "Catalog");
        if (AssetDatabase.LoadAssetAtPath<StoryBook>(catalog + "/BesideYou.asset") == null)
        {
            var book = ScriptableObject.CreateInstance<StoryBook>(); book.bookId = "campus-beside-you";
            book.title = "旁边的位置"; book.subtitle = "凌赫与李晚云的故事";
            book.description = "从一次换座开始，\n走过靠近、迁就，以及那些没说出口的话。";
            book.cover = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/FrontSeats/TenMinutes.png");
            book.chapters = new[] { story, second };
            book.pictures = new[] {
                Picture(StoryShot.BackRow, "RearRow/Whisper"), Picture(StoryShot.TeacherArrives, "RearRow/TeacherArrives"),
                Picture(StoryShot.Caught, "RearRow/Caught"), Picture(StoryShot.PackBooks, "RearRow/PackBooks"),
                Picture(StoryShot.NewSeat, "FrontSeats/NewSeat"), Picture(StoryShot.TenMinutes, "FrontSeats/TenMinutes"),
                Picture(StoryShot.Schoolbag, "FrontSeats/Schoolbag"), Picture(StoryShot.Goodbye, "FrontSeats/Goodbye")
            };
            AssetDatabase.CreateAsset(book, catalog + "/BesideYou.asset");
        }
        WaterCupStoryDraft.Install(AssetDatabase.LoadAssetAtPath<StoryBook>(catalog + "/BesideYou.asset"));
        PaperCardStoryDraft.Install(AssetDatabase.LoadAssetAtPath<StoryBook>(catalog + "/BesideYou.asset"));
        MathHelpStoryDraft.Install(AssetDatabase.LoadAssetAtPath<StoryBook>(catalog + "/BesideYou.asset"));
        AssetDatabase.SaveAssets();
        // Preserve user choices on subsequent imports. Add the existing prototype once.
        var scenes = EditorBuildSettings.scenes.ToList();
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(TableSliceBuilder.ScenePath) != null && !scenes.Any(s => s.path == TableSliceBuilder.ScenePath))
        { scenes.Add(new EditorBuildSettingsScene(TableSliceBuilder.ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
        Debug.Log("ILLUSTRATED_MENU_INSTALLED");
    }

    private static StoryPicture Picture(StoryShot shot, string key)
    { return new StoryPicture { shot = shot, texture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/_Game/Resources/Story/" + key + ".png") }; }

    private static void PrepareMusic()
    {
        var importer = AssetImporter.GetAtPath("Assets/_Game/Resources/Audio/BackgroundMusic.mp3") as AudioImporter;
        if (importer == null) return;
        var settings = importer.defaultSampleSettings;
        if (settings.loadType == AudioClipLoadType.Streaming && settings.compressionFormat == AudioCompressionFormat.Vorbis &&
            settings.quality == 1 && settings.sampleRateSetting == AudioSampleRateSetting.PreserveSampleRate) return;
        settings.loadType = AudioClipLoadType.Streaming;
        settings.compressionFormat = AudioCompressionFormat.Vorbis;
        settings.quality = 1;
        settings.sampleRateSetting = AudioSampleRateSetting.PreserveSampleRate;
        importer.defaultSampleSettings = settings;
        importer.SaveAndReimport();
    }

    private static void PreparePicture(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null) return;
        if (importer.textureType == TextureImporterType.Default && !importer.mipmapEnabled &&
            importer.textureCompression == TextureImporterCompression.Uncompressed && importer.maxTextureSize >= 4096) return;
        // RawImage uses the texture itself; old sprite rectangles must not crop replacement art.
        importer.textureType = TextureImporterType.Default; importer.mipmapEnabled = false;
        importer.textureCompression = TextureImporterCompression.Uncompressed; importer.maxTextureSize = 4096;
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
        Install(); TestPreferences(); TestStory(); CheckBook(); StoryReadingChecks.Run(); StoryMathChecks.Run(); StoryRouteChecks.Run(); StoryGentleChecks.Run();
        var music = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/_Game/Resources/Audio/BackgroundMusic.mp3");
        if (music == null || music.length <= 0) throw new Exception("Background music failed to import.");
        Debug.Log("BACKGROUND_MUSIC_IMPORTED: " + music.length.ToString("F2") + " seconds; looping player enabled");
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

    public static void CheckBook()
    {
        var book = AssetDatabase.LoadAssetAtPath<StoryBook>("Assets/_Game/Resources/Story/Catalog/BesideYou.asset");
        if (book == null || book.cover == null || book.chapters.Length < 5) throw new Exception("Story catalog entry is incomplete.");
        foreach (var chapter in book.chapters) if (chapter == null || !chapter.IsValid()) throw new Exception("Missing chapter.");
        foreach (var picture in book.pictures) if (picture.texture == null) throw new Exception("Missing catalog picture.");
        var expected = new[] { StoryShot.NewSeat, StoryShot.TenMinutes, StoryShot.Schoolbag, StoryShot.Goodbye };
        var actual = book.chapters[1].beats.Select(b => b.shot).Distinct().ToArray();
        if (!actual.SequenceEqual(expected)) throw new Exception("Second-act shot order is invalid.");
        var third = book.chapters.SingleOrDefault(c => c.storyId == "campus-act-03");
        if (third == null || !third.beats.Select(b => b.shot).Distinct().SequenceEqual(WaterCupStoryDraft.Shots))
            throw new Exception("Third-act picture order is invalid.");
        foreach (var shot in WaterCupStoryDraft.Shots)
            if (!book.pictures.Any(p => p.shot == shot && p.texture != null)) throw new Exception("Missing third-act picture: " + shot);
        var fourth = book.chapters.SingleOrDefault(c => c.storyId == "campus-act-04");
        if (fourth == null || !fourth.beats.Select(b => b.shot).Distinct().SequenceEqual(PaperCardStoryDraft.Shots))
            throw new Exception("Fourth-act picture order is invalid.");
        foreach (var shot in PaperCardStoryDraft.Shots)
            if (!book.pictures.Any(p => p.shot == shot && p.texture != null)) throw new Exception("Missing fourth-act picture: " + shot);
        var fifth = book.chapters.SingleOrDefault(c => c.storyId == "campus-act-05");
        if (fifth == null || !fifth.beats.Select(b => b.shot).Distinct().SequenceEqual(MathHelpStoryDraft.Shots))
            throw new Exception("Fifth-act picture order is invalid.");
        foreach (var shot in MathHelpStoryDraft.Shots)
            if (!book.pictures.Any(p => p.shot == shot && p.texture != null)) throw new Exception("Missing fifth-act picture: " + shot);
        Debug.Log("STORY_BOOK_CHECK_PASS: " + book.chapters.Length + " chapters; " + book.pictures.Length + " pictures; " + book.chapters.Sum(c => c.beats.Length) + " beats");
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

using System.Collections.Generic;

namespace LoveNarrativeGame.Prototype
{
    public static class MenuCopy
    {
        public static readonly string[] LanguageNames = { "简体中文", "繁體中文", "English", "日本語" };
        private static readonly Dictionary<string, string[]> Entries = new Dictionary<string, string[]>
        {
            { "chooseStory", new[] { "选择故事", "選擇故事", "Choose a story", "物語を選ぶ" } },
            { "campusStory", new[] { "校园故事（暂名）", "校園故事（暫名）", "A School Story (working title)", "学園物語（仮題）" } },
            { "actOne", new[] { "第一幕 · 最后一排", "第一幕 · 最後一排", "Act I · The back row", "第一幕 · 最後列" } },
            { "storyHook", new[] { "一句关于午饭的悄悄话，换了一个座位。", "一句關於午飯的悄悄話，換了一個座位。", "A whisper about lunch. A different seat.", "昼ごはんの話から、席が変わった。" } },
            { "enterStory", new[] { "进入故事 →", "進入故事 →", "Enter story →", "物語を読む →" } },
            { "storyList", new[] { "返回故事选择", "返回故事選擇", "Story selection", "物語の選択へ" } },
            { "edition", new[] { "校园叙事 · 开发中", "校園敘事 · 開發中", "A SCHOOL STORY · IN DEVELOPMENT", "学園物語 · 開発中" } },
            { "title", new[] { "未定名", "未定名", "Untitled", "タイトル未定" } },
            { "tagline", new[] { "从一次换座开始的故事", "從一次換座開始的故事", "It began with a change of seats.", "席替えから始まる物語。" } },
            { "start", new[] { "开始游戏", "開始遊戲", "Start game", "はじめる" } },
            { "settings", new[] { "设置", "設定", "Settings", "設定" } },
            { "quit", new[] { "退出游戏", "離開遊戲", "Quit game", "ゲームを終了" } },
            { "demo", new[] { "数学讲题与英语互助 · 互动小样", "數學講題與英語互助 · 互動試作", "Math & English · Interactive prototype", "数学と英語の教え合い · 体験版" } },
            { "storyNote", new[] { "当前剧情文本：简体中文", "目前劇情文字：簡體中文", "Prototype story text: Simplified Chinese", "体験版の物語テキスト：中国語（簡体字）" } },
            { "music", new[] { "音乐", "音樂", "Music", "音楽" } },
            { "sound", new[] { "音效", "音效", "Sound effects", "効果音" } },
            { "resolution", new[] { "分辨率", "解析度", "Resolution", "解像度" } },
            { "fullscreen", new[] { "全屏", "全螢幕", "Fullscreen", "フルスクリーン" } },
            { "language", new[] { "语言", "語言", "Language", "言語" } },
            { "saved", new[] { "更改自动保存", "變更自動儲存", "Changes are saved automatically", "変更は自動的に保存されます" } },
            { "reset", new[] { "恢复默认", "還原預設", "Reset defaults", "初期設定に戻す" } },
            { "back", new[] { "返回", "返回", "Back", "戻る" } },
            { "confirm", new[] { "确定离开教室吗？", "確定離開教室嗎？", "Leave the classroom?", "教室を出ますか？" } },
            { "cancel", new[] { "再待一会儿", "再待一會兒", "Stay a little longer", "もう少しいる" } },
            { "exit", new[] { "退出", "離開", "Quit", "終了する" } },
            { "loading", new[] { "正在进入教室……", "正在進入教室……", "Entering the classroom…", "教室へ……" } },
            { "unavailable", new[] { "暂时无法进入，请检查场景。", "暫時無法進入，請檢查場景。", "The scene is currently unavailable.", "シーンを開けません。" } },
            { "editorExit", new[] { "编辑器中请停止 Play；独立程序可以正常退出。", "編輯器中請停止 Play；獨立程式可以正常離開。", "Stop Play in the Editor to exit this test.", "エディターでは Play を停止してください。" } }
        };
        public static string Get(string key) => Get(key, MenuPreferences.Language);
        public static string Get(string key, MenuLanguage language) => Entries[key][(int)MenuPreferences.NormalizeLanguage((int)language)];
        public static bool HasCompleteTranslations()
        {
            foreach (var entry in Entries.Values)
            { if (entry.Length != 4) return false; foreach (var value in entry) if (string.IsNullOrWhiteSpace(value)) return false; }
            return true;
        }
    }
}

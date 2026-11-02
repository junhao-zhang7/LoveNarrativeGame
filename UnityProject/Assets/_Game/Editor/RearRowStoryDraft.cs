using LoveNarrativeGame.Prototype;

// Creates a separate sequence; existing Opening draft and author edits are preserved.
public static class RearRowStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/RearRow.asset";
    private static StoryBeat[] CreateLegacy()
    {
        return new[] {
            B("", "高一七班。\n午前的最后一节数学课。"),
            B("数学老师", "奇变偶不变，符号看象限。先看这个角——"),
            B("", "同桌压低声音，用笔帽轻轻碰了碰凌赫的胳膊。"),
            B("同桌", "中午吃什么？"),
            B("凌赫", "照烧鸡排饭。"),
            B("同桌", "又是这个？"),
            B("凌赫", "吃一辈子都吃不腻。"),
            B("同桌", "那一下课就走。昨天到我们就剩汤了。"),
            B("凌赫", "我发现一条小道，从后面绕——", StoryShot.TeacherArrives),
            B("", "他的手指刚划到纸边，身后传来声音。", StoryShot.TeacherArrives),
            B("班主任", "绕哪儿去？", StoryShot.Caught),
            B("", "两人的笔同时落回题目上。", StoryShot.Caught),
            B("班主任", "凌赫，下课搬到讲台旁边。先换好座位，再去吃饭。", StoryShot.Caught),
            B("", "下课铃响了。椅子响成一片。\n同桌拿着饭卡站起来，看看他。", StoryShot.PackBooks, true),
            B("同桌", "我给你看看还有没有。", StoryShot.PackBooks),
            B("凌赫", "帮我守住最后一份。", StoryShot.PackBooks),
            B("同桌", "这我可不敢答应。", StoryShot.PackBooks),
            B("", "凌赫抱起书，最上面的练习册往外滑了一截。\n他用下巴抵住，向讲台旁边走去。", StoryShot.PackBooks)
        };
    }
    public static StoryBeat[] Create()
    {
        var old = CreateLegacy(); var beats = new System.Collections.Generic.List<StoryBeat>();
        for (int i = 0; i < 8; i++) beats.Add(old[i]);
        beats.Add(B("凌赫", "我发现一条小道。"));
        var gate = B("", "凌赫把练习本往中间挪了挪，指尖停在纸上。"); gate.requiresRoute = true; beats.Add(gate);
        beats.Add(B("凌赫", "从后面绕——", StoryShot.TeacherArrives));
        for (int i = 9; i < old.Length; i++) beats.Add(old[i]);
        for (int i = 0; i < beats.Count; i++) beats[i].id = "rear-v2-" + i.ToString("D2");
        return beats.ToArray();
    }
    public static void UpgradeExisting()
    {
        var sequence = UnityEditor.AssetDatabase.LoadAssetAtPath<StorySequence>(AssetPath);
        if (sequence == null) return;
        var old = CreateLegacy(); var actual = sequence.beats;
        if (actual == null || actual.Length != old.Length) return;
        for (int i = 0; i < old.Length; i++)
            if (actual[i] == null || actual[i].text != old[i].text || actual[i].speaker != old[i].speaker || actual[i].shot != old[i].shot || actual[i].fade != old[i].fade || actual[i].focus != old[i].focus || actual[i].requiresRoute || actual[i].requiresMathAnswer) return;
        sequence.beats = Create();
        sequence.sourceNotes = "2026-11-02作者第一幕互动设计：小道话句后插入练习本校园示意图，最短路线完成后接从后面绕及老师打断。校园布局/距离和重试反馈为可替换原创补充。";
        UnityEditor.EditorUtility.SetDirty(sequence);
    }
    private static StoryBeat B(string speaker, string text, StoryShot shot = StoryShot.BackRow, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }
}

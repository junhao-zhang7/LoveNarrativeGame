using LoveNarrativeGame.Prototype;

// Creates a separate sequence; existing Opening draft and author edits are preserved.
public static class RearRowStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/RearRow.asset";
    public static StoryBeat[] Create()
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
    private static StoryBeat B(string speaker, string text, StoryShot shot = StoryShot.BackRow, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }
}

using LoveNarrativeGame.Prototype;

public static class FrontSeatsStoryDraft
{
    public const string AssetPath = "Assets/_Game/Resources/Story/FrontSeatsSequence.asset";
    public static StoryBeat[] Create()
    {
        return new[] {
            B("", "第二幕 · 再等十分钟\n凌赫把书放到讲台旁的新座位上。", StoryShot.NewSeat, true),
            B("班主任", "在这儿给我老实点。有不会的英语问晚云，别一天到晚只会说话。", StoryShot.NewSeat),
            B("凌赫", "知道了。", StoryShot.NewSeat),
            B("班主任", "晚云，数学有问题也可以问他。你们互相帮一帮。", StoryShot.NewSeat),
            B("李晚云", "嗯。", StoryShot.NewSeat),
            B("", "班主任离开了。凌赫放好最后一本书，李晚云低头继续看。", StoryShot.TenMinutes),
            B("凌赫", "你不去吃饭？", StoryShot.TenMinutes),
            B("李晚云", "现在人太多了。我等十分钟。", StoryShot.TenMinutes),
            B("凌赫", "那鸡排就没了。", StoryShot.TenMinutes),
            B("李晚云", "别的也能吃。", StoryShot.TenMinutes),
            B("凌赫", "……也是。", StoryShot.TenMinutes),
            B("", "他习惯性地拎起空了一半的书包，背到一半，才停下来。", StoryShot.Schoolbag),
            B("", "只是去食堂，已经不需要背书包了。\n李晚云看见了，嘴角动了一下，没有笑出声。", StoryShot.Schoolbag),
            B("", "他把书包放回椅边，起身看了她一眼。", StoryShot.Goodbye),
            B("凌赫", "那我先去了。", StoryShot.Goodbye),
            B("李晚云", "嗯。", StoryShot.Goodbye)
        };
    }
    private static StoryBeat B(string speaker, string text, StoryShot shot, bool fade = false)
    { return new StoryBeat { speaker = speaker, text = text, shot = shot, focus = .5f, fade = fade }; }
}

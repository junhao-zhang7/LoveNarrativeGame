using System;
using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    [Serializable]
    public sealed class StoryPicture
    {
        public StoryShot shot;
        public Texture2D texture;
    }

    // A catalog entry represents a whole story; chapters belong inside that entry.
    [CreateAssetMenu(menuName = "Story/Story Book")]
    public sealed class StoryBook : ScriptableObject
    {
        public string bookId;
        public string title;
        public string subtitle;
        [TextArea] public string description;
        public Texture2D cover;
        public StorySequence[] chapters;
        public StoryPicture[] pictures;
        [TextArea] public string endingSummary;
        public string nextPreview;
    }
}

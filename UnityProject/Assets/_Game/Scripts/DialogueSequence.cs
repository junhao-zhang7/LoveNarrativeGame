using UnityEngine;

namespace LoveNarrativeGame.Prototype
{
    [CreateAssetMenu(fileName = "PrototypeDialogue", menuName = "Prototype/Dialogue Sequence")]
    public sealed class DialogueSequence : ScriptableObject
    {
        // Technical placeholder text; this is not the game's story.
        [SerializeField, TextArea(2, 5)] private string[] lines =
        {
            "[Prototype 1/3] This is the first line.",
            "[Prototype 2/3] Next shows the following line.",
            "[Prototype 3/3] One more click ends this dialogue."
        };

        public int LineCount => lines == null ? 0 : lines.Length;

        public string GetLine(int index)
        {
            return lines[index];
        }
    }
}

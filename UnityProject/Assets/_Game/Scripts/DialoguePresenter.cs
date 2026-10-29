using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    [DisallowMultipleComponent]
    public sealed class DialoguePresenter : MonoBehaviour
    {
        [SerializeField] private DialogueSequence dialogue;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Button nextButton;

        private int currentLineIndex;
        private bool isReady;
        private bool isFinished;

        private void Start()
        {
            if (nextButton != null)
            {
                nextButton.interactable = false;
            }

            if (dialogue == null || dialogueText == null || nextButton == null)
            {
                Debug.LogError("DialoguePresenter needs Dialogue, Dialogue Text, and Next Button assigned.", this);
                enabled = false;
                return;
            }

            if (dialogue.LineCount == 0)
            {
                Debug.LogError("DialogueSequence needs at least one line.", this);
                enabled = false;
                return;
            }

            currentLineIndex = 0;
            isFinished = false;
            isReady = true;
            dialogueText.text = dialogue.GetLine(currentLineIndex);
            nextButton.interactable = true;
        }

        // Connect the Next button's single On Click event to this method.
        public void NextLine()
        {
            if (!isReady || isFinished)
            {
                return;
            }

            // Check the boundary before advancing or reading the array.
            if (currentLineIndex >= dialogue.LineCount - 1)
            {
                isFinished = true;
                dialogueText.text = "[Prototype] End of dialogue.";
                nextButton.interactable = false;
                return;
            }

            currentLineIndex++;
            dialogueText.text = dialogue.GetLine(currentLineIndex);
        }
    }
}

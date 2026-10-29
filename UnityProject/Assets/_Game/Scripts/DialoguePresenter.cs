using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class DialoguePresenter : MonoBehaviour
    {
        [SerializeField] private DialogueSequence dialogue;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Button nextButton;

        private int currentLineIndex;
        private CanvasGroup panelGroup;
        private bool isReady;
        private bool hasStarted;
        private bool isFinished;

        private void Awake()
        {
            panelGroup = GetComponent<CanvasGroup>();
            if (panelGroup == null)
            {
                Debug.LogError("Add a Canvas Group to the object with DialoguePresenter.", this);
                enabled = false;
                return;
            }

            SetPanelVisible(false);
        }

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

            isReady = true;
        }

        // Connect the square's On Clicked event to this method.
        public void BeginDialogue()
        {
            // One conversation per scene visit; repeated clicks never reset it.
            if (!isReady || hasStarted)
            {
                return;
            }

            hasStarted = true;
            currentLineIndex = 0;
            isFinished = false;
            dialogueText.text = dialogue.GetLine(currentLineIndex);
            nextButton.interactable = true;
            SetPanelVisible(true);
        }

        // Connect the Next button's single On Click event to this method.
        public void NextLine()
        {
            if (!isReady || !hasStarted || isFinished)
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

        private void SetPanelVisible(bool visible)
        {
            panelGroup.alpha = visible ? 1f : 0f;
            panelGroup.interactable = visible;
            panelGroup.blocksRaycasts = visible;
        }
    }
}

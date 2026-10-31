using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LoveNarrativeGame.Prototype
{
    public sealed class MenuHoverInk : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        public RectTransform stroke;
        public TMP_Text label;
        public Color resting, active;
        private bool hovered, selected;
        private float amount;
        private Button button;
        private void Awake() { button = GetComponent<Button>(); }
        public void OnPointerEnter(PointerEventData _) { hovered = true; }
        public void OnPointerExit(PointerEventData _) { hovered = false; }
        public void OnSelect(BaseEventData _) { selected = true; }
        public void OnDeselect(BaseEventData _) { selected = false; }
        private void Update()
        {
            bool on = (hovered || selected) && button.interactable;
            amount = Mathf.MoveTowards(amount, on ? 1 : 0, Time.unscaledDeltaTime * 5);
            if (stroke != null) stroke.localScale = new Vector3(Mathf.Max(.001f, amount), 1, 1);
            if (label != null) label.color = Color.Lerp(resting, active, amount);
        }
    }
}

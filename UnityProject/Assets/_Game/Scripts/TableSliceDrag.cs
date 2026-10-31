using UnityEngine;
using UnityEngine.EventSystems;

namespace LoveNarrativeGame.Prototype
{
    public sealed class TableSliceDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public TableSlicePresenter Owner { get; set; }
        public bool IsCase { get; set; }
        private Vector2 start, offset;
        private bool dragging;
        private RectTransform Rect => (RectTransform)transform;
        private Vector2 Point(PointerEventData e)
        {
            var parent = (RectTransform)transform.parent;
            Vector2 p;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, e.position, e.pressEventCamera, out p);
            return p + parent.rect.size * .5f;
        }
        public void OnBeginDrag(PointerEventData e)
        {
            dragging = e.button == PointerEventData.InputButton.Left && Owner.CanDrag(IsCase);
            if (!dragging) return;
            start = Rect.anchoredPosition; offset = start - Point(e);
        }
        public void OnDrag(PointerEventData e)
        {
            if (!dragging) return;
            Vector2 p = Point(e) + offset;
            Rect.anchoredPosition = new Vector2(Mathf.Clamp(p.x, 130, 1080), Mathf.Clamp(p.y, 290, 580));
        }
        public void OnEndDrag(PointerEventData e)
        {
            if (!dragging) return;
            dragging = false;
            Vector2 dropped = Rect.anchoredPosition;
            Rect.anchoredPosition = start;
            Owner.Drop(IsCase, dropped);
        }
    }
}

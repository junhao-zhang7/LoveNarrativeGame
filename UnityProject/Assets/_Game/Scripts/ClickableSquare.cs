using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace LoveNarrativeGame.Prototype
{
    // Mouse hit detection and feedback; listeners decide what the click starts.
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public sealed class ClickableSquare : MonoBehaviour
    {
        [SerializeField] private Color selectedColor = new Color(0.25f, 0.7f, 1f, 1f);
        [SerializeField] private UnityEvent onClicked = new UnityEvent();

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D hitbox;
        private Camera sceneCamera;
        private Color originalColor;
        private bool isSelected;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            hitbox = GetComponent<BoxCollider2D>();
            sceneCamera = Camera.main;
            originalColor = spriteRenderer.color;

            if (sceneCamera == null)
            {
                Debug.LogError("ClickableSquare needs a camera tagged MainCamera.", this);
                enabled = false;
            }
        }

        private void Update()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame)
            {
                return;
            }

            // UI should consume its clicks before the scene reacts.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Vector2 screenPosition = mouse.position.ReadValue();
            if (!sceneCamera.pixelRect.Contains(screenPosition))
            {
                return;
            }

            // Screen pixels and scene coordinates use different units.
            float depth = sceneCamera.WorldToScreenPoint(transform.position).z;
            Vector3 worldPosition = sceneCamera.ScreenToWorldPoint(
                new Vector3(screenPosition.x, screenPosition.y, depth));

            if (hitbox.OverlapPoint(worldPosition))
            {
                isSelected = !isSelected;
                spriteRenderer.color = isSelected ? selectedColor : originalColor;
                onClicked.Invoke();
            }
        }
    }
}

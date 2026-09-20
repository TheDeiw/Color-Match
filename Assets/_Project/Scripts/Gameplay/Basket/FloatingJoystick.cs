using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorMatch.Gameplay.Basket
{
    public class FloatingJoystick : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform area;
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        [Header("Settings")]
        [SerializeField] private float radius = 100f;
        [SerializeField, Range(0f, 0.5f)] private float deadZone = 0.1f;

        public float Horizontal { get; private set; }

        private bool _isActive;
        private Vector2 _origin;

        private void Awake()
        {
            if (area == null) area = GetComponent<RectTransform>();
            if (background == null) background = transform.Find("Background") as RectTransform;
            if (handle == null) handle = transform.Find("Handle") as RectTransform;

           SetVisible(false);
        }

        private void Update()
        {
            // A finished round pauses time; the joystick must not appear over the results panel.
            if (Time.timeScale == 0f)
            {
                if (_isActive) End();
                return;
            }

            Pointer pointer = Pointer.current;
            if (pointer == null) return;

            Vector2 screenPos = pointer.position.ReadValue();

            if (pointer.press.wasPressedThisFrame) Begin(screenPos);
            if (_isActive && pointer.press.isPressed) Drag(screenPos);
            if (pointer.press.wasReleasedThisFrame) End();
        }

        private void Begin(Vector2 screenPos)
        {
            _isActive = true;
            _origin = ScreenToLocal(screenPos);

            background.anchoredPosition = _origin;
            handle.anchoredPosition = Vector2.zero;
            SetVisible(true);
        }

        private void Drag(Vector2 screenPos)
        {
            Vector2 offset = Vector2.ClampMagnitude(ScreenToLocal(screenPos) - _origin, radius);
            handle.anchoredPosition = offset;

            float value = offset.x / radius;
            Horizontal = Mathf.Abs(value) < deadZone ? 0f : value;
        }

        private void End()
        {
            _isActive = false;
            Horizontal = 0f;
            SetVisible(false);
        }

        private Vector2 ScreenToLocal(Vector2 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(area, screenPos, null, out Vector2 local);
            return local;
        }

        private void SetVisible(bool visible) => background.gameObject.SetActive(visible);
    }
}
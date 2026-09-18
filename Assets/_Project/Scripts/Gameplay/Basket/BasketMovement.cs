using UnityEngine;
using UnityEngine.InputSystem;

namespace ColorMatch.Gameplay.Basket
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class BasketMovement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FloatingJoystick joystick;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private float edgePadding = 0.1f;

        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;
        private Camera _mainCamera;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _collider2D = GetComponent<Collider2D>();
            _mainCamera = Camera.main;

            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }

        private void FixedUpdate()
        {
            float x = _rigidbody2D.position.x + joystick.Horizontal * moveSpeed * Time.fixedDeltaTime;
            _rigidbody2D.MovePosition(new Vector2(ClampToScreen(x), _rigidbody2D.position.y));
        }

        private float ClampToScreen(float x)
        {
            float camHalfWidth = _mainCamera.orthographicSize * _mainCamera.aspect;
            float limit = camHalfWidth - _collider2D.bounds.extents.x - edgePadding;
            float camX = _mainCamera.transform.position.x;

            return Mathf.Clamp(x, camX - limit, camX + limit);
        }
    }
}


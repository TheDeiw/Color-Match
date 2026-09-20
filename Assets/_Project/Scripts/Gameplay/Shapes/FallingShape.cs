using System;
using UnityEngine;

namespace ColorMatch.Gameplay.Shapes
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
    public class FallingShape : MonoBehaviour
    {
        public event Action<FallingShape> Released;

        public int ColorIndex { get; private set; }
        public Color Color { get; private set; }

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;
        private bool _isReleased = true;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _rigidbody2D.gravityScale = 0f;
        }

        public void Init(Vector2 position, int colorIndex, Color color, float scale, float rotation, float fallSpeed, float angularVelocity)
        {
            ColorIndex = colorIndex;
            Color = color;
            _isReleased = false;

            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, rotation));
            transform.localScale = Vector3.one * scale;
            _spriteRenderer.color = color;

            gameObject.SetActive(true);

            _rigidbody2D.angularVelocity = angularVelocity;
            _rigidbody2D.linearVelocity = Vector2.down * fallSpeed;
        }


        public void Release()
        {
            if (_isReleased) return;
            _isReleased = true;

            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.angularVelocity = 0f;
            Released?.Invoke(this);
        }
    }
}

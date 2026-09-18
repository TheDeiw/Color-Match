using System;
using UnityEngine;

namespace ColorMatch.Gameplay.Shapes
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
    public class FallingShape : MonoBehaviour
    {
        public event Action<FallingShape> Released;

        public int ColorIndex { get; private set; }

        private Rigidbody2D _rigidbody2D;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _rigidbody2D.gravityScale = 0f;
        }

        public void Init(Vector2 position, int colorIndex, Color color, float scale, float rotation, float fallSpeed, float angularVelocity)
        {
            ColorIndex = colorIndex;

            transform.SetPositionAndRotation(position, Quaternion.Euler(0f, 0f, rotation));
            transform.localScale = Vector3.one * scale;
            _spriteRenderer.color = color;

            gameObject.SetActive(true);

            _rigidbody2D.angularVelocity = angularVelocity;
            _rigidbody2D.linearVelocity = Vector2.down * fallSpeed;
        }

        public void Release()
        {
            _rigidbody2D.linearVelocity = Vector2.zero;
            Released?.Invoke(this);
        }
    }
}

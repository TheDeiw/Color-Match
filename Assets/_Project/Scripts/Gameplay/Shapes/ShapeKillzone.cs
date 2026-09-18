using System;
using UnityEngine;

namespace ColorMatch.Gameplay.Shapes
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ShapeKillzone : MonoBehaviour
    {
        public event Action<FallingShape> ShapeMissed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out FallingShape shape)) return;

            ShapeMissed?.Invoke(shape);
            shape.Release();
        }
    }
}

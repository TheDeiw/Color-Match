using System;
using ColorMatch.Gameplay.Shapes;
using UnityEngine;

namespace ColorMatch.Gameplay.Basket
{
    [RequireComponent(typeof(Collider2D))]
    public class BasketCatcher : MonoBehaviour
    {
        public event Action<FallingShape> ShapeCaught;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent(out FallingShape shape)) return;

            ShapeCaught?.Invoke(shape);
            shape.Release();
        }
    }
}

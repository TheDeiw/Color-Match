using UnityEngine;

namespace ColorMatch.Gameplay.Basket
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BasketColorView : MonoBehaviour
    {
        [SerializeField] private TargetColor targetColor;

        private SpriteRenderer _spriteRenderer;

        private void Awake() => _spriteRenderer = GetComponent<SpriteRenderer>();

        private void OnEnable() => targetColor.Changed += OnChanged;

        private void OnDisable() => targetColor.Changed -= OnChanged;

        private void OnChanged(int index) => _spriteRenderer.color = targetColor.CurrentColor;
    }
}

using ColorMatch.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace ColorMatch.UI
{
    public class TargetColorView : MonoBehaviour
    {
        [SerializeField] private TargetColor targetColor;
        [SerializeField] private Image colorCircle;
        [SerializeField] private Image timerRing;

        private void OnEnable()
        {
            targetColor.Changed += OnChanged;
            targetColor.Ticked += OnTicked;
        }

        private void OnDisable()
        {
            targetColor.Changed -= OnChanged;
            targetColor.Ticked -= OnTicked;
        }

        private void OnChanged(int index)
        {
            colorCircle.color = targetColor.CurrentColor;
            timerRing.color = targetColor.CurrentColor;
        }

        private void OnTicked(float normalized) => timerRing.fillAmount = normalized;
    }
}

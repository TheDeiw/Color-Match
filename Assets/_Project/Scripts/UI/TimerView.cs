using ColorMatch.Core;
using TMPro;
using UnityEngine;

namespace ColorMatch.UI
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text label;

        private int _shownSeconds = -1;

        private void OnEnable() => gameManager.TimeChanged += OnTimeChanged;

        private void OnDisable() => gameManager.TimeChanged -= OnTimeChanged;

        private void OnTimeChanged(float secondsLeft)
        {
            // TimeChanged fires every frame, but the label only has one second of
            // resolution. Rebuilding the text mesh 60 times per second would be waste.
            int seconds = Mathf.CeilToInt(secondsLeft);
            if (seconds == _shownSeconds) return;

            _shownSeconds = seconds;
            label.text = seconds.ToString();
        }
    }
}

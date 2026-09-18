using ColorMatch.Core;
using TMPro;
using UnityEngine;

namespace ColorMatch.UI
{
    public class ScoreView : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private TMP_Text label;

        private void OnEnable()
        {
            gameManager.ScoreChanged += OnScoreChanged;
            OnScoreChanged(gameManager.Score);
        }

        private void OnDisable() => gameManager.ScoreChanged -= OnScoreChanged;

        private void OnScoreChanged(int score) => label.text = score.ToString();
    }
}

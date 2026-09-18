using ColorMatch.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorMatch.UI
{
    public class MenuScreen : MonoBehaviour
    {
        [SerializeField] private Button startButton;
        [SerializeField] private TMP_Text bestLabel;

        private void OnEnable() => startButton.onClick.AddListener(SceneFlow.LoadGame);

        private void OnDisable() => startButton.onClick.RemoveListener(SceneFlow.LoadGame);

        private void Start()
        {
            // Entering the menu from a finished round would otherwise keep time frozen.
            Time.timeScale = 1f;
            bestLabel.text = ScoreStorage.BestScore.ToString();
        }
    }
}

using ColorMatch.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorMatch.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        // The component lives on an object that stays active, while only `root` is
        // toggled. If the component itself were disabled, its OnEnable would never
        // run and it could not subscribe to RoundEnded.
        [SerializeField] private GameObject root;

        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text bestLabel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button menuButton;

        private void Awake() => root.SetActive(false);

        private void OnEnable()
        {
            gameManager.RoundEnded += OnRoundEnded;
            retryButton.onClick.AddListener(SceneFlow.LoadGame);
            menuButton.onClick.AddListener(SceneFlow.LoadMenu);
        }

        private void OnDisable()
        {
            gameManager.RoundEnded -= OnRoundEnded;
            retryButton.onClick.RemoveListener(SceneFlow.LoadGame);
            menuButton.onClick.RemoveListener(SceneFlow.LoadMenu);
        }

        private void OnRoundEnded()
        {
            scoreLabel.text = gameManager.Score.ToString();
            bestLabel.text = gameManager.BestScore.ToString();
            root.SetActive(true);
        }
    }
}

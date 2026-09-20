using ColorMatch.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ColorMatch.UI
{
    public class PausePanel : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        // Only `root` is toggled; this component stays enabled so its OnEnable can subscribe.
        [SerializeField] private GameObject root;

        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void Awake() => root.SetActive(false);

        private void OnEnable()
        {
            gameManager.Paused += OnPaused;
            gameManager.Resumed += OnResumed;

            pauseButton.onClick.AddListener(gameManager.Pause);
            resumeButton.onClick.AddListener(gameManager.Resume);
            restartButton.onClick.AddListener(SceneFlow.LoadGame);
            menuButton.onClick.AddListener(SceneFlow.LoadMenu);
        }

        private void OnDisable()
        {
            gameManager.Paused -= OnPaused;
            gameManager.Resumed -= OnResumed;

            pauseButton.onClick.RemoveListener(gameManager.Pause);
            resumeButton.onClick.RemoveListener(gameManager.Resume);
            restartButton.onClick.RemoveListener(SceneFlow.LoadGame);
            menuButton.onClick.RemoveListener(SceneFlow.LoadMenu);
        }

        // Update still runs while the game is frozen, so this also works to unpause.
        // On Android the hardware back button arrives as Escape through the Input System.
        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame) return;

            if (gameManager.IsPaused) gameManager.Resume();
            else gameManager.Pause();
        }

        private void OnPaused() => root.SetActive(true);

        private void OnResumed() => root.SetActive(false);
    }
}

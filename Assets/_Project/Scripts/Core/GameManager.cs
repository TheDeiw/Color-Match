using System;
using ColorMatch.Gameplay;
using ColorMatch.Gameplay.Basket;
using ColorMatch.Gameplay.Shapes;
using UnityEngine;

namespace ColorMatch.Core
{
    public class GameManager : MonoBehaviour
    {
        [Header("Systems")]
        [SerializeField] private TargetColor targetColor;
        [SerializeField] private BasketCatcher catcher;

        [Header("Scoring")]
        [SerializeField] private int pointsPerMatch = 10;
        [SerializeField] private int penaltyPerWrongCatch = 5;

        [Header("Round")]
        [SerializeField] private float roundDuration = 60f;

        public event Action<int> ScoreChanged;
        public event Action<float> TimeChanged;
        public event Action RoundEnded;

        public int Score { get; private set; }
        public float TimeLeft { get; private set; }
        public bool IsRunning { get; private set; }

        private void Awake()
        {
            // Safety net: a previous round may have left the game paused.
            Time.timeScale = 1f;
            TimeLeft = roundDuration;
        }

        private void OnEnable() => catcher.ShapeCaught += OnShapeCaught;

        private void OnDisable() => catcher.ShapeCaught -= OnShapeCaught;

        // Broadcasting the initial state in Start guarantees every view has already
        // subscribed in its own OnEnable.
        private void Start()
        {
            IsRunning = true;
            ScoreChanged?.Invoke(Score);
            TimeChanged?.Invoke(TimeLeft);
        }

        private void Update()
        {
            if (!IsRunning) return;

            TimeLeft = Mathf.Max(0f, TimeLeft - Time.deltaTime);
            TimeChanged?.Invoke(TimeLeft);

            if (TimeLeft <= 0f) EndRound();
        }

        private void EndRound()
        {
            IsRunning = false;
            ScoreStorage.TrySaveBest(Score);

            // One line freezes the falling shapes, the physics and the spawner
            // coroutine, because all three are driven by scaled time.
            Time.timeScale = 0f;
            RoundEnded?.Invoke();
        }

        private void OnShapeCaught(FallingShape shape)
        {
            bool matches = shape.ColorIndex == targetColor.CurrentIndex;
            AddScore(matches ? pointsPerMatch : -penaltyPerWrongCatch);
        }

        private void AddScore(int delta)
        {
            Score = Mathf.Max(0, Score + delta);
            ScoreChanged?.Invoke(Score);
        }
    }
}

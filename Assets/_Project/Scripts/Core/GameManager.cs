using System;
using ColorMatch.Data;
using ColorMatch.Gameplay;
using ColorMatch.Gameplay.Basket;
using ColorMatch.Gameplay.Shapes;
using UnityEngine;

namespace ColorMatch.Core
{
    // Runs before the other gameplay scripts so the spawner and the target colour are
    // configured before their own Awake/OnEnable read the difficulty values.
    [DefaultExecutionOrder(-100)]
    public class GameManager : MonoBehaviour
    {
        [Header("Systems")]
        [SerializeField] private TargetColor targetColor;
        [SerializeField] private BasketCatcher catcher;
        [SerializeField] private ShapeSpawner spawner;

        [Header("Difficulty")]
        [SerializeField] private DifficultySettings defaultDifficulty;

        public event Action<int> ScoreChanged;
        public event Action<float> TimeChanged;
        public event Action RoundEnded;
        public event Action<CatchResult> CatchResolved;
        public event Action Paused;
        public event Action Resumed;

        public DifficultySettings Difficulty { get; private set; }
        public int Score { get; private set; }
        public int BestScore => ScoreStorage.GetBest(Difficulty);
        public float TimeLeft { get; private set; }
        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }

        private void Awake()
        {
            // Safety net: a previous round may have left the game paused.
            Time.timeScale = 1f;

            Difficulty = DifficultySelection.Current != null ? DifficultySelection.Current : defaultDifficulty;
            spawner.Configure(Difficulty);
            targetColor.Configure(Difficulty);
            TimeLeft = Difficulty.RoundDuration;
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

        // The round timer runs on scaled time, so freezing it is all a pause needs.
        // A finished round cannot be paused: IsRunning already guards timeScale.
        public void Pause()
        {
            if (!IsRunning || IsPaused) return;

            IsPaused = true;
            Time.timeScale = 0f;
            Paused?.Invoke();
        }

        public void Resume()
        {
            if (!IsPaused) return;

            IsPaused = false;
            Time.timeScale = 1f;
            Resumed?.Invoke();
        }

        private void EndRound()
        {
            IsRunning = false;
            ScoreStorage.TrySaveBest(Difficulty, Score);

            // One line freezes the falling shapes, the physics and the spawner
            // coroutine, because all three are driven by scaled time.
            Time.timeScale = 0f;
            RoundEnded?.Invoke();
        }

        private void OnShapeCaught(FallingShape shape)
        {
            bool matches = shape.ColorIndex == targetColor.CurrentIndex;
            int delta = matches ? Difficulty.PointsPerMatch : -Difficulty.PenaltyPerWrongCatch;

            AddScore(delta);
            CatchResolved?.Invoke(new CatchResult(shape.transform.position, shape.Color, matches, delta));
        }

        private void AddScore(int delta)
        {
            Score = Mathf.Max(0, Score + delta);
            ScoreChanged?.Invoke(Score);
        }
    }
}

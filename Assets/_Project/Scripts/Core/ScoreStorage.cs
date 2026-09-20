using ColorMatch.Data;
using UnityEngine;

namespace ColorMatch.Core
{
    /// <summary>
    /// Persists the best score per difficulty. Static on purpose: the menu scene has
    /// to read it as well, and no GameManager exists there.
    /// </summary>
    public static class ScoreStorage
    {
        private const string BestScoreKeyPrefix = "BestScore_";

        public static int GetBest(DifficultySettings difficulty) => PlayerPrefs.GetInt(Key(difficulty), 0);

        public static bool TrySaveBest(DifficultySettings difficulty, int score)
        {
            if (score <= GetBest(difficulty)) return false;

            PlayerPrefs.SetInt(Key(difficulty), score);
            PlayerPrefs.Save();
            return true;
        }

        private static string Key(DifficultySettings difficulty) => BestScoreKeyPrefix + difficulty.Id;
    }
}

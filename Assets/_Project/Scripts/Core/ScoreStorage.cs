using UnityEngine;

namespace ColorMatch.Core
{
    /// <summary>
    /// Persists the best score. Static on purpose: the menu scene has to read it
    /// as well, and no GameManager exists there.
    /// </summary>
    public static class ScoreStorage
    {
        private const string BestScoreKey = "BestScore";

        public static int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);

        public static bool TrySaveBest(int score)
        {
            if (score <= BestScore) return false;

            PlayerPrefs.SetInt(BestScoreKey, score);
            PlayerPrefs.Save();
            return true;
        }
    }
}

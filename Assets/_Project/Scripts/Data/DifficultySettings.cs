using UnityEngine;

namespace ColorMatch.Data
{
    [CreateAssetMenu(fileName = "Difficulty", menuName = "ColorMatch/Difficulty")]
    public class DifficultySettings : ScriptableObject
    {
        // Used as the save key for the best score: renaming it resets the record.
        [SerializeField] private string id = "normal";
        [SerializeField] private string displayName = "NORMAL";

        [Header("Round")]
        [SerializeField, Min(5f)] private float roundDuration = 60f;

        [Header("Shapes")]
        [SerializeField] private Vector2 spawnDelayRange = new Vector2(0.6f, 1.4f);
        [SerializeField] private Vector2 fallSpeedRange = new Vector2(2.5f, 3.5f);

        [Header("Target Colour")]
        [SerializeField, Min(1f)] private float colorDuration = 6f;

        [Header("Scoring")]
        [SerializeField] private int pointsPerMatch = 10;
        [SerializeField] private int penaltyPerWrongCatch = 5;

        public string Id => id;
        public string DisplayName => displayName;
        public float RoundDuration => roundDuration;
        public Vector2 SpawnDelayRange => spawnDelayRange;
        public Vector2 FallSpeedRange => fallSpeedRange;
        public float ColorDuration => colorDuration;
        public int PointsPerMatch => pointsPerMatch;
        public int PenaltyPerWrongCatch => penaltyPerWrongCatch;
    }
}

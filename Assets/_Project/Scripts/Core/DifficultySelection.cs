using ColorMatch.Data;

namespace ColorMatch.Core
{
    /// <summary>
    /// Carries the difficulty chosen in the menu into the game scene. Static because
    /// it has to survive the scene load; stays null when the game scene is opened directly.
    /// </summary>
    public static class DifficultySelection
    {
        public static DifficultySettings Current { get; private set; }

        public static void Select(DifficultySettings difficulty) => Current = difficulty;
    }
}

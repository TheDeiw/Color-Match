using UnityEngine;

namespace ColorMatch.Core
{
    /// <summary>
    /// Snapshot of a resolved catch. Listeners receive a copy of the data instead of
    /// the shape itself, because the shape returns to the pool right after the event
    /// and would then describe a different shape.
    /// </summary>
    public readonly struct CatchResult
    {
        public readonly Vector2 Position;
        public readonly Color Color;
        public readonly bool IsMatch;
        public readonly int ScoreDelta;

        public CatchResult(Vector2 position, Color color, bool isMatch, int scoreDelta)
        {
            Position = position;
            Color = color;
            IsMatch = isMatch;
            ScoreDelta = scoreDelta;
        }
    }
}

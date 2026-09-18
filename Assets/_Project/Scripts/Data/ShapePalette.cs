using UnityEngine;

namespace ColorMatch.Data
{
    [CreateAssetMenu(fileName = "ShapePalette", menuName = "ColorMatch/Shape Palette")]
    public class ShapePalette : ScriptableObject
    {
        [System.Serializable]
        public struct Entry
        {
            public string id;
            public Color color;
        }

        [SerializeField] private Entry[] entries;

        public int Count => entries.Length;

        public Color GetColor(int index) => entries[index].color;

        public string GetId(int index) => entries[index].id;

        public int RandomIndex() => Random.Range(0, entries.Length);
    }
}

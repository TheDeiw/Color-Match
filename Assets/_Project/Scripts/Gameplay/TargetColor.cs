using System;
using ColorMatch.Data;
using UnityEngine;

namespace ColorMatch.Gameplay
{
    public class TargetColor : MonoBehaviour
    {
        [SerializeField] private ShapePalette palette;
        [SerializeField] private float duration = 6f;

        public event Action<int> Changed;
        public event Action<float> Ticked;

        public int CurrentIndex { get; private set; } = -1;
        public Color CurrentColor => palette.GetColor(CurrentIndex);

        private float _timeLeft;

        // The first colour is picked in Start rather than Awake: by then every view
        // has subscribed in its own OnEnable and will receive the event.
        private void Start() => PickNext();

        private void Update()
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f) PickNext();

            Ticked?.Invoke(_timeLeft / duration);
        }

        private void PickNext()
        {
            // The initial value of -1 matches no index, so the first pick excludes
            // nothing, while later picks never repeat the current colour.
            int next = CurrentIndex;
            while (next == CurrentIndex) next = palette.RandomIndex();

            CurrentIndex = next;
            _timeLeft = duration;
            Changed?.Invoke(CurrentIndex);
        }
    }
}

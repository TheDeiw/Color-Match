using System;
using ColorMatch.Data;
using UnityEngine;

namespace ColorMatch.Gameplay
{
    public class TargetColor : MonoBehaviour
    {
        [SerializeField] private ShapePalette palette;

        public event Action<int> Changed;
        public event Action<float> Ticked;

        public int CurrentIndex { get; private set; } = -1;
        public Color CurrentColor => palette.GetColor(CurrentIndex);

        private float _duration = 6f;
        private float _timeLeft;

        public void Configure(DifficultySettings difficulty) => _duration = difficulty.ColorDuration;

        // The first colour is picked in Start rather than Awake: by then every view
        // has subscribed in its own OnEnable and will receive the event.
        private void Start() => PickNext();

        private void Update()
        {
            _timeLeft -= Time.deltaTime;

            if (_timeLeft <= 0f) PickNext();

            Ticked?.Invoke(_timeLeft / _duration);
        }

        private void PickNext()
        {
            // The initial value of -1 matches no index, so the first pick excludes
            // nothing, while later picks never repeat the current colour.
            int next = CurrentIndex;
            while (next == CurrentIndex) next = palette.RandomIndex();

            CurrentIndex = next;
            _timeLeft = _duration;
            Changed?.Invoke(CurrentIndex);
        }
    }
}

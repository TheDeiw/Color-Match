using System;
using ColorMatch.Core;
using ColorMatch.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ColorMatch.UI
{
    public class MenuScreen : MonoBehaviour
    {
        [Serializable]
        private struct DifficultyOption
        {
            public DifficultySettings difficulty;
            public Button button;
            public TMP_Text bestLabel;
        }

        [SerializeField] private DifficultyOption[] options;

        private void OnEnable()
        {
            foreach (DifficultyOption option in options)
            {
                DifficultySettings difficulty = option.difficulty;
                option.button.onClick.AddListener(() => StartGame(difficulty));
            }
        }

        private void OnDisable()
        {
            // Lambda listeners cannot be removed one by one; these buttons belong to this screen alone.
            foreach (DifficultyOption option in options)
                option.button.onClick.RemoveAllListeners();
        }

        private void Start()
        {
            Time.timeScale = 1f;

            foreach (DifficultyOption option in options)
                option.bestLabel.text = "BEST " + ScoreStorage.GetBest(option.difficulty);
        }

        private static void StartGame(DifficultySettings difficulty)
        {
            DifficultySelection.Select(difficulty);
            SceneFlow.LoadGame();
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

namespace ColorMatch.Core
{
    /// <summary>
    /// Single entry point for scene changes. Keeps the scene names and the time scale
    /// reset in one place, so neither the game scene nor the menu can forget them.
    /// </summary>
    public static class SceneFlow
    {
        public const string MenuScene = "Menu";
        public const string GameScene = "Game";

        public static void LoadMenu() => Load(MenuScene);

        public static void LoadGame() => Load(GameScene);

        private static void Load(string sceneName)
        {
            // A finished round leaves the game paused. Without this the next scene
            // would load already frozen.
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneName);
        }
    }
}

using ColorMatch.Core;
using UnityEngine;

namespace ColorMatch.Gameplay
{
    [RequireComponent(typeof(AudioSource))]
    public class RoundEndSound : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AudioClip clip;

        private AudioSource _audioSource;

        private void Awake() => _audioSource = GetComponent<AudioSource>();

        private void OnEnable() => gameManager.RoundEnded += OnRoundEnded;

        private void OnDisable() => gameManager.RoundEnded -= OnRoundEnded;

        // AudioSource ignores Time.timeScale, so the clip still plays while the round is frozen.
        private void OnRoundEnded() => _audioSource.PlayOneShot(clip);
    }
}

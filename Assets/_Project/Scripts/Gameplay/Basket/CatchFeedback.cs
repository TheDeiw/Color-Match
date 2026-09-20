using ColorMatch.Core;
using UnityEngine;

namespace ColorMatch.Gameplay.Basket
{
    public class CatchFeedback : MonoBehaviour
    {
        [SerializeField] private GameManager gameManager;

        [Header("Particles")]
        [SerializeField] private ParticleSystem matchBurst;
        [SerializeField] private ParticleSystem wrongBurst;
        [SerializeField] private int matchParticles = 18;
        [SerializeField] private int wrongParticles = 8;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip matchClip;
        [SerializeField] private AudioClip wrongClip;
        [SerializeField] private Vector2 pitchRange = new Vector2(0.94f, 1.08f);

        private void OnEnable() => gameManager.CatchResolved += OnCatchResolved;

        private void OnDisable() => gameManager.CatchResolved -= OnCatchResolved;

        private void OnCatchResolved(CatchResult result)
        {
            PlayParticles(result);
            PlaySound(result.IsMatch ? matchClip : wrongClip);
        }

        private void PlayParticles(CatchResult result)
        {
            var emitParams = new ParticleSystem.EmitParams
            {
                position = result.Position,
                applyShapeToPosition = true
            };

            if (result.IsMatch)
            {
                emitParams.startColor = result.Color;
                matchBurst.Emit(emitParams, matchParticles);
            }
            else
            {
                wrongBurst.Emit(emitParams, wrongParticles);
            }
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip == null) return;

            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(clip);
        }
    }
}

namespace CT6GAMAI
{
    using System.Collections;
    using UnityEngine;

    public class TurnMusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _enemyPhaseMusic;
        [SerializeField] private AudioClip _playerPhaseMusic;
        [SerializeField] private AudioClip _deathMusic;
        [SerializeField] private bool _isPlayerMusic;
        [SerializeField] private float crossFadeDuration = 2.0f; // Duration of the crossfade

        public void PlayPlayerPhaseMusic()
        {
            _isPlayerMusic = true;
            StartCoroutine(CrossfadeMusic(_playerPhaseMusic));
        }

        public void PlayEnemyPhaseMusic()
        {
            _isPlayerMusic = false;
            StartCoroutine(CrossfadeMusic(_enemyPhaseMusic));
        }

        public void PlayDeathMusic()
        {
            StartCoroutine(CrossfadeMusic(_deathMusic));
        }

        public void ResumeLastMusic()
        {
            if (_isPlayerMusic)
            {
                PlayPlayerPhaseMusic();
            }
            else
            {
                PlayEnemyPhaseMusic();
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip)
        {
            if (_audioSource.clip != null)
            {
                // Fade out current clip
                float startVolume = _audioSource.volume;
                for (float t = 0; t < crossFadeDuration; t += Time.deltaTime)
                {
                    _audioSource.volume = Mathf.Lerp(startVolume, 0, t / crossFadeDuration);
                    yield return null;
                }
            }

            // Change the clip and fade in
            _audioSource.clip = newClip;
            _audioSource.Play();
            for (float t = 0; t < crossFadeDuration; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, 1, t / crossFadeDuration);
                yield return null;
            }
        }
    }

}
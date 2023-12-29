namespace CT6GAMAI
{
    using System.Collections;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the music for different phases of the game.
    /// </summary>
    public class TurnMusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _enemyPhaseMusic;
        [SerializeField] private AudioClip _playerPhaseMusic;
        [SerializeField] private AudioClip _deathMusic;
        [SerializeField] private bool _isPlayerMusic;

        /// <summary>
        /// Plays the player phase music
        /// </summary>
        public void PlayPlayerPhaseMusic()
        {
            _isPlayerMusic = true;
            StartCoroutine(CrossfadeMusic(_playerPhaseMusic));
        }

        /// <summary>
        /// Plays the enemy phase music
        /// </summary>
        public void PlayEnemyPhaseMusic()
        {
            _isPlayerMusic = false;
            StartCoroutine(CrossfadeMusic(_enemyPhaseMusic));
        }

        /// <summary>
        /// Plays the death music
        /// </summary>
        public void PlayDeathMusic()
        {
            StartCoroutine(CrossfadeMusic(_deathMusic));
        }

        /// <summary>
        /// Resumes the last playing phase music. 
        /// Used for after playing other music events.
        /// </summary>
        public void ResumeLastPhaseMusic()
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
                for (float t = 0; t < CROSSFADE_DURATION; t += Time.deltaTime)
                {
                    _audioSource.volume = Mathf.Lerp(startVolume, 0, t / CROSSFADE_DURATION);
                    yield return null;
                }
            }

            // Change the clip and fade in
            _audioSource.clip = newClip;
            _audioSource.Play();
            for (float t = 0; t < CROSSFADE_DURATION; t += Time.deltaTime)
            {
                _audioSource.volume = Mathf.Lerp(0, 1, t / CROSSFADE_DURATION);
                yield return null;
            }
        }
    }

}
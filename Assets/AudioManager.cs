namespace CT6GAMAI
{
    using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource _cursorAudioSource;
        [SerializeField] private CursorAudioClips _cursorAudioClips;

        public CursorAudioClips CursorAudioClips => _cursorAudioClips;

        public void PlayCursorSound(bool isUnitPressed)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isUnitPressed ? _cursorAudioClips.Move2 : _cursorAudioClips.Move1);
            }
        }

        public void PlayToggleUnitSound(bool isUnitPressed)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isUnitPressed ? _cursorAudioClips.SelectUnit : _cursorAudioClips.Cancel);
            }
        }

        public void PlaySelectPathSound(bool isValidPath)
        {
            if (!_cursorAudioSource.isPlaying)
            {
                _cursorAudioSource.PlayOneShot(isValidPath ? _cursorAudioClips.Confirm : _cursorAudioClips.Invalid);
            }
        }
    }
}
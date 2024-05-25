namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.Video;
    using System.Collections;
    using UnityEngine.SceneManagement;

    public class IdleVideoPlayer : MonoBehaviour
    {
        public VideoPlayer _videoPlayer;
        public CanvasGroup _titleScreenCanvasGroup;
        public CanvasGroup _videoCanvasGroup;
        public float _idleTime = 10f;
        public float _fadeDuration = 1f;

        private float _idleTimer = 0f;

        void Start()
        {
            if (_videoPlayer)
            {
                _videoPlayer.Stop();
                _videoPlayer.loopPointReached += OnVideoEnd;
            }
            if (_videoCanvasGroup)
            {
                _videoCanvasGroup.alpha = 0f; // Ensure the video canvas is initially invisible
            }
        }

        void Update()
        {
            // Check for any input
            if (Input.anyKeyDown)
            {
                // Reset the timer on any input
                _idleTimer = 0f;
                // Stop the video if it is playing and reset the UI
                if (_videoPlayer && _videoPlayer.isPlaying)
                {
                    _videoPlayer.Stop();
                    StartCoroutine(FadeCanvasGroup(_videoCanvasGroup, _fadeDuration, 0f));
                    StartCoroutine(FadeCanvasGroup(_titleScreenCanvasGroup, _fadeDuration, 1f));
                }

                SceneManager.LoadScene("Map1");
            }
            else
            {
                // Increment the timer if no input is detected
                _idleTimer += Time.deltaTime;
            }

            // Check if the idle timer has exceeded the idle time
            if (_idleTimer >= _idleTime)
            {
                // Play the video if the idle time is reached
                if (_videoPlayer && !_videoPlayer.isPlaying)
                {
                    StartCoroutine(FadeOutTitleAndPlayVideo());
                }
            }
        }

        private IEnumerator FadeOutTitleAndPlayVideo()
        {
            yield return StartCoroutine(FadeCanvasGroup(_titleScreenCanvasGroup, _fadeDuration, 0f));
            yield return new WaitForSeconds(2f); // Delay before video fade-in
            StartCoroutine(FadeCanvasGroup(_videoCanvasGroup, _fadeDuration, 1f));
            _videoPlayer.Play();
        }

        private void OnVideoEnd(VideoPlayer vp)
        {
            _idleTimer = 0;
            StartCoroutine(FadeOutVideoAndShowTitleScreen());
        }

        private IEnumerator FadeOutVideoAndShowTitleScreen()
        {
            yield return StartCoroutine(FadeCanvasGroup(_videoCanvasGroup, _fadeDuration, 0f));
            _videoPlayer.Stop(); // Ensure the video stops completely
            yield return new WaitForSeconds(2f); // Optional delay before showing the title screen again
            StartCoroutine(FadeCanvasGroup(_titleScreenCanvasGroup, _fadeDuration, 1f));
        }

        private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float duration, float targetAlpha)
        {
            float startAlpha = canvasGroup.alpha;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
        }
    }
}

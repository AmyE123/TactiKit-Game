namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class IdleChecker : MonoBehaviour
    {
        public float idleTime = 600f;
        private float idleTimer = 0f;

        [SerializeField]
        private GameManager gameManager;

        void Update()
        {
            // Check for any input
            if (Input.anyKeyDown || Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0 || gameManager.TurnManager.ActivePhase == Constants.Phases.EnemyPhase)
            {
                // Reset the timer on any input
                idleTimer = 0f;
            }
            else
            {
                // Increment the timer if no input is detected
                idleTimer += Time.deltaTime;
            }

            // Check if the idle timer has exceeded the idle time
            if (idleTimer >= idleTime)
            {
                // Load the menu scene if the idle time is reached
                LoadMenuScene();
            }

            if (Input.GetKeyDown(KeyCode.Minus))
            {
                LoadMenuScene();
            }
        }

        private void LoadMenuScene()
        {
            // Replace "MenuScene" with the actual name of your menu scene
            SceneManager.LoadScene("TitleScreen");
        }
    }
}
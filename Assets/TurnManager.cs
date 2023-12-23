namespace CT6GAMAI
{
    using System;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private Phases _activePhase;

        private GameManager _gameManager;

        // Event that can be used to notify other parts of the game when the phase changes
        public event Action<Phases> OnPhaseChanged;

        /// <summary>
        /// The active phase
        /// </summary>
        public Phases ActivePhase => _activePhase;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            SetPhase(Phases.PlayerPhase);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                _activePhase = Phases.PlayerPhase;
                SetPhase(_activePhase);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _activePhase = Phases.EnemyPhase;
                SetPhase(_activePhase);
            }
        }

        /// <summary>
        /// Sets the phase to a new phase.
        /// </summary>
        /// <param name="newPhase">The new phase you want to transition to.</param>
        public void SetPhase(Phases newPhase)
        {
            _activePhase = newPhase;
            OnPhaseChanged?.Invoke(_activePhase);

            switch (_activePhase)
            {
                case Phases.PlayerPhase:
                    StartPlayerPhase();
                    break;
                case Phases.EnemyPhase:
                    StartEnemyPhase();
                    break;
            }
        }

        private void StartPlayerPhase()
        {
            // Enable player input
            _gameManager.GridManager.GridCursor.SetPlayerInput(true);
        }

        private void StartEnemyPhase()
        {
            // Disable player input
            _gameManager.GridManager.GridCursor.SetPlayerInput(false);

            // Run AI actions
        }
    }
}
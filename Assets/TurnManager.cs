namespace CT6GAMAI
{
    using System;
    using System.Collections.Generic;
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

        private bool _isPhaseStarted = false;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            SetPhase(Phases.PlayerPhase);
        }

        private void StartPhase()
        {
            foreach (var unit in _gameManager.UnitsManager.ActivePlayerUnits)
            {
                unit.ResetTurn();
            }
            _isPhaseStarted = true;
        }

        private void Update()
        {
            if (!_isPhaseStarted)
            {
                StartPhase();
            }
            else if (CheckAllTurnsHaveBeenTaken(ActivePhase))
            {
                SwitchPhase();
                _isPhaseStarted = false;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
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

        private bool CheckAllTurnsHaveBeenTaken(Phases turn)
        {
            List<UnitManager> units = turn == Phases.PlayerPhase ? _gameManager.UnitsManager.ActivePlayerUnits : _gameManager.UnitsManager.ActiveEnemyUnits;

            foreach (UnitManager unit in units)
            {
                if (!unit.HasActedThisTurn)
                {
                    return false;
                }
            }

            return true;
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

        public void SwitchPhase()
        {
            Debug.Log("Switching Phases");

            if (ActivePhase == Phases.PlayerPhase)
            {
                SetPhase(Phases.EnemyPhase);
            }
            else
            {
                SetPhase(Phases.PlayerPhase);
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
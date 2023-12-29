namespace CT6GAMAI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class TurnManager : MonoBehaviour
    {
        [SerializeField] private Phases _activePhase;
        [SerializeField] private UI_PhaseManager _uiPhaseManager;
        [SerializeField] private TurnMusicManager _turnMusicManager;

        private GameManager _gameManager;

        // Event that can be used to notify other parts of the game when the phase changes
        public event Action<Phases> OnPhaseChanged;

        /// <summary>
        /// The active phase
        /// </summary>
        public Phases ActivePhase => _activePhase;

        public TurnMusicManager TurnMusicManager => _turnMusicManager;

        private bool _isPhaseStarted = false;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            SetPhase(Phases.PlayerPhase);
        }

        private void StartPhase()
        {
            if (!_isPhaseStarted)
            {
                if (ActivePhase == Phases.PlayerPhase)
                {
                    foreach (var unit in _gameManager.UnitsManager.ActivePlayerUnits)
                    {
                        unit.ResetTurn();
                    }
                }

                _isPhaseStarted = true;
            }
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
                SwitchPhase();
                _isPhaseStarted = false;
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

            _isPhaseStarted = false;
        }

        public void SwitchPhase()
        {            
            Debug.Log("Switching Phases");

            Phases nextPhase = ActivePhase == Phases.PlayerPhase ? Phases.EnemyPhase : Phases.PlayerPhase;

            _uiPhaseManager.DisplayPhaseUI(nextPhase);
            StartCoroutine(TransitionToNextPhase(nextPhase, 1.0f));
        }

        private IEnumerator TransitionToNextPhase(Phases nextPhase, float delay)
        {            
            UpdatePlayerInput(nextPhase);

            // Here you might play an animation or transition effect
            yield return new WaitForSeconds(delay);

            SetPhase(nextPhase);
        }

        private void UpdatePlayerInput(Phases nextPhase)
        {
            if (nextPhase == Phases.EnemyPhase)
            {
                _gameManager.GridManager.GridCursor.SetPlayerInput(false);
            }
        }

        private void StartPlayerPhase()
        {
            // Enable player input
            _gameManager.GridManager.GridCursor.SetPlayerInput(true);
            _turnMusicManager.PlayPlayerPhaseMusic();
        }

        private void StartEnemyPhase()
        {
            // Disable player input
            //_gameManager.GridManager.GridCursor.SetPlayerInput(false);

            // Run AI actions
            _gameManager.GridManager.GridCursor.MoveCursorTo(_gameManager.UnitsManager.ActiveEnemyUnits[0].StoodNode.Node);
            _turnMusicManager.PlayEnemyPhaseMusic();
        }
    }
}
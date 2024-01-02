namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    /// <summary>
    /// Manages all units in the game, handling their initialization and state.
    /// </summary>
    public class GlobalUnitsManager : MonoBehaviour
    {
        [SerializeField] private List<UnitManager> _allUnits;
        [SerializeField] private List<UnitManager> _activeUnits;
        [SerializeField] private List<UnitManager> _activePlayerUnits;
        [SerializeField] private List<UnitManager> _activeEnemyUnits;
        [SerializeField] private List<UnitManager> _unplayedPlayerUnits;

        [SerializeField] private UnitManager _cursorUnit;
        [SerializeField] private UnitManager _activeUnit;
        [SerializeField] private UnitManager _lastSelectedPlayerUnit;

        private bool _unitsInitalized = false;

        private GameManager _gameManager;

        /// <summary>
        /// Gets the list of all units in the game.
        /// </summary>
        public List<UnitManager> AllUnits => _allUnits;

        /// <summary>
        /// Gets the list of active units in the game.
        /// </summary>
        public List<UnitManager> ActiveUnits => _activeUnits;

        /// <summary>
        /// Gets the currently active/selected unit in the game.
        /// </summary>
        public UnitManager ActiveUnit => _activeUnit;

        /// <summary>
        /// Gets the last selected player unit.
        /// </summary>
        public UnitManager LastSelectedPlayerUnit => _lastSelectedPlayerUnit;

        /// <summary>
        /// Gets the current unit underneath the cursor.
        /// </summary>
        public UnitManager CursorUnit => _cursorUnit;

        /// <summary>
        /// The active player units in the current game.
        /// </summary>
        public List<UnitManager> ActivePlayerUnits => _activePlayerUnits;

        /// <summary>
        /// The active enemy units in the current game.
        /// </summary>
        public List<UnitManager> ActiveEnemyUnits => _activeEnemyUnits;

        /// <summary>
        /// Any unplayed player units in the current turn.
        /// </summary>
        public List<UnitManager> UnplayedPlayerUnits => _unplayedPlayerUnits;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {            
            if (!_unitsInitalized)
            {
                InitializeUnits();
            }

            if (_unitsInitalized)
            {
                StartCoroutine(CheckForAllDeadUnits());
                CheckForUnplayedUnits();
                CheckForVictory();
            }
        }

        private IEnumerator CheckForAllDeadUnits()
        {
            if (_activePlayerUnits.Count <= 0 || _activeEnemyUnits.Count <= 0)
            {
                string currentSceneName = SceneManager.GetActiveScene().name;

                yield return new WaitForSeconds(6);

                SceneManager.LoadScene(currentSceneName);
            }
        }

        private void CheckForUnplayedUnits()
        {
            _unplayedPlayerUnits.Clear();

            foreach (var unit in ActivePlayerUnits)
            {
                if (!unit.HasActedThisTurn)
                {
                    if(!_unplayedPlayerUnits.Contains(unit))
                    {
                        _unplayedPlayerUnits.Add(unit);
                    }                   
                }
            }
        }

        private void InitializeUnits()
        {
            if (_allUnits.Count == 0)
            {
                FindAllUnits();
            }

            _unitsInitalized = true;
        }

        private void FindAllUnits()
        {
            _allUnits = FindObjectsOfType<UnitManager>().ToList();
            _activeUnits = _allUnits;
            UpdateAllUnits();
        }

        /// <summary>
        /// Updates the state of all units in the game
        /// </summary>
        public void UpdateAllUnits()
        {
            _activeEnemyUnits.Clear();
            _activePlayerUnits.Clear();          

            List<UnitManager> unitsToRemove = new List<UnitManager>();

            foreach (UnitManager unit in _activeUnits)
            {
                if (!unit.gameObject.activeSelf)
                {
                    unitsToRemove.Add(unit);
                    continue;
                }

                if (unit.UnitData.UnitTeam == Constants.Team.Player)
                {
                    if (!_activePlayerUnits.Contains(unit) && unit.gameObject.activeSelf)
                    {
                        _activePlayerUnits.Add(unit);
                    }
                }
                else
                {
                    if (!_activeEnemyUnits.Contains(unit) && unit.gameObject.activeSelf)
                    {
                        _activeEnemyUnits.Add(unit);
                    }
                }
            }

            // Remove the inactive units
            foreach (UnitManager unit in unitsToRemove)
            {
                _activeUnits.Remove(unit);
            }
        }

        public void CheckForVictory()
        {
            if (_activeEnemyUnits.Count <= 2 && _gameManager.TurnManager.ActivePhase != Constants.Phases.EnemyPhase)
            {
                _gameManager.TurnManager.TurnMusicManager.PlayVictoryMusic = true;
            }
        }

        /// <summary>
        /// A check to see if any unit is currently in a 'moving' state.
        /// </summary>
        /// <returns>True if any unit is moving.</returns>
        public bool IsAnyUnitMoving()
        {
            foreach (UnitManager unit in _allUnits)
            {
                if (unit.IsMoving)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Looks through all the units to check if any of them are currently in battle animations.
        /// </summary>
        /// <returns>True if any units are battling.</returns>
        public bool IsAnyUnitBattling()
        {
            foreach (UnitManager unit in _allUnits)
            {
                if (unit.IsInBattle)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Setter for the current active unit.
        /// </summary>
        /// <param name="unit">The unit you want to set as active.</param>
        public void SetActiveUnit(UnitManager unit)
        {
            _activeUnit = unit;
        }

        /// <summary>
        /// Setter for the last selected player unit.
        /// </summary>
        /// <param name="unit">The unit you want to set as last selected.</param>
        public void SetLastSelectedPlayerUnit(UnitManager unit)
        {
            _lastSelectedPlayerUnit = unit;
        }

        /// <summary>
        /// Setter for the cursor unit.
        /// </summary>
        /// <param name="unit">The unit you want to set as under the cursor.</param>
        public void SetCursorUnit(UnitManager unit)
        {
            _cursorUnit = unit;
        }
    }
}
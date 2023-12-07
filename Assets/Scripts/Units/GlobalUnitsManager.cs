namespace CT6GAMAI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Manages all units in the game, handling their initialization and state.
    /// </summary>
    public class GlobalUnitsManager : MonoBehaviour
    {
        [SerializeField] private List<UnitManager> _allUnits;
        [SerializeField] private List<UnitManager> _activeUnits;

        [SerializeField] private UnitManager _selectorUnit;
        [SerializeField] private UnitManager _activeUnit;

        private bool _unitsInitalized = false;

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

        public UnitManager SelectorUnit => _selectorUnit;

        private void Update()
        {           
            if (!_unitsInitalized)
            {
                InitializeUnits();                
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
        /// Setter for the current active unit.
        /// </summary>
        /// <param name="unit">The unit you want to set as active.</param>
        public void SetActiveUnit(UnitManager unit)
        {
            _activeUnit = unit;           
        }

        public void SetSelectorUnit(UnitManager unit)
        {
            _selectorUnit = unit;
        }
    }
}
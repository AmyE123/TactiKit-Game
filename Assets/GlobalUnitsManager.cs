namespace CT6GAMAI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GlobalUnitsManager : MonoBehaviour
    {
        [SerializeField] private List<UnitManager> _allUnits;
        [SerializeField] private List<UnitManager> _activeUnits;

        private bool _unitsInitalized = false;

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
        }
    }
}
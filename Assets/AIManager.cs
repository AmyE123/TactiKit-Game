namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AIManager : MonoBehaviour
    {
        private GameManager _gameManager;

        [SerializeField] List<UnitAIManager> _aiUnits;

        public List<UnitAIManager> AIUnits => _aiUnits;

        public void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void StartEnemyAI()
        {
            StartCoroutine(HandleAIUnitTurns());
        }

        private IEnumerator HandleAIUnitTurns()
        {
            foreach (UnitAIManager ai in _aiUnits)
            {
                yield return ai.BeginEnemyAI();
            }
        }

        public void UpdateAIUnits()
        {
            _aiUnits.Clear();
            List<UnitManager> activeEnemyAI = _gameManager.UnitsManager.ActiveEnemyUnits;

            foreach (UnitManager unit in activeEnemyAI)
            {
                UnitAIManager UnitAI = unit.GetComponent<UnitAIManager>();
                if (UnitAI != null)
                {
                    _aiUnits.Add(UnitAI);
                }
                else
                {
                    Debug.LogWarning("[AI]: Unit hasn't got AI component added");
                }               
            }
        }
    }
}
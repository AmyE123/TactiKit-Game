namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the battle properties and behaviors of a unit in the game.
    /// </summary>
    public class BattleUnitManager : MonoBehaviour
    {
        [Header("Unit Identification")]
        [SerializeField] private Side _unitSide;
        [SerializeField] private UnitManager _unitManagerRef;
        [SerializeField] private UnitStatsManager _unitStatsManagerRef;

        [Header("Unit Animation")]
        [SerializeField] private Animator _animator;

        [Header("Unit Status")]
        [SerializeField] private bool _canUnitAttackAgain;
        [SerializeField] private bool _unitCompleteAttacks;

        /// <summary>
        /// Gets the side which the unit is on.
        /// </summary>
        public Side UnitSide => _unitSide;

        /// <summary>
        /// Gets the animator commonent attached to the battle unit.
        /// </summary>
        public Animator Animator => _animator;

        /// <summary>
        /// Gets the UnitManager reference for the unit.
        /// </summary>
        public UnitManager UnitManagerRef => _unitManagerRef;

        /// <summary>
        /// Gets the UnitStatsManager reference for the unit.
        /// </summary>
        public UnitStatsManager UnitStatsManager => _unitStatsManagerRef;

        /// <summary>
        /// Whether the unit can attack again.
        /// </summary>
        public bool CanUnitAttackAgain => _canUnitAttackAgain;

        /// <summary>
        /// Whether the uni has completed all of its attacks.
        /// </summary>
        public bool UnitCompleteAttacks => _unitCompleteAttacks;

        public void SetUnitCompleteAttacks(bool value)
        {
            _unitCompleteAttacks = value;
        }

        public void SetUnitReferences(UnitStatsManager unitStats, UnitManager unitManager)
        {
            int nbChildren = transform.childCount;

            for (int i = nbChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }

            _unitStatsManagerRef = unitStats;
            _unitManagerRef = unitManager;
            _canUnitAttackAgain = _unitStatsManagerRef.DblAtk;
            var battleGO = Instantiate(_unitManagerRef.BattleUnit, transform);
            _animator = battleGO.GetComponentInChildren<Animator>();
        }
    }
}
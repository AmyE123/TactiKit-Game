namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class BattleUnitManager : MonoBehaviour
    {
        [SerializeField] private Side _unitSide;
        [SerializeField] private Animator _animator;
        //[SerializeField] private UnitManager _unitReference;
        [SerializeField] private bool _canUnitAttackAgain;
        [SerializeField] private bool _unitCompleteAttacks;
        [SerializeField] private UnitStatsManager _unitStatsManagerRef;

        public Side UnitSide => _unitSide;
        public Animator Animator => _animator;
        public bool CanUnitAttackAgain => _canUnitAttackAgain;
        public bool UnitCompleteAttacks => _unitCompleteAttacks;
        public UnitStatsManager UnitStatsManager => _unitStatsManagerRef;

        public void SetUnitCompleteAttacks(bool value)
        {
            _unitCompleteAttacks = value;
        }
    }
}
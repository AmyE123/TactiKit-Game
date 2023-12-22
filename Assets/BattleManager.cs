namespace CT6GAMAI
{
    using UnityEngine;

    public class BattleManager : MonoBehaviour
    {
        [Header("Current Battle Stats")]
        [SerializeField] private UnitManager _unitA;
        [SerializeField] private int _attackA;
        [SerializeField] private bool _canDoubleAttackA;
        [SerializeField] private int _hitRateA;
        [SerializeField] private int _critRateA;
        [SerializeField] private int _remainingHPA;

        [SerializeField] private UnitManager _unitB;
        [SerializeField] private int _attackB;
        [SerializeField] private bool _canDoubleAttackB;
        [SerializeField] private int _hitRateB;
        [SerializeField] private int _critRateB;
        [SerializeField] private int _remainingHPB;

        public UnitManager UnitA => _unitA;
        public int AttackA => _attackA;
        public bool CanDoubleAttackA => _canDoubleAttackA;
        public int HitRateA => _hitRateA;
        public int CritRateA => _critRateA;
        public int RemainingHPA => _remainingHPA;

        public UnitManager UnitB => _unitB;
        public int AttackB => _attackB;
        public bool CanDoubleAttackB => _canDoubleAttackB;
        public int HitRateB => _hitRateB;
        public int CritRateB => _critRateB;
        public int RemainingHPB => _remainingHPB;

        [Header("Other Components")]
        [SerializeField] private BattleSequenceManager _battleSequenceManager;

        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private UIManager _uiManager;
        private bool _isBattleActive;

        [SerializeField] private GameObject _unitModel;

        public bool IsBattleActive => _isBattleActive;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _cameraManager = _gameManager.CameraManager;
        }

        public void SwitchToBattle()
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[1]);
            _isBattleActive = true;
            _battleSequenceManager.StartBattle(_gameManager.UnitsManager.ActiveUnit.UnitData.UnitTeam, _unitA, _unitB);
        }

        public void SwitchToMap()
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[0]);
            _isBattleActive = false;

            var unit = _gameManager.UnitsManager.ActiveUnit;

            unit.FinalizeMovementValues(_gameManager.GridManager.MovementPath.Count - 1);
            unit.IsAwaitingMoveConfirmation = false;
            _gameManager.UIManager.ActionItemsManager.HideActionItems();
            _gameManager.UIManager.BattleForecastManager.CancelBattleForecast();
        }

        public void CalculateValuesForBattleForecast(UnitManager unitA, UnitManager unitB)
        {
            _attackA = BattleCalculator.CalculateAttackPower(unitA, unitB);
            _canDoubleAttackA = BattleCalculator.CanDoubleAttack(unitA, unitB);
            _hitRateA = BattleCalculator.CalculateHitRatePercentage(unitA, unitB);
            _critRateA = BattleCalculator.CalculateCriticalRatePercentage(unitA, unitB);

            _attackB = BattleCalculator.CalculateAttackPower(unitB, unitA);
            _canDoubleAttackB = BattleCalculator.CanDoubleAttack(unitB, unitA);
            _hitRateB = BattleCalculator.CalculateHitRatePercentage(unitB, unitA);
            _critRateB = BattleCalculator.CalculateCriticalRatePercentage(unitB, unitA);

            _remainingHPA = BattleCalculator.CalculateRemainingHPForecast(unitA, AttackB, CanDoubleAttackB);
            _remainingHPB = BattleCalculator.CalculateRemainingHPForecast(unitB, AttackA, CanDoubleAttackA);

            _gameManager.BattleManager.SetBattleStats(unitA, AttackA, CanDoubleAttackA, HitRateA, CritRateA, RemainingHPA, unitB, AttackB, CanDoubleAttackB, HitRateB, CritRateB, RemainingHPB);
        }

        public void SetBattleStats(UnitManager unitA, int atkA, bool dblAtkA, int hitA, int critA, int remainingHPA, UnitManager unitB, int atkB, bool dblAtkB, int hitB, int critB, int remainingHPB)
        {
            _unitA = unitA;
            _attackA = atkA;
            _canDoubleAttackA = dblAtkA;
            _hitRateA = hitA;
            _critRateA = critA;
            _remainingHPA = remainingHPA;
            SetUnitStats(unitA.UnitStatsManager, atkA, dblAtkA, hitA, critA);

            _unitB = unitB;
            _attackB = atkB;
            _canDoubleAttackB = dblAtkB;
            _hitRateB = hitB;
            _critRateB = critB;
            _remainingHPB = remainingHPB;
            SetUnitStats(unitB.UnitStatsManager, atkB, dblAtkB, hitB, critB);
        }

        private void SetUnitStats(UnitStatsManager unitStats, int atk, bool dblAtk, int hit, int crit)
        {
            unitStats.SetUnitStats(atk, dblAtk, hit, crit);

        }
    }
}
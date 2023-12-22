namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_BattleForecastManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleForecastSideManager[] _battleForecastManagers;

        private bool _areBattleForecastsToggled;

        public bool AreBattleForecastsToggled => _areBattleForecastsToggled;

        public int AttackA;
        public bool CanDoubleAttackA;
        public int HitRateA;
        public int CritRateA;
        public int RemainingHPA;

        public int AttackB;
        public bool CanDoubleAttackB;
        public int HitRateB;
        public int CritRateB;
        public int RemainingHPB;

        public void CalculateValuesForBattleForecast(UnitManager unitA, UnitManager unitB)
        {
            AttackA = BattleCalculator.CalculateAttackPower(unitA, unitB);
            CanDoubleAttackA = BattleCalculator.CanDoubleAttack(unitA, unitB);
            HitRateA = BattleCalculator.CalculateHitRatePercentage(unitA, unitB);
            CritRateA = BattleCalculator.CalculateCriticalRatePercentage(unitA, unitB);         

            AttackB = BattleCalculator.CalculateAttackPower(unitB, unitA);
            CanDoubleAttackB = BattleCalculator.CanDoubleAttack(unitB, unitA);
            HitRateB = BattleCalculator.CalculateHitRatePercentage(unitB, unitA);
            CritRateB = BattleCalculator.CalculateCriticalRatePercentage(unitB, unitA);

            RemainingHPA = CalculateRemainingHPForecast(unitA, AttackB, CanDoubleAttackB);
            RemainingHPB = CalculateRemainingHPForecast(unitB, AttackA, CanDoubleAttackA);

            PopulateBattleForecastValues(0, unitA, AttackA, CanDoubleAttackA, HitRateA, CritRateA, RemainingHPA);
            PopulateBattleForecastValues(1, unitB, AttackB, CanDoubleAttackB, HitRateB, CritRateB, RemainingHPB);
        }

        public void PopulateBattleForecastValues(int sideIdx, UnitManager unit, int attack, bool canDoubleAttack, int hitRate, int critRate, int remainingHP)
        {
            _battleForecastManagers[sideIdx].PopulateBattleForecastData(unit.UnitData, attack, canDoubleAttack, hitRate, critRate, unit.UnitStatsManager.HealthPoints, remainingHP);
        }

        public void SpawnBattleForecast(UnitManager unitA, UnitManager unitB)
        {
            CalculateValuesForBattleForecast(unitA, unitB);

            _battleForecastManagers[0].SpawnBattleForecastSide();
            _battleForecastManagers[1].SpawnBattleForecastSide();

            _areBattleForecastsToggled = true;
        }

        public void CancelBattleForecast()
        {
            _battleForecastManagers[0].CancelBattleForecast();
            _battleForecastManagers[1].CancelBattleForecast();

            _areBattleForecastsToggled = false;
        }

        private int CalculateRemainingHPForecast(UnitManager unit, int attackAmount, bool canDoubleAttack)
        {
            var attackValue = attackAmount;

            if (canDoubleAttack)
            {
                attackValue *= 2;
            }

            return unit.UnitStatsManager.HealthPoints - attackValue;
        }
    }

}
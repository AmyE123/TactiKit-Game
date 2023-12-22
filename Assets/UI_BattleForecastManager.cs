namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_BattleForecastManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleForecastSideManager[] _battleForecastManagers;

        private GameManager _gameManager;

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

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void GetValuesForBattleForecast(UnitManager unitA, UnitManager unitB)
        {
            var battleManager = _gameManager.BattleManager;

            battleManager.SetBattleStats(unitA, AttackA, CanDoubleAttackA, HitRateA, CritRateA, RemainingHPA, unitB, AttackB, CanDoubleAttackB, HitRateB, CritRateB, RemainingHPB);
            battleManager.CalculateValuesForBattleForecast(unitA, unitB);
            
            PopulateBattleForecastValues(0, unitA, battleManager.AttackA, battleManager.CanDoubleAttackA, battleManager.HitRateA, battleManager.CritRateA, battleManager.RemainingHPA);
            PopulateBattleForecastValues(1, unitB, battleManager.AttackB, battleManager.CanDoubleAttackB, battleManager.HitRateB, battleManager.CritRateB, battleManager.RemainingHPB);
        }

        public void PopulateBattleForecastValues(int sideIdx, UnitManager unit, int attack, bool canDoubleAttack, int hitRate, int critRate, int remainingHP)
        {
            _battleForecastManagers[sideIdx].PopulateBattleForecastData(unit.UnitData, attack, canDoubleAttack, hitRate, critRate, unit.UnitStatsManager.HealthPoints, remainingHP);
        }

        public void SpawnBattleForecast(UnitManager unitA, UnitManager unitB)
        {
            GetValuesForBattleForecast(unitA, unitB);

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
    }

}
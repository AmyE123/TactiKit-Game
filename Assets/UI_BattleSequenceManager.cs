namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_BattleSequenceManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleSequenceSideManager[] _battleSequenceSideManagers;

        [SerializeField] private BattleManager _battleManager;

        public void GetValuesForBattleSequenceUI(UnitManager unitA, UnitManager unitB)
        {           
            PopulateBattleSequenceUIValues(0, unitA, _battleManager.AttackA, _battleManager.CanDoubleAttackA, _battleManager.HitRateA, _battleManager.CritRateA);
            PopulateBattleSequenceUIValues(1, unitB, _battleManager.AttackB, _battleManager.CanDoubleAttackB, _battleManager.HitRateB, _battleManager.CritRateB);            
        }

        public void PopulateBattleSequenceUIValues(int sideIdx, UnitManager unit, int attack, bool canDoubleAttack, int hitRate, int critRate)
        {
            _battleSequenceSideManagers[sideIdx].PopulateBattleSequenceUIData(unit, attack, hitRate, critRate);
        }
    }
}
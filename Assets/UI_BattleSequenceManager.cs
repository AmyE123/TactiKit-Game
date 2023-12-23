namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_BattleSequenceManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleSequenceSideManager[] _battleSequenceSideManagers;

        [SerializeField] private BattleManager _battleManager;

        public void GetValuesForBattleSequenceUI(UnitManager unitA, UnitManager unitB)
        {           
            PopulateBattleSequenceUIValues(0, unitA, _battleManager.AttackerAtk, _battleManager.AttackerDblAtk, _battleManager.AttackerHit, _battleManager.AttackerCrit);
            PopulateBattleSequenceUIValues(1, unitB, _battleManager.DefenderAtk, _battleManager.DefenderDblAtk, _battleManager.DefenderHit, _battleManager.DefenderCrit);            
        }

        public void PopulateBattleSequenceUIValues(int sideIdx, UnitManager unit, int attack, bool canDoubleAttack, int hitRate, int critRate)
        {
            _battleSequenceSideManagers[sideIdx].PopulateBattleSequenceUIData(unit, attack, hitRate, critRate);
        }
    }
}
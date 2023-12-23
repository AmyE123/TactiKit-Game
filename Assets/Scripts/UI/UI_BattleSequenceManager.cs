namespace CT6GAMAI
{
    using UnityEngine;

    /// <summary>
    /// Manages the UI elements for displaying battle sequence information.
    /// </summary>
    public class UI_BattleSequenceManager : MonoBehaviour
    {
        [SerializeField] private UI_BattleSequenceSideManager[] _battleSequenceSideManagers;
        [SerializeField] private BattleManager _battleManager;

        /// <summary>
        /// Gets and populates battle data for the battle sequence UI for both units involved in the battle.
        /// </summary>
        /// <param name="attacker">The attacking unit.</param>
        /// <param name="defender">The defending unit.</param>
        public void GetValuesForBattleSequenceUI(UnitManager attacker, UnitManager defender)
        {           
            PopulateBattleSequenceUIValues(0, attacker, _battleManager.AttackerAtk, _battleManager.AttackerDblAtk, _battleManager.AttackerHit, _battleManager.AttackerCrit);
            PopulateBattleSequenceUIValues(1, defender, _battleManager.DefenderAtk, _battleManager.DefenderDblAtk, _battleManager.DefenderHit, _battleManager.DefenderCrit);            
        }

        /// <summary>
        /// Populates the battle sequence UI for a specific side with data from a unit.
        /// </summary>
        public void PopulateBattleSequenceUIValues(int sideIdx, UnitManager unit, int attack, bool canDoubleAttack, int hitRate, int critRate)
        {
            _battleSequenceSideManagers[sideIdx].PopulateBattleSequenceUIData(unit, attack, hitRate, critRate);
        }
    }
}
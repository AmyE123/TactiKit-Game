namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
    using System.ComponentModel;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// This class manages everything animation/visual for battle sequences
    /// </summary>
    public class BattleSequenceManager : MonoBehaviour
    {
        [Header("Battle State")]
        [SerializeField] private BattleSequenceStates _currentBattleState = BattleSequenceStates.PreBattle;
        [SerializeField] private Team _initiatingTeam;
        
        [Header("Units")]
        [SerializeField] private BattleUnitManager _attackerUnit;
        [SerializeField] private BattleUnitManager _defenderUnit;

        [Header("Positions - Attack")]
        [SerializeField] private Transform _attackPositionLeft;
        [SerializeField] private Transform _attackPositionRight;

        [Header("Positions - Dodge")]
        [SerializeField] private Transform _dodgePositionLeft;
        [SerializeField] private Transform _dodgePositionRight;

        [Header("Sequence Turn Management")]
        [SerializeField] private bool _attackerTakenTurn;
        [SerializeField] private bool _defenderTakenTurn;

        private bool _isBattleEnding = false;
        private GameManager _gameManager;

        private void Start()
        {
            InitializeValues();
        }

        private void InitializeValues()
        {
            _gameManager = GameManager.Instance;
            _currentBattleState = BattleSequenceStates.PreBattle;
        }

        private void ProcessBattleState()
        {
            if (_isBattleEnding && _currentBattleState != BattleSequenceStates.BattleEnd)
            {
                return;
            }

            switch (_currentBattleState)
            {
                case BattleSequenceStates.PreBattle:
                    HandlePreBattle();
                    break;

                case BattleSequenceStates.AttackerMoveForward:
                    HandleAttackerMoveForward();
                    break;

                case BattleSequenceStates.AttackerAttack:
                    HandleAttackerAttack();
                    break;

                case BattleSequenceStates.AttackerMoveBack:
                    HandleAttackerMoveBack();
                    break;

                case BattleSequenceStates.DefenderMoveForward:
                    HandleDefenderMoveForward();
                    break;

                case BattleSequenceStates.DefenderAttack:
                    HandleDefenderAttack();
                    break;

                case BattleSequenceStates.DefenderMoveBack:
                    HandleDefenderMoveBack();
                    break;

                case BattleSequenceStates.CheckAdditionalAttacks:
                    HandleCheckAdditionalAttacks();
                    break;

                case BattleSequenceStates.BattleEnd:
                    EndBattle();
                    return;
            }
        }

        #region Battle State Handler Functions

        private void HandlePreBattle()
        {
            StartCoroutine(BattleBeginDelay(BATTLE_SEQUENCE_DELAY, _initiatingTeam));
        }

        private void HandleAttackerMoveForward()
        {
            MoveUnitForward(_attackerUnit);
        }

        private void HandleAttackerAttack()
        {
            if (!_attackerTakenTurn)
            {
                AttackSequence(_attackerUnit, _defenderUnit);
                StartCoroutine(AttackDelay(BATTLE_SEQUENCE_DELAY, Side.Left));
            }
        }

        private void HandleAttackerMoveBack()
        {
            MoveUnitsBack(_attackerUnit, _defenderUnit);
            StartCoroutine(SwitchSidesDelay(BATTLE_SEQUENCE_DELAY));
        }

        private void HandleDefenderMoveForward()
        {
            MoveUnitForward(_defenderUnit);
        }

        private void HandleDefenderAttack()
        {
            if (!_defenderTakenTurn)
            {
                AttackSequence(_defenderUnit, _attackerUnit);
                StartCoroutine(AttackDelay(BATTLE_SEQUENCE_DELAY, Side.Right));
            }
        }

        private void HandleDefenderMoveBack()
        {
            MoveUnitsBack(_defenderUnit, _attackerUnit);
            StartCoroutine(SwitchSidesDelay(BATTLE_SEQUENCE_DELAY));
        }

        private void HandleCheckAdditionalAttacks()
        {
            if (_attackerUnit.CanUnitAttackAgain && !_attackerUnit.UnitCompleteAttacks)
            {
                ResetAttacker();
            }
            else if (_defenderUnit.CanUnitAttackAgain && !_defenderUnit.UnitCompleteAttacks)
            {
                ResetDefender();
            }
            else
            {
                EndBattleSequence();
            }
        }

        #endregion // Battle State Handler Functions

        private void ChangeBattleSequenceState(BattleSequenceStates newState)
        {
            _currentBattleState = newState;
            ProcessBattleState();
        }

        private void ResetAttacker()
        {
            _attackerTakenTurn = false;
            _attackerUnit.SetUnitCompleteAttacks(true);

            ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveForward);
        }

        private void ResetDefender()
        {
            _defenderTakenTurn = false;
            _defenderUnit.SetUnitCompleteAttacks(true);

            ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveForward);
        }

        private void EndBattleSequence()
        {
            _attackerUnit.SetUnitCompleteAttacks(true);
            _defenderUnit.SetUnitCompleteAttacks(true);

            ChangeBattleSequenceState(BattleSequenceStates.BattleEnd);
        }

        private void CheckForDeadUnits(BattleUnitManager defendingUnit)
        {
            if (defendingUnit.UnitStatsManager.CheckHealthState() == UnitHealthState.Dead)
            {
                StartCoroutine(HandleUnitDeath());
            }
        }

        IEnumerator HandleUnitDeath()
        {
            _isBattleEnding = true;

            // Wait for a second or two before ending the battle
            yield return new WaitForSeconds(2f);

            ChangeBattleSequenceState(BattleSequenceStates.BattleEnd);
        }

        private void MoveUnitForward(BattleUnitManager unit)
        {
            if (unit.UnitSide == Side.Left)
            {
                unit.transform.DOMoveX(_attackPositionRight.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                //_currentBattleState = BattleSequenceStates.Attacker_Attack;
                ChangeBattleSequenceState(BattleSequenceStates.AttackerAttack);
            }
            else
            {
                unit.transform.DOMoveX(_attackPositionLeft.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                //_currentBattleState = BattleSequenceStates.Defender_Attack;
                ChangeBattleSequenceState(BattleSequenceStates.DefenderAttack);
            }

            //ProcessBattleState();
        }

        private void PlayAttackAnimation(BattleUnitManager attackingUnit)
        {
            attackingUnit.Animator.SetInteger(ATTACKING_ANIM_IDX_PARAM, Random.Range(1, 4));
            attackingUnit.Animator.SetTrigger(ATTACKING_ANIM_PARAM);
        }

        private void PlayDamageAnimation(BattleUnitManager defendingUnit)
        {
            if (defendingUnit.UnitStatsManager.CheckHealthState() != UnitHealthState.Dead)
            {
                defendingUnit.Animator.SetInteger(HIT_ANIM_IDX_PARAM, Random.Range(1, 2));
                defendingUnit.Animator.SetTrigger(HIT_ANIM_PARAM);
            }
            else
            {
                defendingUnit.Animator.SetInteger(DEAD_ANIM_IDX_PARAM, Random.Range(1, 2));
                defendingUnit.Animator.SetBool(DEAD_ANIM_PARAM, true);

                StartCoroutine(DeathDelay(2f));
            }
        }

        private void PlayDodgeAnimation(BattleUnitManager defendingUnit)
        {
            defendingUnit.Animator.SetTrigger(DODGE_ANIM_PARAM);

            float jumpHeight = 0.2f;

            if (defendingUnit.UnitSide == Side.Left)
            {
                defendingUnit.transform.DOJump(
                new Vector3(_dodgePositionLeft.position.x, defendingUnit.transform.position.y, defendingUnit.transform.position.z),
                jumpHeight,
                1,
                BATTLE_SEQUENCE_MOVEMENT_SPEED + 0.2f
                );

            }
            else
            {
                defendingUnit.transform.DOJump(
                new Vector3(_dodgePositionRight.position.x, defendingUnit.transform.position.y, defendingUnit.transform.position.z),
                jumpHeight,
                1,
                BATTLE_SEQUENCE_MOVEMENT_SPEED + 0.2f
                );

            }
        }

        private void AttackSequence(BattleUnitManager attackingUnit, BattleUnitManager defendingUnit)
        {
            PlayAttackAnimation(attackingUnit);

            if (DoesUnitHit(attackingUnit))
            {
                ApplyAttackDamage(attackingUnit, defendingUnit);
                PlayDamageAnimation(defendingUnit);

                // Check if the defending unit is dead right after an attack
                CheckForDeadUnits(defendingUnit);
            }
            else
            {
                Debug.Log("[BATTLE]: Unit doesn't Hit!!");
                PlayDodgeAnimation(defendingUnit);
            }
        }

        private bool DoesUnitHit(BattleUnitManager attackingUnit)
        {
            var hit = attackingUnit.UnitStatsManager.Hit;
            var doesHit = BattleCalculator.HitRoll(hit);

            return doesHit;
        }

        private bool DoesUnitCrit(BattleUnitManager attackingUnit)
        {
            var crit = attackingUnit.UnitStatsManager.Crit;
            var doesCrit = BattleCalculator.CriticalRoll(crit);

            return doesCrit;
        }

        private void ApplyAttackDamage(BattleUnitManager attackingUnit, BattleUnitManager defendingUnit)
        {
            var attackDeduction = -attackingUnit.UnitStatsManager.Atk;

            if (DoesUnitCrit(attackingUnit))
            {
                Debug.Log("[BATTLE]: Unit does Crit!!");
                attackDeduction *= 3;
            }

            defendingUnit.UnitStatsManager.AdjustHealthPoints(attackDeduction);
        }

        IEnumerator BattleBeginDelay(float delaySeconds, Team initiator)
        {
            Debug.Log("[BATTLE]: Starting!!");
            yield return new WaitForSeconds(delaySeconds);

            if (initiator == Team.Player)
            {
                ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveForward);
            }
            else
            {
                ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveForward);
            }
        }

        IEnumerator AttackDelay(float delaySeconds, Side side)
        {
            Debug.Log("[BATTLE]: Attacking!!");
            yield return new WaitForSeconds(delaySeconds);

            if (_currentBattleState != BattleSequenceStates.BattleEnd)
            {
                if (side == Side.Left)
                {
                    ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveBack);
                }
                else
                {
                    ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveBack);
                }
            }
        }

        IEnumerator DeathDelay(float delaySeconds)
        {
            Debug.Log("[BATTLE]: Unit death!!");
            yield return new WaitForSeconds(delaySeconds);

            ChangeBattleSequenceState(BattleSequenceStates.BattleEnd);

        }

        private void MoveUnitsBack(BattleUnitManager unitA, BattleUnitManager unitB)
        {
            if (unitA.UnitSide == Side.Left)
            {
                unitA.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _attackerTakenTurn = true;
            }
            else
            {
                unitA.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _defenderTakenTurn = true;
            }
        }

        IEnumerator SwitchSidesDelay(float delaySeconds)
        {
            Debug.Log("[BATTLE]: Switching sides!!");
            yield return new WaitForSeconds(delaySeconds);

            if (_initiatingTeam == Team.Player && !_defenderTakenTurn)
            {
                ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveForward);
            }
            else if (_initiatingTeam == Team.Enemy && !_attackerTakenTurn)
            {
                ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveForward);
            }

            if (_defenderTakenTurn && _attackerTakenTurn)
            {
                ChangeBattleSequenceState(BattleSequenceStates.CheckAdditionalAttacks);
            }
        }

        private void EndBattle()
        {
            Debug.Log("[BATTLE]: End the battle");

            ResetBattle();
            _gameManager.BattleManager.SwitchToMap();

        }

        /// <summary>
        /// Starts a battle sequence between two units.
        /// </summary>
        /// <param name="attacker">The attacker/initiator of the battle.</param>
        /// <param name="defender">The defender/opponent of the battle.</param>
        public void StartBattle(UnitManager attacker, UnitManager defender)
        {
            _initiatingTeam = attacker.UnitData.UnitTeam;
            _gameManager.UIManager.BattleSequenceManager.GetValuesForBattleSequenceUI(attacker, defender);

            _attackerUnit.SetUnitReferences(attacker.UnitStatsManager, attacker);
            _defenderUnit.SetUnitReferences(defender.UnitStatsManager, defender);

            ProcessBattleState();
        }

        public void ResetBattle()
        {
            _isBattleEnding = false;

            _currentBattleState = BattleSequenceStates.PreBattle;
            _attackerUnit.SetUnitCompleteAttacks(false);
            _defenderUnit.SetUnitCompleteAttacks(false);
            _defenderTakenTurn = false;
            _attackerTakenTurn = false;
        }
    }
}
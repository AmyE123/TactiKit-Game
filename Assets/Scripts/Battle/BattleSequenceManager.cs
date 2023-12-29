namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
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
            MoveUnitsBackToOriginalPos(_attackerUnit, _defenderUnit);
            StartCoroutine(BattleBeginDelay(BATTLE_SEQUENCE_DELAY));
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

        private void MoveUnitForward(BattleUnitManager unit)
        {
            if (unit.UnitSide == Side.Left)
            {
                unit.transform.DOMoveX(_attackPositionRight.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                ChangeBattleSequenceState(BattleSequenceStates.AttackerAttack);
            }
            else
            {
                unit.transform.DOMoveX(_attackPositionLeft.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                ChangeBattleSequenceState(BattleSequenceStates.DefenderAttack);
            }
        }

        private void TriggerAttackAnimation(BattleUnitManager attackingUnit)
        {
            attackingUnit.Animator.SetInteger(ATTACKING_ANIM_IDX_PARAM, Random.Range(1, ATTACKING_ANIM_IDX_COUNT));
            attackingUnit.Animator.SetTrigger(ATTACKING_ANIM_PARAM);
        }

        private void TriggerDamageAnimation(BattleUnitManager defendingUnit)
        {
            if (defendingUnit.UnitStatsManager.CheckHealthState() != UnitHealthState.Dead)
            {
                defendingUnit.Animator.SetInteger(HIT_ANIM_IDX_PARAM, Random.Range(1, HIT_ANIM_IDX_COUNT));
                defendingUnit.Animator.SetTrigger(HIT_ANIM_PARAM);
            }
            else
            {
                defendingUnit.Animator.SetInteger(DEAD_ANIM_IDX_PARAM, Random.Range(1, DEAD_ANIM_IDX_COUNT));
                defendingUnit.Animator.SetBool(DEAD_ANIM_PARAM, true);

                StartCoroutine(DeathDelay(BATTLE_SEQUENCE_DEATH_DELAY));
            }
        }

        private void TriggerDodgeAnimation(BattleUnitManager unit)
        {
            unit.Animator.SetTrigger(DODGE_ANIM_PARAM);

            if (unit.UnitSide == Side.Left)
            {
                DodgeAnimation(_dodgePositionLeft, unit);
            }
            else
            {
                DodgeAnimation(_dodgePositionRight, unit);
            }
        }

        private void DodgeAnimation(Transform dodgeTransform, BattleUnitManager unit)
        {
            unit.transform.DOJump(
            new Vector3(dodgeTransform.position.x, unit.transform.position.y, unit.transform.position.z),
            DODGE_TWEEN_JUMP_HEIGHT,
            1,
            BATTLE_SEQUENCE_MOVEMENT_SPEED * 2
            );
        }

        private void AttackSequence(BattleUnitManager attackingUnit, BattleUnitManager defendingUnit)
        {
            TriggerAttackAnimation(attackingUnit);

            if (DoesUnitHit(attackingUnit))
            {
                ApplyAttackDamage(attackingUnit, defendingUnit);
                TriggerDamageAnimation(defendingUnit);
                CheckForDeadUnits(defendingUnit);
            }
            else
            {
                TriggerDodgeAnimation(defendingUnit);
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
            var attackPower = attackingUnit.UnitStatsManager.Atk;

            if (DoesUnitCrit(attackingUnit))
            {
                attackPower *= CRIT_HIT_MULTIPLIER;
            }

            defendingUnit.UnitStatsManager.AdjustHealthPoints(-attackPower);
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

        private void MoveUnitsBackToOriginalPos(BattleUnitManager unitA, BattleUnitManager unitB)
        {
            if (unitA.UnitSide == Side.Left)
            {
                unitA.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
            }
            else
            {
                unitA.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
            }
        }

        private void EndBattle()
        {
            ResetBattle();
            _gameManager.BattleManager.SwitchToMap();

        }

        private IEnumerator BattleBeginDelay(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);

            if (_initiatingTeam == Team.Player)
            {
                ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveForward);
            }
            else
            {
                ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveForward);
            }
        }

        private IEnumerator AttackDelay(float delaySeconds, Side side)
        {
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

        private IEnumerator HandleUnitDeath()
        {
            _isBattleEnding = true;

            yield return new WaitForSeconds(BATTLE_SEQUENCE_DEATH_DELAY);

            ChangeBattleSequenceState(BattleSequenceStates.BattleEnd);
        }

        private IEnumerator DeathDelay(float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);

            _gameManager.TurnManager.TurnMusicManager.ResumeLastMusic();

            ChangeBattleSequenceState(BattleSequenceStates.BattleEnd);
        }

        private IEnumerator SwitchSidesDelay(float delaySeconds)
        {
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

        /// <summary>
        /// A reset for when the attacker initiates a double attack.
        /// </summary>
        private void ResetAttacker()
        {
            _attackerTakenTurn = false;
            _attackerUnit.SetUnitCompleteAttacks(true);

            ChangeBattleSequenceState(BattleSequenceStates.AttackerMoveForward);
        }

        /// <summary>
        /// A reset for when the defender initiates a double attack.
        /// </summary>
        private void ResetDefender()
        {
            _defenderTakenTurn = false;
            _defenderUnit.SetUnitCompleteAttacks(true);

            ChangeBattleSequenceState(BattleSequenceStates.DefenderMoveForward);
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

        /// <summary>
        /// Reset the battle so it is ready for the next one.
        /// </summary>
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
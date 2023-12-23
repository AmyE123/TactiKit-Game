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
        [SerializeField] private BattleUnitManager _leftUnit;
        [SerializeField] private BattleUnitManager _rightUnit;

        [Header("Positions - Attack")]
        [SerializeField] private Transform _attackPositionLeft;
        [SerializeField] private Transform _attackPositionRight;

        [Header("Positions - Dodge")]
        [SerializeField] private Transform _dodgePositionLeft;
        [SerializeField] private Transform _dodgePositionRight;

        [Header("Sequence Turn Management")]
        [SerializeField] private bool _leftTakenTurn;
        [SerializeField] private bool _rightTakenTurn;

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

                case BattleSequenceStates.PlayerMoveForward:
                    HandlePlayerMoveForward();
                    break;

                case BattleSequenceStates.PlayerAttack:
                    HandlePlayerAttack();
                    break;

                case BattleSequenceStates.PlayerMoveBack:
                    HandlePlayerMoveBack();
                    break;

                case BattleSequenceStates.EnemyMoveForward:
                    HandleEnemyMoveForward();
                    break;

                case BattleSequenceStates.EnemyAttack:
                    HandleEnemyAttack();
                    break;

                case BattleSequenceStates.EnemyMoveBack:
                    HandleEnemyMoveBack();
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
            StartCoroutine(BattleBeginDelay(1f, _initiatingTeam));
        }

        private void HandlePlayerMoveForward()
        {
            MoveUnitForward(_leftUnit);
        }

        private void HandlePlayerAttack()
        {
            if (!_leftTakenTurn)
            {
                AttackSequence(_leftUnit, _rightUnit);
                StartCoroutine(AttackDelay(1f, Side.Left));
            }
        }

        private void HandlePlayerMoveBack()
        {
            MoveUnitsBack(_leftUnit, _rightUnit);
            StartCoroutine(SwitchSidesDelay(1f));
        }

        private void HandleEnemyMoveForward()
        {
            MoveUnitForward(_rightUnit);
        }

        private void HandleEnemyAttack()
        {
            if (!_rightTakenTurn)
            {
                AttackSequence(_rightUnit, _leftUnit);
                StartCoroutine(AttackDelay(1f, Side.Right));
            }
        }

        private void HandleEnemyMoveBack()
        {
            MoveUnitsBack(_rightUnit, _leftUnit);
            StartCoroutine(SwitchSidesDelay(1f));
        }

        private void HandleCheckAdditionalAttacks()
        {
            if (_leftUnit.CanUnitAttackAgain && !_leftUnit.UnitCompleteAttacks)
            {
                _leftTakenTurn = false;
                _leftUnit.SetUnitCompleteAttacks(true);
                _currentBattleState = BattleSequenceStates.PlayerMoveForward;
                ProcessBattleState();
            }
            else if (_rightUnit.CanUnitAttackAgain && !_rightUnit.UnitCompleteAttacks)
            {
                _rightTakenTurn = false;
                _rightUnit.SetUnitCompleteAttacks(true);
                _currentBattleState = BattleSequenceStates.EnemyMoveForward;
                ProcessBattleState();
            }
            else
            {
                _leftUnit.SetUnitCompleteAttacks(true);
                _rightUnit.SetUnitCompleteAttacks(true);
                _currentBattleState = BattleSequenceStates.BattleEnd;
                ProcessBattleState();
            }
        }

        #endregion // Battle State Handler Functions


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

            _currentBattleState = BattleSequenceStates.BattleEnd;
            ProcessBattleState();
        }

        // Methods for MoveUnitForward, PlayAttackAnimation, EndBattle, etc., go here

        private void MoveUnitForward(BattleUnitManager unit)
        {
            if (unit.UnitSide == Side.Left)
            {
                unit.transform.DOMoveX(_attackPositionRight.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _currentBattleState = BattleSequenceStates.PlayerAttack;
            }
            else
            {
                unit.transform.DOMoveX(_attackPositionLeft.position.x, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _currentBattleState = BattleSequenceStates.EnemyAttack;
            }

            ProcessBattleState();
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
                _currentBattleState = BattleSequenceStates.PlayerMoveForward;
                ProcessBattleState();
            }
            else
            {
                _currentBattleState = BattleSequenceStates.EnemyMoveForward;
                ProcessBattleState();
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
                    _currentBattleState = BattleSequenceStates.PlayerMoveBack;
                    ProcessBattleState();
                }
                else
                {
                    _currentBattleState = BattleSequenceStates.EnemyMoveBack;
                    ProcessBattleState();
                }
            }
        }

        IEnumerator DeathDelay(float delaySeconds)
        {
            Debug.Log("[BATTLE]: Unit death!!");
            yield return new WaitForSeconds(delaySeconds);

            _currentBattleState = BattleSequenceStates.BattleEnd;
            ProcessBattleState();

        }

        private void MoveUnitBack(BattleUnitManager unit)
        {
            if (unit.UnitSide == Side.Left)
            {
                unit.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _leftTakenTurn = true;
            }
            else
            {
                unit.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _rightTakenTurn = true;
            }
        }

        private void MoveUnitsBack(BattleUnitManager unitA, BattleUnitManager unitB)
        {
            if (unitA.UnitSide == Side.Left)
            {
                unitA.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _leftTakenTurn = true;
            }
            else
            {
                unitA.transform.DOMoveX(-1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                unitB.transform.DOMoveX(1, BATTLE_SEQUENCE_MOVEMENT_SPEED);
                _rightTakenTurn = true;
            }
        }

        IEnumerator SwitchSidesDelay(float delaySeconds)
        {
            Debug.Log("[BATTLE]: Switching sides!!");
            yield return new WaitForSeconds(delaySeconds);

            if (_initiatingTeam == Team.Player && !_rightTakenTurn)
            {
                _currentBattleState = BattleSequenceStates.EnemyMoveForward;
                ProcessBattleState();
            }
            else if (_initiatingTeam == Team.Enemy && !_leftTakenTurn)
            {
                _currentBattleState = BattleSequenceStates.PlayerMoveForward;
                ProcessBattleState();
            }

            if (_rightTakenTurn && _leftTakenTurn)
            {
                _currentBattleState = BattleSequenceStates.CheckAdditionalAttacks;
                ProcessBattleState();
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

            _leftUnit.SetUnitReferences(attacker.UnitStatsManager, attacker);
            _rightUnit.SetUnitReferences(defender.UnitStatsManager, defender);

            ProcessBattleState();
        }

        public void ResetBattle()
        {
            _isBattleEnding = false;

            _currentBattleState = BattleSequenceStates.PreBattle;
            _leftUnit.SetUnitCompleteAttacks(false);
            _rightUnit.SetUnitCompleteAttacks(false);
            _rightTakenTurn = false;
            _leftTakenTurn = false;
        }
    }
}
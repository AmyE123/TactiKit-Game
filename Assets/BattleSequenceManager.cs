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
        [SerializeField] private BattleSequenceStates _currentBattleState = BattleSequenceStates.PreBattle;
        [SerializeField] private Team _initiatingTeam;

        /// <summary>
        /// Left unit is the player Unit
        /// </summary>
        [SerializeField] private BattleUnitManager _leftUnit;

        /// <summary>
        /// Right unit is the enemy Unit
        /// </summary>
        [SerializeField] private BattleUnitManager _rightUnit;

        [SerializeField] private Transform _attackPositionLeft;
        [SerializeField] private Transform _attackPositionRight;

        [SerializeField] private Transform _dodgePositionLeft;
        [SerializeField] private Transform _dodgePositionRight;

        [SerializeField] private bool _battleBegin = false;
        [SerializeField] private bool _leftTakenTurn;
        [SerializeField] private bool _rightTakenTurn;

        private GameManager _gameManager;
        private BattleManager _battleManager;

        public void Start()
        {
            _gameManager = GameManager.Instance;
            _battleManager = _gameManager.BattleManager;
            _currentBattleState = BattleSequenceStates.PreBattle;
        }

        public void StartBattle(Team initiator, UnitManager unitA, UnitManager unitB)
        {
            _initiatingTeam = initiator;
            _gameManager.UIManager.BattleSequenceManager.GetValuesForBattleSequenceUI(unitA, unitB);

            _leftUnit.SetUnitReferences(unitA.UnitStatsManager, unitA);
            _rightUnit.SetUnitReferences(unitB.UnitStatsManager, unitB);

            ProcessBattleState();       
        }

        private void ProcessBattleState()
        {
            switch (_currentBattleState)
            {
                case BattleSequenceStates.PreBattle:
                    StartCoroutine(BattleBeginDelay(1f, _initiatingTeam));
                    break;

                case BattleSequenceStates.PlayerMoveForward:
                    MoveUnitForward(_leftUnit);
                    break;

                case BattleSequenceStates.PlayerAttack:
                    if (!_leftTakenTurn)
                    {
                        AttackSequence(_leftUnit, _rightUnit);
                        StartCoroutine(AttackDelay(1f, Side.Left));
                    }
                    break;

                case BattleSequenceStates.PlayerMoveBack:
                    MoveUnitsBack(_leftUnit, _rightUnit);
                    StartCoroutine(SwitchSidesDelay(1f));
                    break;

                case BattleSequenceStates.EnemyMoveForward:
                    MoveUnitForward(_rightUnit);
                    break;

                case BattleSequenceStates.EnemyAttack:
                    if (!_rightTakenTurn)
                    {
                        AttackSequence(_rightUnit, _leftUnit);
                        StartCoroutine(AttackDelay(1f, Side.Right));
                    }
                    break;

                case BattleSequenceStates.EnemyMoveBack:
                    MoveUnitsBack(_rightUnit, _leftUnit);
                    StartCoroutine(SwitchSidesDelay(1f));
                    break;     
                    
                case BattleSequenceStates.CheckAdditionalAttacks:
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
                    break;

                case BattleSequenceStates.BattleEnd:
                    EndBattle();
                    return;
            }

            // Call this method again after a delay or at the end of an animation event
            //Invoke("ProcessBattleState", BATTLE_SEQUENCE_ANIM_DELAY);
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

        private void PlayAttackAnimation(BattleUnitManager attackingUnit, BattleUnitManager defendingUnit)
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
            PlayAttackAnimation(attackingUnit, defendingUnit);

            if (DoesUnitHit(attackingUnit))
            {
                ApplyAttackDamage(attackingUnit, defendingUnit);
                PlayDamageAnimation(defendingUnit);
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
            //CheckForOpponentDeath(defendingUnit);
        }

        //private void CheckForOpponentDeath(BattleUnitManager defendingUnit)
        //{
        //    if (defendingUnit.UnitStatsManager.CheckHealthState() == UnitHealthState.Dead)
        //    {
        //        defendingUnit.Animator.SetInteger(DEAD_ANIM_IDX_PARAM, Random.Range(1, 2));
        //        defendingUnit.Animator.SetBool(DEAD_ANIM_PARAM, true);

        //        StartCoroutine(DeathDelay(2f));
        //    }
        //}

        IEnumerator BattleBeginDelay(float delaySeconds, Team initiator)
        {
            Debug.Log("[BATTLE]: Starting!!");
            yield return new WaitForSeconds(delaySeconds);

            _battleBegin = true;

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
            _gameManager.BattleManager.SwitchToMap();
            ResetBattle();
        }

        private void ResetBattle()
        {
            _currentBattleState = BattleSequenceStates.PreBattle;
            _battleBegin = false;
            _leftUnit.SetUnitCompleteAttacks(false);
            _rightUnit.SetUnitCompleteAttacks(false);
            _rightTakenTurn = false; 
            _leftTakenTurn = false;
        }
    }
}
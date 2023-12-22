namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Collections;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class BattleUnitManager : MonoBehaviour
    {
        public enum Side { Left, Right };

        [SerializeField] private Side _unitSide;
        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _attackPositionLeft;
        [SerializeField] private Transform _attackPositionRight;

        [SerializeField] private bool _isUnitsTurn = false;

        private void Update()
        {
            if (_isUnitsTurn)
            {
                if (_unitSide == Side.Left)
                {
                    transform.DOMoveX(_attackPositionRight.position.x, 0.2f);
                }
                else
                {
                    transform.DOMoveX(_attackPositionLeft.position.x, 0.2f);
                }

                _animator.SetInteger(ATTACKING_ANIM_IDX_PARAM, Random.Range(1, 4));
                _animator.SetBool(ATTACKING_ANIM_PARAM, true);
                StartCoroutine(AnimationDelay());
            }
            if (!_isUnitsTurn)
            {
                _animator.SetBool(ATTACKING_ANIM_PARAM, false);
            }
        }

        IEnumerator AnimationDelay()
        {
            yield return new WaitForSeconds(IDLE_DELAY);
            _isUnitsTurn = false;
        }
    }
}
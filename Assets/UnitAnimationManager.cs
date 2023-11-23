namespace CT6GAMAI
{
    using UnityEngine;

    public class UnitAnimationManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;
        [SerializeField] private Animator _animator;

        private bool _warningLogSent = false;

        private void Update()
        {
            if (_animator != null)
            {
                UpdateAnimationParameters();
            }
            else
            {
                if(!_warningLogSent)
                {
                    TryForceAnimationComponentAttach();
                }
            }
        }

        private void TryForceAnimationComponentAttach()
        {
            _animator = GetComponentInChildren<Animator>();
            if (_animator != null)
            {
                Debug.LogWarning("[UNIT]: " + gameObject.name + " Has no Animator attached, please attach this in the inspector. " +
                    "Attached " + _animator.name + "'s animation in it's place.");
            }
            else
            {
                Debug.LogWarning("[UNIT]: " + gameObject.name + " Has no Animator component in children.");
            }
        }

        private void UpdateAnimationParameters()
        {
            _animator.SetBool("Moving", _unitManager.IsMoving);
        }
    }
}
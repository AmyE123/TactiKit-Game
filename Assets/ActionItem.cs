namespace CT6GAMAI
{
    using DG.Tweening;
    using UnityEngine;

    public interface IActionItem
    {
        public RectTransform Pointer { get; }
        public void ActionEvent();
        public void AnimatePointer();
    }

    public abstract class ActionItemBase : MonoBehaviour, IActionItem
    {
        public abstract RectTransform Pointer { get; }

        public void AnimatePointer()
        {
            Pointer.DOAnchorPosX(0, 0.5f).SetLoops(-1, LoopType.Yoyo);
        }

        public abstract void ActionEvent();
    }
}
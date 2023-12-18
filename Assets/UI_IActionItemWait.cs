namespace CT6GAMAI
{
    using UnityEngine;

    public class UI_IActionItemWait : ActionItemBase
    {
        [SerializeField] private RectTransform _pointer;

        public override RectTransform Pointer => _pointer;

        private void Start()
        {
            AnimatePointer();
        }

        public override void ActionEvent()
        {
            Debug.Log("Event Action");
        }
    }
}
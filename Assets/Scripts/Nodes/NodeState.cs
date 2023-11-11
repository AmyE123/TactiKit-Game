namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class NodeState : MonoBehaviour
    {
        [SerializeField] private NodeManager _nodeManager;

        RaycastHit nodeHit;

        public NodeStateVisualData[] VisualDatas;
        public NodeStateVisualData[] SelectorVisualDatas;

        public SpriteRenderer VisualSR;
        public SpriteRenderer SelectorSR;

        public GameObject PointerCanvas;
        public GameObject Selector;

        public NodeVisualState CurrentState = default;
        public bool IsSelected = false;
        public bool IsLocked = false;
        //public bool IsHighlighted = false;
        public bool IsBolded = false;
       
        public NodeVisualManager NodeVisualManager;
        public NodeSelectorManager NodeSelectorManager;

        void Update()
        {

        }

        public void ChangeVisualData(SpriteRenderer SR, NodeStateVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}
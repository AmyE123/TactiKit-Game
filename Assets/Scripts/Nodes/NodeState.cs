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
        public bool IsHighlighted = false;
        public bool IsBolded = false;
       
        public NodeVisualManager NodeVisualManager;
        public NodeSelectorManager NodeSelectorManager;

        void Update()
        {
            if (NodeSelectorManager.IsActiveSelection)
            {
                // TODO: Update with new visualFSM
                ChangeVisualData(SelectorSR, SelectorVisualDatas[0]);
            }
            else
            {
                // TODO: Update with new visualFSM
                CurrentState = NodeVisualState.Default;
                //NodeVisualManager.SetDefault();
            }


            if (IsLocked)
            {
                ChangeVisualData(SelectorSR, SelectorVisualDatas[1]);

                PointerCanvas.SetActive(false);
            }
            if (IsHighlighted)
            {
                CurrentState = NodeVisualState.HoveredBlue;
            }
            if (IsBolded)
            {
                CurrentState = NodeVisualState.PointOfInterest;
            }


            CheckState();
        }

        void CheckState()
        {
            switch (CurrentState)
            {
                case NodeVisualState.Default:
                    ChangeVisualData(VisualSR, VisualDatas[0]);
                    break;

                case NodeVisualState.HoveredBlue:
                    ChangeVisualData(VisualSR, VisualDatas[1]); 
                    break;

                case NodeVisualState.HoveredRed:
                    ChangeVisualData(VisualSR, VisualDatas[2]);
                    break;

                case NodeVisualState.HoveredGreen:
                    ChangeVisualData(VisualSR, VisualDatas[3]);
                    break;

                case NodeVisualState.SelectedBlue:
                    ChangeVisualData(VisualSR, VisualDatas[4]);
                    break;

                case NodeVisualState.SelectedRed:
                    ChangeVisualData(VisualSR, VisualDatas[5]);
                    break;

                case NodeVisualState.SelectedGreen:
                    ChangeVisualData(VisualSR, VisualDatas[6]);
                    break;

                case NodeVisualState.AllEnemyRange:
                    ChangeVisualData(VisualSR, VisualDatas[7]);
                    break;

                case NodeVisualState.SingularEnemyRange:
                    ChangeVisualData(VisualSR, VisualDatas[8]);
                    break;

                case NodeVisualState.PointOfInterest:
                    ChangeVisualData(VisualSR, VisualDatas[9]);
                    break;

                default:
                    break;
            }
        }

        public void ChangeVisualData(SpriteRenderer SR, NodeStateVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}
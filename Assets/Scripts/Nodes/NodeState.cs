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

        public State CurrentState = default;
        public bool IsSelected = false;
        public bool IsLocked = false;
        public bool IsHighlighted = false;
        public bool IsBolded = false;

        void Update()
        {
            if (IsSelected)
            {
                ChangeVisualData(SelectorSR, SelectorVisualDatas[0]);

                Selector.SetActive(true);
                PointerCanvas.SetActive(true);
            }
            if (IsLocked)
            {
                ChangeVisualData(SelectorSR, SelectorVisualDatas[1]);

                PointerCanvas.SetActive(false);
            }
            if (IsHighlighted)
            {
                CurrentState = State.HoveredBlue;
            }
            if (IsBolded)
            {
                CurrentState = State.PointOfInterest;
            }
            if (!IsSelected && !IsLocked && !IsHighlighted && !IsBolded)
            {
                CurrentState = State.Default;
                Selector.SetActive(false);
                PointerCanvas.gameObject.SetActive(false);
            }

            CheckState();
        }

        void CheckState()
        {
            switch (CurrentState)
            {
                case State.Default:
                    ChangeVisualData(VisualSR, VisualDatas[0]);
                    break;

                case State.HoveredBlue:
                    ChangeVisualData(VisualSR, VisualDatas[1]); 
                    break;

                case State.HoveredRed:
                    ChangeVisualData(VisualSR, VisualDatas[2]);
                    break;

                case State.HoveredGreen:
                    ChangeVisualData(VisualSR, VisualDatas[3]);
                    break;

                case State.SelectedBlue:
                    ChangeVisualData(VisualSR, VisualDatas[4]);
                    break;

                case State.SelectedRed:
                    ChangeVisualData(VisualSR, VisualDatas[5]);
                    break;

                case State.SelectedGreen:
                    ChangeVisualData(VisualSR, VisualDatas[6]);
                    break;

                case State.AllEnemyRange:
                    ChangeVisualData(VisualSR, VisualDatas[7]);
                    break;

                case State.SingularEnemyRange:
                    ChangeVisualData(VisualSR, VisualDatas[8]);
                    break;

                case State.PointOfInterest:
                    ChangeVisualData(VisualSR, VisualDatas[9]);
                    break;

                default:
                    break;
            }
        }

        void ChangeVisualData(SpriteRenderer SR, NodeStateVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}
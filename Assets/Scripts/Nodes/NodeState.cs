namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class NodeState : MonoBehaviour
    {       
        public NodeStateData VisualData;
        public SpriteRenderer SpriteRenderer;
        public Canvas PointerCanvas;

        public State CurrentState = default;
        public bool IsSelected = false;
        public bool IsLocked = false;

        RaycastHit nodeHit;

        //public NodeState NorthNodeState;
        //public NodeState EastNodeState;
        //public NodeState SouthNodeState;
        //public NodeState WestNodeState;        

        [SerializeField] private NodeManager _nodeManager;

        private void Start()
        {

        }

        void Update()
        {
            if (IsSelected)
            {
                CurrentState = State.Selector;
                PointerCanvas.gameObject.SetActive(true);
            }
            if (IsLocked)
            {
                CurrentState = State.ConfirmedSelected;
                PointerCanvas.gameObject.SetActive(false);
            }
            if(!IsSelected && !IsLocked)
            {
                CurrentState = State.Default;
                PointerCanvas.gameObject.SetActive(false);
            }

            CheckState();
        }


        NodeState CheckForNeighbourNode(Vector3 Direction)
        {
            if (Physics.Raycast(transform.position, Direction, out nodeHit, 1))
            {
                if (nodeHit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
                {                    
                    var NS = nodeHit.transform.parent.GetComponent<NodeState>();

                    Debug.Log("SUCCESS: Found NodeState");

                    return NS;
                }
                else
                {
                    Debug.Log("ERROR: Cast hit non-node object");
                    return null;
                }
            }
            else
            {
                Debug.Log("ERROR: Cast hit nothing");
                return null;
            }
        }

        void CheckState()
        {
            switch (CurrentState)
            {
                case State.Default:
                    ChangeToDefault();
                    break;

                case State.Selector:
                    ChangeToSelector();
                    break;

                case State.ConfirmedSelected:
                    ChangeToConfirmedSelected();
                    break;

                case State.Selected:
                    ChangeToSelected();
                    break;

                case State.Pathway:
                    ChangeToPathway();
                    break;

                default:
                    break;
            }
        }

        void ChangeToDefault()
        {
            SpriteRenderer.material = VisualData.DefaultMaterial;
            SpriteRenderer.color = VisualData.DefaultColor;
            SpriteRenderer.sprite = VisualData.DefaultSprite;
        }

        void ChangeToSelected()
        {
            SpriteRenderer.material = VisualData.SelectedMaterial;
            SpriteRenderer.color = VisualData.SelectedColor;
            SpriteRenderer.sprite = VisualData.SelectedSprite;
        }

        void ChangeToConfirmedSelected()
        {
            SpriteRenderer.material = VisualData.ConfirmedSelectedMaterial;
            SpriteRenderer.color = VisualData.ConfirmedSelectedColor;
            SpriteRenderer.sprite = VisualData.ConfirmedSelectedSprite;
        }


        void ChangeToSelector()
        {
            SpriteRenderer.material = VisualData.SelectorMaterial;
            SpriteRenderer.color = VisualData.SelectorColor;
            SpriteRenderer.sprite = VisualData.SelectorSprite;
        }

        void ChangeToPathway()
        {
            SpriteRenderer.material = VisualData.PathwayMaterial;
            SpriteRenderer.color = VisualData.PathwayColor;
            SpriteRenderer.sprite = VisualData.PathwaySprite;
        }
    }
}
namespace CT6GAMAI
{
    using Unity.Burst.CompilerServices;
    using UnityEngine;

    public class NodeManager : MonoBehaviour
    {
        #region Editor Fields
        [Header("Temp Params")]
        // TODO: Find a new way to implement this
        public MovementRange _movementRange;

        [Header("Node Information")]
        [SerializeField] private NodeState _nodeState;
        [SerializeField] private NodeData _nodeData;
        [SerializeField] private Node _node;

        // If valid, the unit which is stood within the node.
        public UnitManager StoodUnit;

        [Header("Neighbouring Nodes")]
        [SerializeField] private NodeManager _northNode;
        [SerializeField] private NodeManager _eastNode;
        [SerializeField] private NodeManager _southNode;
        [SerializeField] private NodeManager _westNode;

        [SerializeField] private NodeManager _northEastNode;
        [SerializeField] private NodeManager _northWestNode;
        [SerializeField] private NodeManager _southEastNode;
        [SerializeField] private NodeManager _southWestNode;
        #endregion // Editor Fields

        #region Private

        private RaycastHit nodeHit;
        private RaycastHit unitHit;

        #endregion //Private

        public bool NodeInitialized = false;

        #region Public Getters

        public NodeManager NorthNode => _northNode;
        public NodeManager EastNode => _eastNode;
        public NodeManager SouthNode => _southNode;
        public NodeManager WestNode => _westNode;

        public NodeManager NorthEastNode => _northEastNode;
        public NodeManager NorthWestNode => _northWestNode;
        public NodeManager SouthEastNode => _southEastNode;
        public NodeManager SouthWestNode => _southWestNode;

        public NodeState NodeState => _nodeState;
        public NodeData NodeData => _nodeData;
        public Node Node => _node;

        #endregion // Public Getters

        void Start()
        {
            SetupNeighbours();
            _nodeState = GetComponent<NodeState>();

            // TODO: Find another way to deal with this
            _movementRange = FindObjectOfType<MovementRange>();

            Node.Cost = NodeData.TerrainType.MovementCost;

            StoodUnit = DetectStoodUnit();

            NodeInitialized = true;
        }

        public UnitManager DetectStoodUnit()
        {
            if (Physics.Raycast(transform.position, transform.up * 5, out unitHit, 1))
            {
                return GetUnitFromRayHit(unitHit);
            }
            else
            {
                return null;
            }
        }

        private UnitManager GetUnitFromRayHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.tag == Constants.UNIT_TAG_REFERENCE)
            {
                return unitHit.transform.GetComponentInParent<UnitManager>();
            }
            else
            {
                Debug.Log("[ERROR]: Cast hit non-unit object - " + unitHit.transform.gameObject.name);
                return null;
            }
        }

        void SetupNeighbours()
        {
            _northNode = CheckForNeighbourNode(-transform.forward);
            _westNode = CheckForNeighbourNode(transform.right);
            _southNode = CheckForNeighbourNode(transform.forward);
            _eastNode = CheckForNeighbourNode(-transform.right);

            _northEastNode = CheckForNeighbourNode(new Vector3(-0.5f, 0, -0.5f));
            _northWestNode = CheckForNeighbourNode(new Vector3(0.5f, 0, -0.5f));
            _southEastNode = CheckForNeighbourNode(new Vector3(-0.5f, 0, 0.5f));
            _southWestNode = CheckForNeighbourNode(new Vector3(0.5f, 0, 0.5f));


            // TODO: Cleanup this code
            if (_northNode != null)
            {
                Node.AddNeighbor(_northNode.Node);
            }
            if (_eastNode != null)
            {
                Node.AddNeighbor(_eastNode.Node);
            }
            if (_southNode != null)
            {
                Node.AddNeighbor(_southNode.Node);
            }
            if (_westNode != null)
            {
                Node.AddNeighbor(_westNode.Node);
            }
        }

        NodeManager CheckForNeighbourNode(Vector3 Direction)
        {
            if (Physics.Raycast(transform.position, Direction, out nodeHit, 1))
            {
                if (nodeHit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
                {
                    var NodeManager = nodeHit.transform.parent.GetComponent<NodeManager>();
                    return NodeManager;
                }
                else
                {
                    Debug.Log("ERROR: Cast hit non-node object - " + nodeHit.transform.gameObject.name);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Highlight the movement range of a Unit
        /// </summary>
        /// <param name="unit">The unit which we want to highlight the range of.</param>
        /// <param name="isPressed">Whether we are highlighting in a pressed state or not.</param>
        public void HighlightRangeArea(UnitManager unit, bool isPressed = false)
        {
            // Identify unit's movement range
            var range = unit.UnitData.MovementBaseValue;

            //_movementRange.CalculateMovementRange(Node, range);
            _movementRange.CalculateMovementRange(unit);

            for (int i = 0; i < _movementRange.ReachableNodes.Count; i++)
            {
                if (isPressed)
                {
                    _movementRange.ReachableNodes[i].NodeManager.NodeState.VisualStateManager.SetPressed(Constants.NodeVisualColorState.Blue);
                }
                else
                {
                    _movementRange.ReachableNodes[i].NodeManager.NodeState.VisualStateManager.SetHovered(Constants.NodeVisualColorState.Blue);
                }
            }
        }
    }
}

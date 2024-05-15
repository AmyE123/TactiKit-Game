namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the individual nodes within the grid, handling their state, data, and neighbor nodes.
    /// Also responsible for detecting units on the node and highlighting movement ranges.
    /// </summary>
    public class NodeManager : MonoBehaviour
    {
        #region Editor Fields
        [Header("Node Information")]
        [SerializeField] private NodeState _nodeState;
        [SerializeField] private NodeData _nodeData;
        [SerializeField] private Node _node;

        /// <summary>
        /// The unit that is currently positioned on this node, if any.
        /// </summary>
        public UnitManager StoodUnit;

        /// <summary>
        /// Indicates if the node has been initialized.
        /// </summary>
        public bool NodeInitialized = false;

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

        #region Public Getters

        /// <summary>
        /// The north neighboring node.
        /// </summary>
        public NodeManager NorthNode => _northNode;

        /// <summary>
        /// The east neighboring node.
        /// </summary>
        public NodeManager EastNode => _eastNode;

        /// <summary>
        /// The south neighboring node.
        /// </summary>
        public NodeManager SouthNode => _southNode;

        /// <summary>
        /// The west neighboring node.
        /// </summary>
        public NodeManager WestNode => _westNode;

        /// <summary>
        /// The north-east neighboring node.
        /// </summary>
        public NodeManager NorthEastNode => _northEastNode;

        /// <summary>
        /// The north-west neighboring node.
        /// </summary>
        public NodeManager NorthWestNode => _northWestNode;

        /// <summary>
        /// The south-east neighboring node.
        /// </summary>
        public NodeManager SouthEastNode => _southEastNode;

        /// <summary>
        /// The south-west neighboring node.
        /// </summary>
        public NodeManager SouthWestNode => _southWestNode;

        /// <summary>
        /// The state of this node.
        /// </summary>
        public NodeState NodeState => _nodeState;

        /// <summary>
        /// The data associated with this node.
        /// </summary>
        public NodeData NodeData => _nodeData;

        /// <summary>
        /// The node object itself.
        /// </summary>
        public Node Node => _node;

        #endregion // Public Getters

        private void Start()
        {
            SetupNeighbours();
            _nodeState = GetComponent<NodeState>();

            Node.Cost = NodeData.TerrainType.MovementCost;

            StoodUnit = DetectStoodUnit();

            NodeInitialized = true;
        }

        private void Update()
        {
            UpdateStoodUnit();
        }

        private UnitManager DetectStoodUnit()
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
            if (hit.transform.gameObject.tag == UNIT_TAG_REFERENCE)
            {
                return unitHit.transform.GetComponentInParent<UnitManager>();
            }
            else
            {
                Debug.LogWarning("[WARNING]: Cast hit non-unit object - " + unitHit.transform.gameObject.name);
                return null;
            }
        }

        private void SetupNeighbours()
        {
            _northNode = CheckForNeighbourNode(-transform.forward);
            _westNode = CheckForNeighbourNode(transform.right);
            _southNode = CheckForNeighbourNode(transform.forward);
            _eastNode = CheckForNeighbourNode(-transform.right);

            // TODO: Move magic numbers to constants
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

        private NodeManager CheckForNeighbourNode(Vector3 Direction)
        {
            if (Physics.Raycast(transform.position, Direction, out nodeHit, 1))
            {
                if (nodeHit.transform.gameObject.tag == NODE_TAG_REFERENCE)
                {
                    var NodeManager = nodeHit.transform.parent.GetComponent<NodeManager>();
                    return NodeManager;
                }
                else
                {
                    Debug.LogWarning("[WARNING]: Cast hit non-node object - " + nodeHit.transform.gameObject.name);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Update the stood unit every once in a while!
        /// </summary>
        public void UpdateStoodUnit()
        {
            StoodUnit = DetectStoodUnit();
            Node.IsOccupied = StoodUnit != null;
        }

        /// <summary>
        /// Highlights the movement range for a given unit around this node.
        /// </summary>
        /// <param name="unit">The unit which we want to highlight the range of.</param>
        /// <param name="isPressed">Whether we are highlighting in a pressed state or not.</param>
        public void HighlightRangeArea(UnitManager unit, bool isPressed = false)
        {
            unit.MovementRange.CalculateMovementRange(unit);


            if (Input.GetKey(KeyCode.Keypad0))
            {
                HighlightListNodes(unit.MovementRange.ReachableNodes, isPressed, NodeVisualColorState.Blue);
            }

            else if (Input.GetKey(KeyCode.Keypad1))
            {
                HighlightListNodes(unit.MovementRange.InteractiveNodes, isPressed, NodeVisualColorState.Red);
            }

            else if(Input.GetKey(KeyCode.Keypad2))
            {
                HighlightListNodes(unit.MovementRange.ValidStandNodes, isPressed, NodeVisualColorState.Green);
            }

            else if(Input.GetKey(KeyCode.Keypad3))
            {
                HighlightListNodes(unit.MovementRange.AttackNodes, isPressed, NodeVisualColorState.Red);
            }

            else
            {
                HighlightListNodes(unit.MovementRange.ReachableNodes, isPressed, NodeVisualColorState.Blue);
            }

        }

        private void HighlightListNodes(List<Node> nodes, bool isPressed, NodeVisualColorState nodeVisualColorState)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (isPressed)
                {
                    nodes[i].NodeManager.NodeState.VisualStateManager.SetPressed(nodeVisualColorState);
                }
                else
                {
                    nodes[i].NodeManager.NodeState.VisualStateManager.SetHovered(nodeVisualColorState);
                }
            }
        }

        /// <summary>
        /// Clears the stood unit.
        /// </summary>
        public void ClearStoodUnit()
        {
            StoodUnit = null;
        }
    }
}

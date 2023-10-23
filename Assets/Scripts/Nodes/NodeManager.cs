namespace CT6GAMAI
{
    using UnityEditor.Experimental.GraphView;
    using UnityEngine;

    public class NodeManager : MonoBehaviour
    {
        [SerializeField] private NodeState _nodeState;
        [SerializeField] private NodeData _nodeData;

        [SerializeField] private NodeManager _northNode;
        [SerializeField] private NodeManager _eastNode;
        [SerializeField] private NodeManager _southNode;
        [SerializeField] private NodeManager _westNode;

        [SerializeField] private NodeManager _northEastNode;
        [SerializeField] private NodeManager _northWestNode;
        [SerializeField] private NodeManager _southEastNode;
        [SerializeField] private NodeManager _southWestNode;

        private RaycastHit nodeHit;

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

        public UnitManager StoodUnit;

        void Start()
        {
            SetupNeighbours();
            _nodeState = GetComponent<NodeState>();
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
                Debug.Log("ERROR: Cast hit nothing");
                return null;
            }
        }

        public void HighlightRangeArea(UnitManager unit)
        {
            // Identify unit's movement range
            var range = unit.UnitData.MovementBaseValue;

            // Highlight selected node
            SetNodeState(unit.StoodNode.NodeState, Constants.State.HoveredBlue);

            // Highlight N E S W with the range amount
            HighlightNodes(_northNode, range, Constants.Direction.North);
            HighlightNodes(_eastNode, range, Constants.Direction.East);
            HighlightNodes(_southNode, range, Constants.Direction.South);
            HighlightNodes(_westNode, range, Constants.Direction.West);

            // Highlight diagonals with the range amount -1
            HighlightNodes(_northEastNode, range, Constants.Direction.NorthEast);
            HighlightNodes(_northWestNode, range, Constants.Direction.NorthWest);
            HighlightNodes(_southEastNode, range, Constants.Direction.SouthEast);
            HighlightNodes(_southWestNode, range, Constants.Direction.SouthWest);

            // Highlight range amount + 1 with either heal/attack range depending on unit type
        }

        private void HighlightNodes(NodeManager startNode, int range, Constants.Direction direction)
        {
            // the full range includes enemy/heal range
            int movementRange = range - 1;
            
            // If highlighting diagonally, -1, if normally 0
            int directionModifier = CalculateDirectionModifier(direction);

            if (movementRange + directionModifier <= -1)
            {
                return;
            }

            startNode.NodeState.IsHighlighted = true;
            SetNodeState(startNode.NodeState, Constants.State.HoveredBlue);

            NodeManager currentNode = startNode;           

            for (int i = 0; i < movementRange + directionModifier; i++)
            {
                NodeManager nextNode = null;

                switch (direction)
                {
                    case Constants.Direction.North:
                        nextNode = currentNode.NorthNode;
                        break;

                    case Constants.Direction.NorthEast: 
                        nextNode = currentNode.NorthEastNode;
                        break;

                    case Constants.Direction.East:
                        nextNode = currentNode.EastNode;
                        break;

                    case Constants.Direction.SouthEast: 
                        nextNode = currentNode.SouthEastNode;
                        break;

                    case Constants.Direction.South:
                        nextNode = currentNode.SouthNode;
                        break;

                    case Constants.Direction.SouthWest:
                        nextNode = currentNode.SouthWestNode;
                        break;

                    case Constants.Direction.West:
                        nextNode = currentNode.WestNode;
                        break;

                    case Constants.Direction.NorthWest:
                        nextNode = currentNode.NorthWestNode;
                        break;
                }

                if (nextNode != null)
                {
                    nextNode.NodeState.IsHighlighted = true;
                    SetNodeState(nextNode.NodeState, Constants.State.HoveredBlue);                   
                    currentNode = nextNode;
                }
                else
                {
                    break;
                }
            }
        }
        
        private int CalculateDirectionModifier(Constants.Direction direction)
        {
            // If direction is diagonal, apply a modifier
            if (direction == Constants.Direction.NorthEast || 
                direction == Constants.Direction.NorthWest || 
                direction == Constants.Direction.SouthEast || 
                direction == Constants.Direction.SouthWest)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        private void SetNodeState(NodeState nodeState, Constants.State newState)
        {
            nodeState.CurrentState = newState;           
        }
    }
}

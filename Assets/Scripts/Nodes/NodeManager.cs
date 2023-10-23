namespace CT6GAMAI
{
    using UnityEngine;

    public class NodeManager : MonoBehaviour
    {
        [SerializeField] private NodeState _nodeState;
        [SerializeField] private NodeData _nodeData;

        [SerializeField] private NodeManager _northNode;
        [SerializeField] private NodeManager _eastNode;
        [SerializeField] private NodeManager _southNode;
        [SerializeField] private NodeManager _westNode;

        private RaycastHit nodeHit;

        public NodeState NodeState => _nodeState;
        public NodeData NodeData => _nodeData;

        public NodeManager NorthNode => _northNode;
        public NodeManager EastNode => _eastNode;
        public NodeManager SouthNode => _southNode;
        public NodeManager WestNode => _westNode;

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
        }

        NodeManager CheckForNeighbourNode(Vector3 Direction)
        {
            if (Physics.Raycast(transform.position, Direction, out nodeHit, 1))
            {
                if (nodeHit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
                {
                    var NodeManager = nodeHit.transform.parent.GetComponent<NodeManager>();

                    Debug.Log("SUCCESS: Found NodeManager");

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
    }
}

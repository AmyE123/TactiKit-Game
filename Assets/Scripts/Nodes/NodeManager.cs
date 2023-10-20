namespace CT6GAMAI
{
    using UnityEngine;

    public class NodeManager : MonoBehaviour
    {
        [SerializeField] private NodeState _nodeState;

        public NodeState NodeState => _nodeState;

        void Start()
        {
            _nodeState = GetComponent<NodeState>();
        }
    }
}

namespace CT6GAMAI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridSelector _gridSelector;

        // TODO: Make this auto-populate.
        [SerializeField] private List<NodeManager> _allNodes;
        
        // TODO: Auto-detect and auto-populate this.
        //[SerializeField] private Node[] _occupiedNodes;
        [SerializeField] private List<NodeManager> _occupiedNodes;

        [SerializeField] private List<Node> _movementPath;
        [SerializeField] private MovementRange _movementRange;       

        // TODO: Move this somewhere more appropriate
        //[SerializeField] private Animator _animator;

        private bool _pathing = false;
        private bool _selectorWithinRange;
        private bool _gridInitialized = false;

        public bool UnitPressed = false;      

        public List<NodeManager> AllNodes => _allNodes;
        public List<NodeManager> OccupiedNodes => _occupiedNodes;
        public List<Node> MovementPath => _movementPath;

        private void Update()
        {
            if (!_gridInitialized)
            {
                InitializeGrid();
            }           
        }

        private void InitializeGrid()
        {
            if (CheckNodesInitialized())
            {
                CheckForOccupiedNodes();
                _gridInitialized = true;
            }

            if(_allNodes.Count == 0)
            {
                FindAllNodes();
            }
        }

        private bool CheckNodesInitialized()
        {
            foreach (NodeManager n in _allNodes)
            {
                if (!n.NodeInitialized)
                {
                    return false;
                }
            }

            return true;
        }

        private void FindAllNodes()
        {
            _allNodes = FindObjectsOfType<NodeManager>().ToList();
        }

        private void CheckForOccupiedNodes()
        {
            foreach (NodeManager n in _allNodes)
            {
                if (n.StoodUnit != null)
                {
                    if (!_occupiedNodes.Contains(n))
                    {
                        _occupiedNodes.Add(n);
                    }                    
                }
            }
        }
    }

}
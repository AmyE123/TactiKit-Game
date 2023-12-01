namespace CT6GAMAI
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the grid for the map.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private GridSelector _gridSelector;
        [SerializeField] private List<NodeManager> _allNodes;
        [SerializeField] private List<NodeManager> _occupiedNodes;
        [SerializeField] private List<Node> _movementPath;
        [SerializeField] private MovementRange _movementRange;

        private GameManager _gameManager;
        private bool _selectorWithinRange;
        private bool _gridInitialized = false;

        /// <summary>
        /// Indicates if a unit has been selected.
        /// </summary>
        public bool UnitPressed = false;

        /// <summary>
        /// Gets the list of all nodes in the grid.
        /// </summary>
        public List<NodeManager> AllNodes => _allNodes;

        /// <summary>
        /// Gets the list of currently occupied nodes.
        /// </summary>
        public List<NodeManager> OccupiedNodes => _occupiedNodes;

        /// <summary>
        /// Gets the movement path of the currently selected unit.
        /// </summary>
        public List<Node> MovementPath => _movementPath;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

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

            if (_allNodes.Count == 0)
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

        /// <summary>
        /// Handles the pathing logic when a unit is selected.
        /// </summary>
        public void HandleUnitPathing()
        {
            UpdateSelectorRange();
            ProcessPathing();
        }

        /// <summary>
        /// Updates whether the selector is within range of the reachable nodes.
        /// </summary>
        public void UpdateSelectorRange()
        {
            foreach (Node n in _movementRange.ReachableNodes)
            {
                _selectorWithinRange = _movementRange.ReachableNodes.Contains(_gridSelector.SelectedNode.Node);
                n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
            }
        }

        /// <summary>
        /// Processes pathing for the selected unit, including movement and path highlighting.
        /// </summary>
        public void ProcessPathing()
        {
            if (_gridSelector.Pathing)
            {
                // TODO: Upate this start node with new pathing
                Node startNode = _occupiedNodes[0].Node;
                Node targetNode = _gridSelector.SelectedNode.Node;

                if (_selectorWithinRange)
                {
                    _movementPath = _movementRange.ReconstructPath(startNode, targetNode);

                    if (_movementPath.Count > 1 && Input.GetKeyDown(KeyCode.Space))
                    {
                        var unit = _gameManager.UnitsManager.AllUnits[0];
                        // Clear the stood node's reference to the unit

                        unit.ClearStoodUnit();

                        // Move unit here
                        StartCoroutine(unit.MoveToEndPoint());
                        unit.Animator.SetBool("Moving", unit.IsMoving);
                    }
                }
            }

            HighlightPath();
        }

        /// <summary>
        /// Highlights the path for the selected units potential movement.
        /// </summary>
        private void HighlightPath()
        {
            foreach (Node n in _movementPath)
            {
                if (_movementRange.ReachableNodes.Contains(n))
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPath();
                }
            }
        }
    }

}
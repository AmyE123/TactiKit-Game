namespace CT6GAMAI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using static CT6GAMAI.Constants;
    using static CT6GAMAI.GridManager;

    /// <summary>
    /// Manages the grid for the map.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public enum CurrentState { Idle, Moving, ActionSelected, ConfirmingMove };

        [SerializeField] private GridCursor _gridCursor;
        [SerializeField] private List<NodeManager> _allNodes;
        [SerializeField] private List<NodeManager> _occupiedNodes;
        [SerializeField] private List<Node> _movementPath;
        
        public CurrentState _currentState;

        private UnitManager _activeUnit;

        private GameManager _gameManager;
        private bool _cursorWithinRange;
        private bool _gridInitialized = false;

        public GridCursor GridCursor => _gridCursor;

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

        /// <summary>
        /// Whether a unit is currently pressed or not
        /// </summary>
        public bool UnitPressed => _gridCursor.UnitPressed;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            UpdateUnitReferences();

            if (!_gridInitialized)
            {
                InitializeGrid();
            }

            if (_currentState == CurrentState.ConfirmingMove)
            {
                var unit = _gameManager.UnitsManager.ActiveUnit;

                if (unit != null && unit.IsAwaitingMoveConfirmation)
                {
                    if (Input.GetKeyDown(KeyCode.Escape))
                    {                      
                        Debug.Log("CANCEL!!");
                        unit.CancelMove();
                        unit.IsAwaitingMoveConfirmation = false;
                        //_currentState = CurrentState.Idle;
                    }
                }
            }

            if (_currentState == CurrentState.ActionSelected)
            {
                StartCoroutine(IdleDelay());
            }
        }

        IEnumerator IdleDelay()
        {
            yield return new WaitForSeconds(0.5f);
            _currentState = CurrentState.Idle;
        }

        private void UpdateUnitReferences()
        {
            _activeUnit = _gameManager.UnitsManager.ActiveUnit;
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

        private void HighlightPath()
        {
            foreach (Node n in _movementPath)
            {
                if (_activeUnit.MovementRange.ReachableNodes.Contains(n))
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPath();
                }
            }
        }

        /// <summary>
        /// Checks if a movement to a node is valid based on current game rules.
        /// </summary>
        /// <returns>
        /// False if the node is occupied by a unit other than the last selected unit. Otherwise true.
        /// </returns>
        public bool CanMoveToNode(Node node)
        {
            // Checks if the node is occupied by a unit other than the last selected one.
            var currentUnitOnNode = GetOccupyingUnitFromNode(node);
            var nodeOccupiedByOtherUnit = currentUnitOnNode != null && currentUnitOnNode != _gameManager.UnitsManager.ActiveUnit;

            bool canMoveToNode = !nodeOccupiedByOtherUnit;

            return canMoveToNode;
        }

        /// <summary>
        /// Gets the occupying unit from a node.
        /// </summary>
        public UnitManager GetOccupyingUnitFromNode(Node node)
        {
            return node.NodeManager.StoodUnit;
        }

        /// <summary>
        /// Checks if a node is occupied by an enemy.
        /// </summary>
        public bool isNodeOccupiedByEnemy(Node node)
        {
            var currentUnitOnNode = GetOccupyingUnitFromNode(node);
            return currentUnitOnNode != null && currentUnitOnNode.UnitData.UnitTeam == Team.Enemy;
        }



        /// <summary>
        /// Handles the pathing logic when a unit is selected.
        /// </summary>
        public void HandleUnitPathing()
        {
            UpdateCursorRange();
            ProcessPathing();
        }

        /// <summary>
        /// Updates whether the cursor is within range of the reachable nodes.
        /// </summary>
        public void UpdateCursorRange()
        {
            foreach (Node n in _activeUnit.MovementRange.ReachableNodes)
            {
                _cursorWithinRange = _activeUnit.MovementRange.ReachableNodes.Contains(_gridCursor.SelectedNode.Node);
                n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
            }
        }

        /// <summary>
        /// Processes pathing for the selected unit, including movement and path highlighting.
        /// </summary>
        public void ProcessPathing()
        {
            if (_gridCursor.Pathing)
            {
                Node startNode = _gameManager.UnitsManager.ActiveUnit.StoodNode.Node;
                Node targetNode = _gridCursor.SelectedNode.Node;

                if (_cursorWithinRange)
                {
                    _movementPath = _activeUnit.MovementRange.ReconstructPath(startNode, targetNode);

                    if (_currentState != CurrentState.ConfirmingMove && _currentState != CurrentState.Moving && Input.GetKeyDown(KeyCode.Space))
                    {
                        var validPath = _movementPath.Count > 1 && CanMoveToNode(targetNode);

                        _gameManager.AudioManager.PlaySelectPathSound(validPath);

                        if (validPath)
                        {
                            Debug.Log("[GAME]: Actions list pop-up UI here");
                            _gameManager.UIManager.ActionItemsManager.ShowActionItems();

                            var unit = _gameManager.UnitsManager.ActiveUnit;

                            // Clear the stood node's reference to the unit
                            unit.ClearStoodUnit();

                            // Move unit here
                            StartCoroutine(unit.MoveToEndPoint());
                        }
                        else
                        {
                            if (isNodeOccupiedByEnemy(targetNode))
                            {
                                Debug.Log("[GAME]: Battle Forecast UI here");
                                _gameManager.UIManager.SpawnBattleForecast(_activeUnit.UnitData, targetNode.NodeManager.StoodUnit.UnitData);
                            }
                        }
                    }
                }
            }

            HighlightPath();
        }
    }
}
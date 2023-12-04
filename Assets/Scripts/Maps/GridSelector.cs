namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages the selection and interaction of nodes within the grid.
    /// This includes handling node selection, grid navigation, unit selection, and pathing.
    /// </summary>
    public class GridSelector : MonoBehaviour
    {
        //[SerializeField] private MovementRange _movementRange;
        //[SerializeField] private Animator _animator; // TODO: Update with UnitAnimationManager.cs functionality.
        //[SerializeField] private UnitManager _unit; // TODO: Once multi-units is implemented, this will be changed.

        private bool _pathing = false;
        private GameManager _gameManager;
        private GridManager _gridManager;
        private UnitManager _lastSelectedUnit;

        /// <summary>
        /// Array of all NodeManagers in the grid.
        /// </summary>
        public NodeManager[] Nodes; // TODO: Update with AllNodes in GridManager.

        /// <summary>
        /// The currently selected node.
        /// </summary>
        public NodeManager SelectedNode;

        /// <summary>
        /// The state of the currently selected node.
        /// </summary>
        public NodeState SelectedNodeState; // TODO: Update this, this isn't needed (SelectedNode.NodeState).

        /// <summary>
        /// Indicates whether a unit is currently pressed (selected).
        /// </summary>
        public bool UnitPressed = false;

        /// <summary>
        /// Indicates whether pathing mode is active.
        /// </summary>
        public bool Pathing => _pathing;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;

            SelectedNode.NodeState.SelectorStateManager.SetDefaultSelected();

            // TODO: Once multi-units is implemented, this will be changed.
            //_activeUnit = FindObjectOfType<UnitManager>();
        }

        private void Update()
        {
            // TODO: Update with UnitAnimationManager.cs functionality.
            //_animator.SetBool("Ready", UnitPressed);

            UpdateUnitReferences();
            UpdateSelectedNode();
            HandleNodeUnitInteraction();
            HandleGridNavigation();
            HandleUnitSelection();
            HandleUnitPathing();
        }

        private void UpdateUnitReferences()
        {
            var unitManager = _gameManager.UnitsManager;
            _lastSelectedUnit = unitManager.LastSelectedUnit;
        }

        private void UpdateSelectedNode()
        {
            if (SelectedNodeState == null) 
            { 
                SelectedNodeState = SelectedNode.NodeState; 
            }

            if (SelectedNode == null || !SelectedNodeState.SelectorStateManager.IsActiveSelection)
            {
                UpdateActiveNodeSelection();
            }
        }

        private void UpdateActiveNodeSelection()
        {
            for (int i = 0; i < Nodes.Length; i++)
            {
                if (Nodes[i].NodeState.SelectorStateManager.IsActiveSelection)
                {
                    SelectedNode = Nodes[i];
                    SelectedNodeState = SelectedNode.NodeState;
                }
            }
        }

        private void HandleNodeUnitInteraction()
        {
            if (SelectedNode.StoodUnit != null)
            {
                _gameManager.UnitsManager.SetActiveUnit(SelectedNode.StoodUnit);

                ResetHighlightedNodes();              
                SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.VisualStateManager.IsPressed);
            }
            else
            {
                _gameManager.UnitsManager.SetActiveUnit(null);
                if (!UnitPressed)
                {
                    ResetHighlightedNodes();
                }
            }
        }

        private void HandleGridNavigation()
        {
            if (!_gameManager.UnitsManager.IsAnyUnitMoving())
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    MoveSelector(Direction.North);
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    MoveSelector(Direction.West);
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    MoveSelector(Direction.South);
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    MoveSelector(Direction.East);
                }
            }
        }

        private void MoveSelector(Direction direction)
        {
            NodeState adjacentNodeState = GetAdjacentNodeState(direction);
            if (adjacentNodeState != null)
            {
                adjacentNodeState.SelectorStateManager.SetDefaultSelected();
                SelectedNodeState.SelectorStateManager.SetInactive();
            }
        }

        private NodeState GetAdjacentNodeState(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return SelectedNode.NorthNode.NodeState;
                case Direction.South:
                    return SelectedNode.SouthNode.NodeState;
                case Direction.West:
                    return SelectedNode.WestNode.NodeState;
                case Direction.East:
                    return SelectedNode.EastNode.NodeState;
                default:
                    Debug.Assert(false, "Invalid direction passed to GetAdjacentNodeState.");
                    return null;
            }
        }

        private void HandleUnitSelection()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleUnitSelection();
            }
        }

        private void ToggleUnitSelection()
        {
            if (SelectedNode.StoodUnit != null)
            {
                UnitPressed = !UnitPressed;
                _pathing = UnitPressed;

                UpdateNodeVisualState();
            }
        }

        private void UpdateNodeVisualState()
        {
            if (UnitPressed)
            {
                SelectedNodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
                }
            }

            if (!UnitPressed)
            {
                SelectedNodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);

                foreach (Node n in _lastSelectedUnit.MovementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);
                }
            }
        }

        private void HandleUnitPathing()
        {
            if (!UnitPressed)
            {
                return;
            }

            _gridManager.HandleUnitPathing();
        }

        /// <summary>
        /// Resets the visual state of all highlighted nodes to default.
        /// </summary>
        public void ResetHighlightedNodes()
        {
            if (_lastSelectedUnit != null)
            {
                _lastSelectedUnit.MovementRange.ResetNodes();
            }        

            for (int i = 0; i < Nodes.Length; i++)
            {
                if (Nodes[i].NodeState.VisualStateManager.IsActive)
                {
                    Nodes[i].NodeState.VisualStateManager.SetDefault();
                }

                SelectedNodeState.VisualStateManager.SetDefault();
            }
        }
    }
}
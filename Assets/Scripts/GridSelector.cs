namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class GridSelector : MonoBehaviour
    {
        public NodeManager[] Nodes;
        public NodeManager SelectedNode;
        public NodeState SelectedNodeState;

        //public Node OccupiedNode;
        //public List<Node> path = new List<Node>();

        [SerializeField] private MovementRange _movementRange;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnitManager _unit;

        public bool UnitPressed = false;
        private bool _pathing = false;
        //private bool selectorWithinRange;

        public bool Pathing => _pathing;

        private GameManager _gameManager;
        private GridManager _gridManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;

            SelectedNode.NodeState.SelectorStateManager.SetDefaultSelected();
            _unit = FindObjectOfType<UnitManager>();
        }

        private void Update()
        {
            _animator.SetBool("Ready", UnitPressed);

            UpdateSelectedNode();
            HandleNodeUnitInteraction();
            HandleGridNavigation();
            HandleUnitSelection();
            HandleUnitPathing();
        }

        /// <summary>
        /// Updates the currently selected node and its state.
        /// </summary>
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

        /// <summary>
        /// Scans all nodes and updates the currently active selection.
        /// </summary>
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

        /// <summary>
        /// Handles interactions for the node.
        /// </summary>
        private void HandleNodeUnitInteraction()
        {
            if (SelectedNode.StoodUnit != null)
            {
                ResetHighlightedNodes();
                SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.VisualStateManager.IsPressed);
            }
            else if (!UnitPressed)
            {
                ResetHighlightedNodes();
            }
        }

        /// <summary>
        /// Handles user input for grid navigation.
        /// </summary>
        private void HandleGridNavigation()
        {
            if (!_unit.IsMoving)
            {
                if (Input.GetKeyDown(KeyCode.W)) MoveSelector(Direction.North);
                if (Input.GetKeyDown(KeyCode.A)) MoveSelector(Direction.West);
                if (Input.GetKeyDown(KeyCode.S)) MoveSelector(Direction.South);
                if (Input.GetKeyDown(KeyCode.D)) MoveSelector(Direction.East);
            }
        }

        /// <summary>
        /// Moves the selector in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move the selector.</param>
        private void MoveSelector(Direction direction)
        {
            NodeState adjacentNodeState = GetAdjacentNodeState(direction);
            if (adjacentNodeState != null)
            {
                adjacentNodeState.SelectorStateManager.SetDefaultSelected();
                SelectedNodeState.SelectorStateManager.SetInactive();
            }
        }

        /// <summary>
        /// Gets the node state of the adjacent node in the specified direction.
        /// </summary>
        /// <param name="direction">The direction of the adjacent node.</param>
        /// <returns>The state of the adjacent node, or null if none.</returns>
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
                    return null;
            }
        }

        /// <summary>
        /// Handles the user input for selecting or deselecting a unit.
        /// </summary>
        private void HandleUnitSelection()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleUnitSelection();
            }
        }

        /// <summary>
        /// Toggles the selection state of a unit and updates the visual state of nodes.
        /// </summary>
        private void ToggleUnitSelection()
        {
            if (SelectedNode.StoodUnit != null)
            {
                UnitPressed = !UnitPressed;
                _pathing = UnitPressed;

                UpdateNodeVisualState();
            }
        }

        /// <summary>
        /// Updates the visual state of the selected node and its reachable nodes.
        /// </summary>
        private void UpdateNodeVisualState()
        {
            if (UnitPressed)
            {
                SelectedNodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);

                foreach (Node n in _movementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
                }
            }

            if (!UnitPressed)
            {
                SelectedNodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);

                foreach (Node n in _movementRange.ReachableNodes)
                {
                    n.NodeManager.NodeState.VisualStateManager.SetHovered(NodeVisualColorState.Blue);
                }
            }
        }

        /// <summary>
        /// Handles the pathing logic when a unit is selected.
        /// </summary>
        private void HandleUnitPathing()
        {
            if (!UnitPressed) return;

            _gridManager.HandleUnitPathing();
        }

        /// <summary>
        /// Resets all highlighted nodes.
        /// </summary>
        public void ResetHighlightedNodes()
        {
            _movementRange.ResetNodes();

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
namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class GridSelector : MonoBehaviour
    {
        public NodeManager[] Nodes;
        public NodeManager SelectedNode;
        public NodeState SelectedNodeState;

        public Node OccupiedNode;
        public List<Node> path = new List<Node>();

        [SerializeField] private MovementRange _movementRange;
        [SerializeField] private Animator _animator;
        [SerializeField] private UnitManager _unit;

        public bool UnitPressed = false;
        private bool pathing = false;
        private bool selectorWithinRange;

        private void Start()
        {
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
                pathing = UnitPressed;

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

            UpdateSelectorRange();
            ProcessPathing();
        }

        /// <summary>
        /// Updates whether the selector is within range of the reachable nodes.
        /// </summary>
        private void UpdateSelectorRange()
        {
            foreach (Node n in _movementRange.ReachableNodes)
            {
                selectorWithinRange = _movementRange.ReachableNodes.Contains(SelectedNode.Node);
                n.NodeManager.NodeState.VisualStateManager.SetPressed(NodeVisualColorState.Blue);
            }          
        }

        /// <summary>
        /// Processes pathing for the selected unit, including movement and path highlighting.
        /// </summary>
        private void ProcessPathing()
        {
            if (pathing)
            {
                Node startNode = OccupiedNode;
                Node targetNode = SelectedNode.Node;

                if (selectorWithinRange)
                {
                    path = _movementRange.ReconstructPath(startNode, targetNode);

                    if (path.Count > 1 && Input.GetKeyDown(KeyCode.Space))
                    {
                        // Clear the stood node's reference to the unit
                        _unit.ClearStoodUnit();

                        // Move unit here
                        StartCoroutine(_unit.MoveToEndPoint());
                        _animator.SetBool("Moving", _unit.IsMoving);
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
            foreach (Node n in path)
            {
                if (_movementRange.ReachableNodes.Contains(n))
                {
                    n.NodeManager.NodeState.VisualStateManager.SetPath();
                }
            }
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
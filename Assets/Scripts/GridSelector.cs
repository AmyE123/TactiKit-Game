namespace CT6GAMAI
{
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

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

        private bool unitPressed = false;
        private bool pathing = false;
        private bool selectorWithinRange;

        public bool unitActivatedMoving = false;


        private void Start()
        {
            SelectedNode.NodeState.SelectorStateManager.SetDefaultSelected();
            _unit = FindObjectOfType<UnitManager>();
        }

        // Update is called once per frame
        void Update()
        {
            _animator.SetBool("Ready", unitPressed);

            if (SelectedNodeState == null) { SelectedNodeState = SelectedNode.NodeState; }

            if (SelectedNode == null || !SelectedNodeState.SelectorStateManager.IsActiveSelection)
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

            if (SelectedNode.StoodUnit != null)
            {
                SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.VisualStateManager.IsPressed);
            }
            else
            {
                if (!unitPressed)
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

            // Moving forward
            if (Input.GetKeyDown(KeyCode.W))
            {
                var northNs = SelectedNode.NorthNode.NodeState;

                if (northNs != null)
                {
                    northNs.SelectorStateManager.SetDefaultSelected();
                    SelectedNodeState.SelectorStateManager.SetInactive();
                }
            }

            // Moving left
            if (Input.GetKeyDown(KeyCode.A))
            {
                var westNs = SelectedNode.WestNode.NodeState;

                if (westNs != null)
                {
                    westNs.SelectorStateManager.SetDefaultSelected();
                    SelectedNodeState.SelectorStateManager.SetInactive();
                }
            }

            // Moving backward
            if (Input.GetKeyDown(KeyCode.S))
            {
                var southNs = SelectedNode.SouthNode.NodeState;

                if (southNs != null)
                {
                    southNs.SelectorStateManager.SetDefaultSelected();
                    SelectedNodeState.SelectorStateManager.SetInactive();
                }
            }

            // Moving right
            if (Input.GetKeyDown(KeyCode.D))
            {
                var eastNs = SelectedNode.EastNode.NodeState;

                if (eastNs != null)
                {
                    eastNs.SelectorStateManager.SetDefaultSelected();
                    SelectedNodeState.SelectorStateManager.SetInactive();
                }
            }

            // Selection of a unit, if hovered over one.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (SelectedNode.StoodUnit != null)
                {
                    unitPressed = !unitPressed;
                    pathing = unitPressed;

                    if (unitPressed)
                    {
                        SelectedNodeState.VisualStateManager.SetPressed(Constants.NodeVisualColorState.Blue);

                        foreach (Node n in _movementRange.Nodes)
                        {
                            n.NodeManager.NodeState.VisualStateManager.SetPressed(Constants.NodeVisualColorState.Blue);
                        }
                    }

                    if (!unitPressed)
                    {
                        SelectedNodeState.VisualStateManager.SetHovered(Constants.NodeVisualColorState.Blue);

                        foreach (Node n in _movementRange.Nodes)
                        {
                            n.NodeManager.NodeState.VisualStateManager.SetHovered(Constants.NodeVisualColorState.Blue);
                        }
                    }

                }
            }

            if (unitPressed)
            {               
                foreach (Node n in _movementRange.Nodes)
                {
                    if (_movementRange.Nodes.Contains(SelectedNode.Node))
                    {
                        selectorWithinRange = true;
                    }
                    else
                    {
                        selectorWithinRange = false;
                    }

                    n.NodeManager.NodeState.VisualStateManager.SetPressed(Constants.NodeVisualColorState.Blue);
                }

                if (pathing)
                {
                    Node startNode = OccupiedNode;
                    Node targetNode = SelectedNode.Node;

                    if (selectorWithinRange)
                    {
                        path = _movementRange.ReconstructPath(startNode, targetNode);

                        if (path.Count > 1 && Input.GetKeyDown(KeyCode.Space))
                        {
                            //Move unit here
                            StartCoroutine(_unit.MoveToEndPoint());
                            _animator.SetBool("Moving", _unit.IsMoving);
                        }
                    }


                    foreach (Node n in path)
                    {
                        if (_movementRange.Nodes.Contains(n))
                        {
                            n.NodeManager.NodeState.VisualStateManager.SetPath();
                        }
                    }
                }
            }
        }
    }
}
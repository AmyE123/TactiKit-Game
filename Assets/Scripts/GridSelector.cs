namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GridSelector : MonoBehaviour
    {
        public NodeManager[] Nodes;
        public NodeManager SelectedNode;
        public NodeState SelectedNodeState;

        public Node OccupiedNode;
        public List<Node> path = new List<Node>();

        [SerializeField] private MovementRange _movementRange;

        public bool unitPressed = false;

        private void Start()
        {
            SelectedNode.NodeState.NodeSelectorManager.SetDefaultSelected();
        }

        // Update is called once per frame
        void Update()
        {
            if (SelectedNodeState == null) { SelectedNodeState = SelectedNode.NodeState; }

            if (SelectedNode == null || !SelectedNodeState.NodeSelectorManager.IsActiveSelection)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].NodeState.NodeSelectorManager.IsActiveSelection)
                    {
                        SelectedNode = Nodes[i];
                        SelectedNodeState = SelectedNode.NodeState;
                    }
                }
            }

            if (SelectedNode.StoodUnit != null)
            {
                SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit, SelectedNodeState.NodeVisualManager.IsPressed);
            }
            else
            {
                if (!unitPressed)
                {
                    _movementRange.ResetNodes();

                    for (int i = 0; i < Nodes.Length; i++)
                    {
                        if (Nodes[i].NodeState.NodeVisualManager.IsActive)
                        {
                            Nodes[i].NodeState.NodeVisualManager.SetDefault();
                        }

                        SelectedNodeState.NodeVisualManager.SetDefault();
                    }
                }

                if (Input.GetKeyDown(KeyCode.P))
                {
                    Node startNode = OccupiedNode;
                    Node targetNode = SelectedNode.Node;

                    path = _movementRange.ReconstructPath(startNode, targetNode);

                    foreach (Node n in path)
                    {
                        n.NodeManager.NodeState.NodeVisualManager.SetPath();
                    }
                }

                // Moving forward
                if (Input.GetKeyDown(KeyCode.W))
                {
                    var northNs = SelectedNode.NorthNode.NodeState;

                    if (northNs != null)
                    {
                        northNs.NodeSelectorManager.SetDefaultSelected();
                        SelectedNodeState.NodeSelectorManager.SetInactive();
                    }
                }

                // Moving left
                if (Input.GetKeyDown(KeyCode.A))
                {
                    var westNs = SelectedNode.WestNode.NodeState;

                    if (westNs != null)
                    {
                        westNs.NodeSelectorManager.SetDefaultSelected();
                        SelectedNodeState.NodeSelectorManager.SetInactive();
                    }
                }

                // Moving backward
                if (Input.GetKeyDown(KeyCode.S))
                {
                    var southNs = SelectedNode.SouthNode.NodeState;

                    if (southNs != null)
                    {
                        southNs.NodeSelectorManager.SetDefaultSelected();
                        SelectedNodeState.NodeSelectorManager.SetInactive();
                    }
                }

                // Moving right
                if (Input.GetKeyDown(KeyCode.D))
                {
                    var eastNs = SelectedNode.EastNode.NodeState;

                    if (eastNs != null)
                    {
                        eastNs.NodeSelectorManager.SetDefaultSelected();
                        SelectedNodeState.NodeSelectorManager.SetInactive();
                    }
                }

                // Selection of a unit, if hovered over one.
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (SelectedNode.StoodUnit != null)
                    {
                        unitPressed = !unitPressed;

                        if (unitPressed)
                        {
                            SelectedNodeState.NodeVisualManager.SetPressed(Constants.NodeVisualColorState.Blue);

                            foreach (Node n in _movementRange.Nodes)
                            {
                                n.NodeManager.NodeState.NodeVisualManager.SetPressed(Constants.NodeVisualColorState.Blue);
                            }
                        }

                        if (!unitPressed)
                        {
                            SelectedNodeState.NodeVisualManager.SetHovered(Constants.NodeVisualColorState.Blue);

                            foreach (Node n in _movementRange.Nodes)
                            {
                                n.NodeManager.NodeState.NodeVisualManager.SetHovered(Constants.NodeVisualColorState.Blue);
                            }
                        }

                    }
                }

                if (unitPressed)
                {
                    foreach (Node n in _movementRange.Nodes)
                    {
                        n.NodeManager.NodeState.NodeVisualManager.SetPressed(Constants.NodeVisualColorState.Blue);
                    }
                }

            }
        }
    }
}
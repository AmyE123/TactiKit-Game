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

        // Update is called once per frame
        void Update()
        {
            if (SelectedNodeState == null) { SelectedNodeState = SelectedNode.NodeState; }

            if (SelectedNode == null || !SelectedNodeState.IsSelected)
            {
                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].NodeState.IsSelected)
                    {
                        SelectedNode = Nodes[i];
                        SelectedNodeState = SelectedNode.NodeState;
                    }
                }
            }

            if (SelectedNode.StoodUnit != null)
            {
                SelectedNode.HighlightRangeArea(SelectedNode.StoodUnit);
            }
            else
            {
                _movementRange.ResetNodes();

                for (int i = 0; i < Nodes.Length; i++)
                {
                    if (Nodes[i].NodeState.IsHighlighted)
                    {
                        Nodes[i].NodeState.IsHighlighted = false;
                    }
                }

                SelectedNodeState.CurrentState = Constants.NodeVisualState.Default;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Node startNode = OccupiedNode;
                Node targetNode = SelectedNode.Node;

                path = _movementRange.ReconstructPath(startNode, targetNode);

                foreach (Node n in path)
                {
                    n.NodeManager.NodeState.IsBolded = true;
                    //n.NodeManager.NodeState.IsBolded = true;
                }
            }

            // Moving forward
            if (Input.GetKeyDown(KeyCode.W))
            {
                var northNs = SelectedNode.NorthNode.NodeState;

                if (northNs != null)
                {
                    northNs.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving left
            if (Input.GetKeyDown(KeyCode.A))
            {
                var westNs = SelectedNode.WestNode.NodeState;

                if (westNs != null)
                {
                    westNs.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving backward
            if (Input.GetKeyDown(KeyCode.S))
            {
                var southNs = SelectedNode.SouthNode.NodeState;

                if (southNs != null)
                {
                    southNs.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving right
            if (Input.GetKeyDown(KeyCode.D))
            {
                var eastNs = SelectedNode.EastNode.NodeState;

                if (eastNs != null)
                {
                    eastNs.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Selection
            // TODO: Make this work
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectedNodeState.IsLocked = true;

                // TODO: Select North IF avaliable, else, select from avaliable.
                //SelectedNode.NorthNode.NodeState.IsSelected = true;

                //SelectedNodeState.IsSelected = false;

                //foreach (Node n in _movementRange.Nodes)
                //{
                //    n.NodeManager.NodeState.IsBolded = true;
                //}
            }

        }
    }
}
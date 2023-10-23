namespace CT6GAMAI
{
    using UnityEngine;

    public class GridSelector : MonoBehaviour
    {
        public NodeManager[] Nodes;
        public NodeManager SelectedNode;
        private NodeState SelectedNodeState;

        // Update is called once per frame
        void Update()
        {           
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectedNodeState.IsLocked = true;

                // TODO: Select North IF avaliable, else, select from avaliable.
                SelectedNode.NorthNode.NodeState.IsSelected = true;

                SelectedNodeState.IsSelected = false;
            }

        }
    }
}
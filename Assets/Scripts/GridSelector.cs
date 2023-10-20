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
                if (SelectedNodeState.ForwardNode != null)
                {
                    SelectedNodeState.ForwardNode.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving left
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (SelectedNodeState.LeftNode != null)
                {
                    SelectedNodeState.LeftNode.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving backward
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (SelectedNodeState.BackwardNode != null)
                {
                    SelectedNodeState.BackwardNode.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Moving right
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (SelectedNodeState.RightNode != null)
                {
                    SelectedNodeState.RightNode.IsSelected = true;
                    SelectedNodeState.IsSelected = false;
                }
            }

            // Selection
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SelectedNodeState.IsLocked = true;
                SelectedNodeState.ForwardNode.IsSelected = true;
                SelectedNodeState.IsSelected = false;
            }

        }
    }
}
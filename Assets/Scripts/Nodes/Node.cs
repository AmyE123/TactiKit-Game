namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Node : MonoBehaviour
    {
        #region Private Variables

        // TODO: Figure out a better way to deal with this
        [SerializeField] private NodeManager _manager;

        #endregion // Private Variables

        // TODO: Move public variables to private
        #region Public Variables

        // The cost to enter this node
        // TODO: Make this update with the terrain data
        public int Cost = 1;

        // The distance used for Dijkstra's algorithm
        public int Distance { get; set; }

        // Whether the node has been visited
        public bool Visited = false;

        // Whether the node is walkable
        public bool Walkable;

        // A list of adjacent nodes (neighbors)
        public List<Node> Neighbors;

        // For movement algorithm, store the predecessor
        public Node Predecessor { get; set; }

        public Transform UnitTransform;

        #endregion // Public Variables

        #region Public Getters

        public NodeManager NodeManager => _manager;

        #endregion // Public Getters

        public Node(int cost)
        {
            Cost = cost;
            Visited = false;
            Distance = int.MaxValue;
            Neighbors = new List<Node>();
        }

        public void AddNeighbor(Node neighbor)
        {
            Neighbors.Add(neighbor);
        }

        public void ResetNode()
        {
            Visited = false;
            Distance = int.MaxValue;
        }
    }
}

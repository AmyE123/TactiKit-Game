namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A class for calculating the range that the unit can move.
    /// </summary>
    public class MovementRange : MonoBehaviour
    {
        // TODO: Update grid selector to grid manager
        [SerializeField] private GridSelector _gridSelector;
        [SerializeField] private List<Node> _reachableNodes = new List<Node>();

        /// <summary>
        /// A list of nodes which the unit can reach.
        /// This list is only populated when the unit has been selected.
        /// </summary>
        public List<Node> ReachableNodes => _reachableNodes;

        /// <summary>
        /// Calculates the movement range of a unit using Dijkstra's Algorithm.
        /// </summary>
        /// <param name="start">The starting node.</param>
        /// <param name="movementPoints">The maximum movement points of the unit.</param>
        /// <returns>A list of nodes representing the reachable area.</returns>
        public List<Node> CalculateMovementRange(Node start, int movementPoints)
        {
            InitializeNodes();

            // Initialize the starting node's distance to 0
            start.Distance = 0;

            // Priority queue to select the node with the smallest distance
            var priorityQueue = new PriorityQueue<Node>();
            priorityQueue.Enqueue(start, start.Distance);

            AddNodeToReachable(start);

            while (!priorityQueue.IsEmpty())
            {
                // Get the node with the smallest distance
                Node current = priorityQueue.Dequeue();

                if (current.Visited)
                {
                    continue;
                }

                current.Visited = true;

                // If the current node is within movement points, add to reachable nodes
                if (current.Distance <= movementPoints)
                {
                    AddNodeToReachable(current);            
                }

                EnqueueNeighbours(current, priorityQueue);
            }

            ResetNodeStates();

            return _reachableNodes;
        }

        /// <summary>
        /// Reconstructs the path from a start node to a target node.
        /// </summary>
        /// <param name="start">The starting node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>A list of nodes representing the path from start to target.</returns>
        public List<Node> ReconstructPath(Node start, Node target)
        {
            List<Node> path = new List<Node>();
            Node current = target;

            while (current != null && current != start)
            {
                path.Add(current);
                current = current.Predecessor;
            }

            path.Add(start); // Add the start node
            path.Reverse(); // Reverse the list to get the path from start to target

            return path;
        }

        /// <summary>
        /// Resets the state of the nodes for a new calculation.
        /// </summary>
        public void ResetNodes()
        {
            _reachableNodes.Clear();
        }

        private void InitializeNodes()
        {
            foreach (NodeManager nodeManager in _gridSelector.Nodes)
            {
                nodeManager.Node.Visited = false;
                nodeManager.Node.Distance = int.MaxValue;
            }
        }

        private void AddNodeToReachable(Node node)
        {
            if (!_reachableNodes.Contains(node))
            {
                _reachableNodes.Add(node);
            }
        }

        private void EnqueueNeighbours(Node current, PriorityQueue<Node> queue)
        {
            // Loop through each neighbor of the current node
            foreach (Node neighbor in current.Neighbors)
            {
                if (neighbor.Visited) continue; // Skip already visited neighbors

                // Calculate the tentative distance to the neighbor
                int tentativeDistance = current.Distance + neighbor.Cost;

                // If the tentative distance is less than the neighbor's recorded distance
                if (tentativeDistance < neighbor.Distance)
                {
                    // Update the neighbor's distance
                    neighbor.Distance = tentativeDistance;

                    // Sets the predecessor for pathfinding
                    neighbor.Predecessor = current;

                    // If the neighbor has not been visited or the tentative distance is better, enqueue it
                    if (!neighbor.Visited)
                    {
                        queue.Enqueue(neighbor, tentativeDistance);
                    }
                }
            }
        }

        private void ResetNodeStates()
        {
            // Reset visited and distance for all nodes for the next calculation
            foreach (NodeManager nodeManager in _gridSelector.Nodes)
            {
                nodeManager.Node.Visited = false;
                nodeManager.Node.Distance = int.MaxValue;
            }
        }
    }
}
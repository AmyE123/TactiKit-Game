namespace CT6GAMAI
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A class for calculating the range that the unit can move.
    /// </summary>
    public class MovementRange : MonoBehaviour
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private List<Node> _reachableNodes = new List<Node>();
        [SerializeField] private List<Node> _rangeNodes = new List<Node>();
        [SerializeField] private List<Node> _validStandNodes = new List<Node>();
        [SerializeField] private List<Node> _interactiveNodes = new List<Node>();
        [SerializeField] private List<Node> _attackNodes = new List<Node>();
        [SerializeField] private List<Node> _trueReachableNodes = new List<Node>();
        
        private GameManager _gameManager;
        private Node _unitStoodNode;

        /// <summary>
        /// A list of nodes which the unit can reach.
        /// This list is only populated when the unit has been selected.
        /// </summary>
        public List<Node> ReachableNodes => _reachableNodes;

        /// <summary>
        /// A list of nodes within the units range for their weapon.
        /// </summary>
        public List<Node> RangeNodes => _rangeNodes;

        /// <summary>
        /// A list of the valid stand positions.
        /// This list is populated by taking all reachable nodes and taking out all occupied nodes.
        /// </summary>
        public List<Node> ValidStandNodes => _validStandNodes;

        /// <summary>
        /// A list of all the interactive nodes.
        /// This list is populated by taking all the valid stand nodes and additionally adding in .. todo
        /// </summary>
        public List<Node> InteractiveNodes => _interactiveNodes;

        public List<Node> AttackNodes => _attackNodes;

        public List<Node> TrueReachableNodes => _trueReachableNodes;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void InitializeNodes()
        {
            foreach (NodeManager nodeManager in _gridManager.AllNodes)
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

        private void AddNodeToRange(Node node)
        {
            if (!_rangeNodes.Contains(node))
            {
                _rangeNodes.Add(node);
            }
        }

        private void EnqueueNeighbours(Node current, PriorityQueue<Node> queue)
        {
            // Loop through each neighbor of the current node
            foreach (Node neighbor in current.Neighbors)
            {
                if (neighbor.Visited)
                {
                    continue; // Skip already visited neighbors
                }

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
            foreach (NodeManager nodeManager in _gridManager.AllNodes)
            {
                nodeManager.Node.Visited = false;
                nodeManager.Node.Distance = int.MaxValue;
            }
        }

        /// <summary>
        /// Calculates the movement range of a unit using Dijkstra's Algorithm.
        /// </summary>
        /// <param name="unit">The unit whose movement you want to calculate.</param>
        /// <returns>A list of nodes representing the reachable area.</returns>
        public List<Node> CalculateMovementRange(UnitManager unit)
        {
            _unitStoodNode = unit.StoodNode.Node;
            var movementPoints = unit.UnitData.MovementBaseValue;

            return CalculateMovementRange(_unitStoodNode, movementPoints, unit.UnitData.EquippedWeapon.WeaponMaxRange);
        }

        /// <summary>
        /// Calculates the movement range of a unit using Dijkstra's Algorithm.
        /// </summary>
        /// <param name="startingNode">The starting node.</param>
        /// <param name="movementPoints">The maximum movement points of the unit.</param>
        /// <returns>A list of nodes representing the reachable area.</returns>
        public List<Node> CalculateMovementRange(Node startingNode, int movementPoints, int weaponRange)
        {
            InitializeNodes();

            // Initialize the starting node's distance to 0
            startingNode.Distance = 0;

            // Priority queue to select the node with the smallest distance
            var queue = new PriorityQueue<Node>();
            queue.Enqueue(startingNode, startingNode.Distance);

            AddNodeToReachable(startingNode);

            while (!queue.IsEmpty())
            {
                // Get the node with the smallest distance
                Node current = queue.Dequeue();

                if (current.Visited)
                {
                    continue;
                }

                current.Visited = true;

                // If the current node is within movement points, add to reachable nodes
                CalculateReachableNodes(current, movementPoints);
                CalculateRangeNodes(current, weaponRange);

                EnqueueNeighbours(current, queue);
            }

            PopulateStandNodes();
            PopulateInteractiveNodes();
            PopulateAttackNodes();
            PopulateTrueReachableNodes();

            ResetNodeStates();

            return _reachableNodes;
        }

        private void CalculateReachableNodes(Node current, int movementPoints)
        {
            if (current.Distance <= movementPoints)
            {
                AddNodeToReachable(current);
            }
        }

        private void CalculateRangeNodes(Node current, int weaponRange)
        {
            if (current.Distance <= weaponRange)
            {
                AddNodeToRange(current);
            }
        }

        private void PopulateStandNodes()
        {
            foreach (Node node in _reachableNodes)
            {
                if (!node.IsOccupied)
                {
                    if (!_validStandNodes.Contains(node))
                    {
                        _validStandNodes.Add(node);
                    }                   
                }
            }
        }

        private void PopulateInteractiveNodes()
        {
            foreach (Node node in _reachableNodes)
            {
                if (node.IsOccupied)
                {
                    if (!_interactiveNodes.Contains(node))
                    {
                        _interactiveNodes.Add(node);
                    }
                }
            }
        }

        private void PopulateAttackNodes()
        {
            foreach (Node node in _validStandNodes)
            {
                foreach (Node neighbour in node.Neighbors)
                {
                    if (!_attackNodes.Contains(neighbour))
                    {
                        _attackNodes.Add(neighbour);
                    }
                }
            }

            foreach (Node neighbour in _unitStoodNode.Neighbors)
            {
                if (!_attackNodes.Contains(neighbour))
                {
                    _attackNodes.Add(neighbour);
                }
            }
        }

        private void PopulateTrueReachableNodes()
        {
            foreach (Node node in _reachableNodes)
            {
                if (_attackNodes.Contains(node))
                {
                    if (!_trueReachableNodes.Contains(node))
                    {
                        _trueReachableNodes.Add(node);
                    }
                }
            }
        }

        public List<Node> ReconstructPath(Node start, Node target)
        {
            List<Node> path = new List<Node>();
            Node current = target;

            int safetyCounter = 0;
            int maxIterations = 1000;

            while (current != null && current != start && safetyCounter < maxIterations)
            {
                path.Add(current);
                current = current.Predecessor;
                safetyCounter++;
            }

            if (safetyCounter >= maxIterations)
            {
                return new List<Node>();
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
            _rangeNodes.Clear();
            _validStandNodes.Clear();
            _interactiveNodes.Clear();
            _attackNodes.Clear();
            _trueReachableNodes.Clear();
        }
    }
}
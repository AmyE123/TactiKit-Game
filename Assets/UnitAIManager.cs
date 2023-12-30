namespace CT6GAMAI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnitAIManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;

        [Header("Utility Theory Information")]
        [SerializeField] private AIPlaystyleWeighting _playstyle;
        [SerializeField] private int _unitPowerAmount;
        [SerializeField] private int _unitCurrentHealth;
        [SerializeField] private List<VisibleUnitDetails> _visibleUnitsDetails = new List<VisibleUnitDetails>();
        [SerializeField] private List<VisibleTerrainDetails> _visibleUniqueTerrainDetails = new List<VisibleTerrainDetails>();

        [Header("Utility Theory Desirabilities")]
        [SerializeField] private int _fortDesirability;
        [SerializeField] private int _retreatDesirability;
        [SerializeField] private int _attackDesirability;
        [SerializeField] private int _waitDesirability;

        private GameManager _gameManager;
        private UnitStatsManager _unitStatsManager;

        public AIPlaystyleWeighting Playstyle => _playstyle;
        public int UnitPowerAmount => _unitPowerAmount;
        public int UnitCurrentHealth => _unitCurrentHealth;
        public UnitStatsManager UnitStatsManager => _unitStatsManager;
        
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitPowerAmount = BattleCalculator.CalculatePower(_unitManager);
            _unitStatsManager = _unitManager.UnitStatsManager;
        }

        private void Update() 
        {
            _unitCurrentHealth = _unitManager.UnitStatsManager.HealthPoints;
        }

        public IEnumerator BeginEnemyAI()
        {            
            _gameManager.GridManager.GridCursor.MoveCursorTo(_unitManager.StoodNode.Node);           
            yield return StartCoroutine(EnemyAITurn());
        }

        private IEnumerator EnemyAITurn()
        {            
            yield return new WaitForSeconds(2);

            GetVisiblePlayerUnits();
            GetVisibleUniqueTerrain();
            //MoveUnitToRandomValidNodeWithinRange();
            _fortDesirability = AIDesirabilityCalculator.CalculateFortDesirability(this);
            _retreatDesirability = AIDesirabilityCalculator.CalculateRetreatDesirability(this);
            _attackDesirability = AIDesirabilityCalculator.CalculateAttackDesirability(this);
            _waitDesirability = AIDesirabilityCalculator.CalculateWaitDesirability(this);

            _gameManager.UIManager.UI_DebugDesirabilityManager.PopulateDebugDesirability(this);

            yield return new WaitForSeconds(2);
        }

        private void MoveUnitToRandomValidNodeWithinRange()
        {
            List<Node> nodes = _unitManager.MovementRange.ReachableNodes;
            List<Node> steppableNodes = new List<Node>();

            foreach (Node node in nodes) 
            {
                if (node.NodeManager.StoodUnit == null)
                {
                    steppableNodes.Add(node);
                }
            }

            if (steppableNodes.Count > 0)
            {
                int rand = UnityEngine.Random.Range(1, steppableNodes.Count);
                Node randNode = steppableNodes[rand];
                _unitManager.MoveUnitToNode(randNode);
            }
        }

        private bool IsPlayerUnitVisible()
        {
            var range = _unitManager.MovementRange.ReachableNodes;

            foreach (Node node in range)
            {
                var stoodUnit = node.NodeManager.StoodUnit;
                if (stoodUnit != null)
                {
                    if(stoodUnit.UnitData.UnitTeam == Constants.Team.Player)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<VisibleUnitDetails> GetVisiblePlayerUnits()
        {
            var range = _unitManager.MovementRange.ReachableNodes;
            _visibleUnitsDetails.Clear();

            foreach (Node node in range)
            {
                var stoodUnit = node.NodeManager.StoodUnit;
                if (stoodUnit != null)
                {
                    if (stoodUnit.UnitData.UnitTeam == Constants.Team.Player)
                    {
                        _visibleUnitsDetails.Add(new VisibleUnitDetails(stoodUnit, GetDistanceToUnit(stoodUnit)));
                    }
                }
            }

            return _visibleUnitsDetails;
        }

        private List<VisibleTerrainDetails> GetVisibleUniqueTerrain()
        {
            var range = _unitManager.MovementRange.ReachableNodes;
            _visibleUniqueTerrainDetails.Clear();

            foreach (Node node in range)
            {
                var terrainType = node.NodeManager.NodeData.TerrainType.TerrainType;

                if (!(terrainType == Constants.Terrain.Plain || terrainType == Constants.Terrain.Unwalkable))
                {
                    _visibleUniqueTerrainDetails.Add(new VisibleTerrainDetails(node.NodeManager, GetDistanceToNode(node.NodeManager)));
                }
            }

            return _visibleUniqueTerrainDetails;
        }

        private int GetDistanceToUnit(UnitManager unit)
        {
            Node startNode = _unitManager.StoodNode.Node;
            Node targetNode = unit.StoodNode.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        private int GetDistanceToNode(NodeManager node)
        {
            Node startNode = _unitManager.StoodNode.Node;
            Node targetNode = node.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        private float GetDistanceToNearestUniqueTerrainType(Constants.Terrain terrainTarget)
        {
            List<VisibleTerrainDetails> allTerrain = GetVisibleUniqueTerrain();
            List<VisibleTerrainDetails> uniqueTerrain = new List<VisibleTerrainDetails>();

            foreach (VisibleTerrainDetails terrainDetails in allTerrain)
            {
                Constants.Terrain terrainType = terrainDetails.TerrainNode.NodeData.TerrainType.TerrainType;
                if (terrainType == terrainTarget)
                {
                    uniqueTerrain.Add(terrainDetails);
                }
            }

            float closestDistance = Constants.MAX_NODE_COST;

            foreach (VisibleTerrainDetails terrain in uniqueTerrain)
            {
                if (terrain.Distance < closestDistance)
                {
                    closestDistance = terrain.Distance;
                }
            }

            return closestDistance;
        }

        public float GetDistanceToBestSafeSpot()
        {
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            float maxMinDistanceToPlayer = 0; // Store the maximum of the minimum distances to players
            Node furthestNode = null; // This will store the node furthest from any player

            foreach (Node node in movementRange)
            {
                float minDistanceToPlayer = float.MaxValue; // Initialize with a large value

                foreach (VisibleUnitDetails playerUnitDetails in _visibleUnitsDetails)
                {
                    // Calculate distance from the current node to this player unit
                    float distanceToPlayer = GetDistanceToUnit(playerUnitDetails.Unit, node);

                    // Update the minimum distance for this node
                    if (distanceToPlayer < minDistanceToPlayer)
                    {
                        minDistanceToPlayer = distanceToPlayer;
                    }
                }

                // Update the furthest node if this node is more distant than current furthest
                if (minDistanceToPlayer > maxMinDistanceToPlayer)
                {
                    maxMinDistanceToPlayer = minDistanceToPlayer;
                    furthestNode = node;
                }
            }

            return furthestNode != null ? maxMinDistanceToPlayer : -1; // Return -1 if no safe spot is found
        }

        private int GetDistanceToUnit(UnitManager unit, Node fromNode)
        {
            Node startNode = fromNode;
            Node targetNode = unit.StoodNode.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        /// <summary>
        /// Gets the distance to the nearest fort to the Unit.
        /// </summary>
        /// <returns>The distance to the nearest fort.</returns>
        public float GetDistanceToNearestFort()
        {
            return GetDistanceToNearestUniqueTerrainType(Constants.Terrain.Fort);
        }

        /// <summary>
        /// Gets the distance to the nearest player to the Unit.
        /// </summary>
        /// <returns>The distance to the nearest player.</returns>
        public float GetDistanceToNearestPlayer()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();

            float closestDistance = Constants.MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                if (unit.Distance < closestDistance)
                {
                    closestDistance = unit.Distance;
                }
            }

            return closestDistance;
        }

        public UnitManager GetNearestPlayer()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();
            UnitManager closestUnit = null;

            float closestDistance = Constants.MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                if (unit.Distance < closestDistance)
                {
                    closestUnit = unit.Unit;
                }
            }

            return closestUnit;
        }

        public bool ArePlayersVisible()
        {
            return _visibleUnitsDetails.Count > 0;
        }

        public float CalculatePowerAdvantage(UnitManager opponent)
        {           
            int opponentPower = BattleCalculator.CalculatePower(opponent);
            int unitPower = BattleCalculator.CalculatePower(_unitManager, opponent);
            float powerAdvantage = unitPower - opponentPower;

            // Normalize power advantage, assuming a maximum expected power difference
            float maxPowerDifference = 50;
            powerAdvantage = Mathf.Clamp(powerAdvantage, -maxPowerDifference, maxPowerDifference);
            powerAdvantage = (powerAdvantage + maxPowerDifference) / (2 * maxPowerDifference);

            return powerAdvantage;
        }

    }

    [Serializable]
    public class VisibleUnitDetails
    {
        public UnitManager Unit;
        public float Distance;

        public VisibleUnitDetails(UnitManager unit, float distance)
        {
            Unit = unit;
            Distance = distance;
        }
    }

    [Serializable]
    public class VisibleTerrainDetails
    {
        public NodeManager TerrainNode;
        public float Distance;

        public VisibleTerrainDetails(NodeManager terrainNode, float distance)
        {
            TerrainNode = terrainNode;
            Distance = distance;
        }
    }
}
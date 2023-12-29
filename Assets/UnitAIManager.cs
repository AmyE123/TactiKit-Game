namespace CT6GAMAI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnitAIManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;

        [Header("Utility Theory Desirability Stuff")]
        [SerializeField] private int _unitPowerAmount;
        [SerializeField] private int _unitCurrentHealth;
        [SerializeField] private List<VisibleUnitDetails> _visibleUnitsDetails = new List<VisibleUnitDetails>();
        [SerializeField] private List<VisibleTerrainDetails> _visibleUniqueTerrainDetails = new List<VisibleTerrainDetails>();

        private GameManager _gameManager;
        
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitPowerAmount = BattleCalculator.CalculatePower(_unitManager);            
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
            
            //GetVisiblePlayerUnits();
            //GetVisibleUniqueTerrain();
            //MoveUnitToRandomValidNodeWithinRange();

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
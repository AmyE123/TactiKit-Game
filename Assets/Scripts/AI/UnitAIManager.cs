namespace CT6GAMAI
{
    using CT6GAMAI.BehaviourTrees;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class UnitAIManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;

        [Header("AI Information")]
        [SerializeField] private UnitManager _targetUnit;
        [SerializeField] private bool _nextTargetOverride = false;
        public bool IsMoving;

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

        private BTNode _topNode;
        private GameManager _gameManager;
        private UnitStatsManager _unitStatsManager;

        public bool NextTargetOverride { get { return _nextTargetOverride; } set { _nextTargetOverride = value; } }
        
        public bool HasAttacked { get; set; } = false;

        /// <summary>
        /// The AI's playstyle. Aggressive, Normal, Easy, etc.
        /// </summary>
        public AIPlaystyleWeighting Playstyle => _playstyle;

        /// <summary>
        /// The amount of power this unit has. Useful when determining attack desirability.
        /// </summary>
        public int UnitPowerAmount => _unitPowerAmount;

        /// <summary>
        /// The current health this unit has. Useful when determining desirabilities based on health.
        /// </summary>
        public int UnitCurrentHealth => _unitCurrentHealth;

        /// <summary>
        /// The unit which this AI unit is currently targetting. This is always updated even if the unit doesn't have a desire to attack.
        /// </summary>
        public UnitManager TargetUnit => _targetUnit;

        /// <summary>
        /// A reference to unit manager.
        /// </summary>
        public UnitManager UnitManager => _unitManager;

        /// <summary>
        /// A reference to unit stats.
        /// </summary>
        public UnitStatsManager UnitStatsManager => _unitStatsManager;

        
        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitPowerAmount = BattleCalculator.CalculatePower(_unitManager);
            _unitStatsManager = _unitManager.UnitStatsManager;

            ConstructBehaviourTree();
        }

        private void ConstructBehaviourTree()
        {
            MoveToSafeSpaceNode moveToSafeSpaceNode = new MoveToSafeSpaceNode(this);
            MoveToAttackPositionNode moveToAttackPositionNode = new MoveToAttackPositionNode(this);
            MoveToFortNode moveToFortNode = new MoveToFortNode(this);
            AttackTargetNode attackTargetNode = new AttackTargetNode(this);
            WaitNode waitNode = new WaitNode(this);
            DefaultNode defaultNode = new DefaultNode(this);

            CheckAttackDesirabilityNode checkAttackDesirabilityNode = new CheckAttackDesirabilityNode(this);
            CheckFortDesirabilityNode checkFortDesirabilityNode = new CheckFortDesirabilityNode(this);
            CheckRetreatDesirabilityNode checkRetreatDesirabilityNode = new CheckRetreatDesirabilityNode(this);
            CheckWaitDesirabilityNode checkWaitDesirabilityNode = new CheckWaitDesirabilityNode(this);

            Sequence waitSequence = new Sequence(new List<BTNode> { checkWaitDesirabilityNode, waitNode }, this, "Wait Sequence");
            Sequence attackSequence = new Sequence(new List<BTNode> { checkAttackDesirabilityNode, moveToAttackPositionNode, attackTargetNode }, this, "Attack Sequence");
            Sequence fortSequence = new Sequence(new List<BTNode> { checkFortDesirabilityNode, moveToFortNode }, this, "Fort Sequence");
            Sequence retreatSequence = new Sequence(new List<BTNode> { checkRetreatDesirabilityNode, moveToSafeSpaceNode }, this, "Retreat Sequence");
            Sequence defaultSequence = new Sequence(new List<BTNode> { defaultNode }, this, "Default Sequence");

            _topNode = new Selector(new List<BTNode> { retreatSequence, fortSequence, attackSequence, waitSequence, defaultSequence });
        }

        private void Update() 
        {           
            _unitCurrentHealth = _unitManager.UnitStatsManager.HealthPoints;
            GetBestUnitToAttack();
        }

        public void ChangePlaystyle(Playstyle newPlaystyle)
        {
            foreach (AIPlaystyleWeighting playstyle in _gameManager.AIManager.Playstyles)
            {
                if (playstyle.Playstyle == newPlaystyle)
                {
                    _playstyle = playstyle;
                }
            }           
        }

        public IEnumerator BeginEnemyAI()
        {            
            _gameManager.GridManager.GridCursor.MoveCursorTo(_unitManager.StoodNode.Node);           
            yield return StartCoroutine(EnemyAITurn());
        }

        public void UpdateDebugActiveActionUI(string activeAction)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveAction(activeAction);
        }

        public void UpdateDebugActiveActionStateUI(BTNodeState activeActionState)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveActionState(activeActionState);
        }

        public void UpdateDebugActiveSequenceUI(string activeSequence)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveSequence(activeSequence);
        }

        public void UpdateDebugActiveAIUI(string activeAI)
        {
            _gameManager.UIManager.UI_DebugBehaviourTree.UpdateActiveAIUnit(activeAI);
        }

        private IEnumerator EnemyAITurn()
        {
            HasAttacked = false;
            _unitManager.IsInBattle = false;
            yield return new WaitForSeconds(2);

            GetVisiblePlayerUnits();
            GetVisibleUniqueTerrain();

            _fortDesirability = AIDesirabilityCalculator.CalculateFortDesirability(this);
            _retreatDesirability = AIDesirabilityCalculator.CalculateRetreatDesirability(this);
            _attackDesirability = AIDesirabilityCalculator.CalculateAttackDesirability(this);
            _waitDesirability = AIDesirabilityCalculator.CalculateWaitDesirability(this);

            _gameManager.UIManager.UI_DebugDesirabilityManager.PopulateDebugDesirability(this);

            UpdateDebugActiveAIUI(_unitManager.UnitData.UnitName);

            // Loop to continuously evaluate the behaviour tree
            BTNodeState state = _topNode.Evaluate();

            while (state == BTNodeState.RUNNING)
            {
                yield return null; // Wait for the next frame
                state = _topNode.Evaluate(); // Re-evaluate the tree

                if (_unitManager.UnitDead)
                {
                    yield return new WaitForSeconds(3);
                    state = BTNodeState.SUCCESS;
                }
            }            
        }

        public bool IsOnNode(Node node)
        {
            return _unitManager.StoodNode == node;
        }

        public bool Wait()
        {
            _unitManager.FinalizeMovementValues(true);
            return true;
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
                _unitManager.MoveUnitToNode(randNode, false);
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

        private Node GetNearestUniqueTerrainNode(Constants.Terrain terrainTarget)
        {
            List<VisibleTerrainDetails> allTerrain = GetVisibleUniqueTerrain();
            List<VisibleTerrainDetails> uniqueTerrain = new List<VisibleTerrainDetails>();
            Node closestNode = null;

            foreach (VisibleTerrainDetails terrainDetails in allTerrain)
            {
                Constants.Terrain terrainType = terrainDetails.TerrainNode.NodeData.TerrainType.TerrainType;
                if (terrainType == terrainTarget)
                {
                    uniqueTerrain.Add(terrainDetails);
                }
            }

            float closestDistance = MAX_NODE_COST;
            
            foreach (VisibleTerrainDetails terrain in uniqueTerrain)
            {
                if (terrain.Distance < closestDistance && terrain.TerrainNode.Node.NodeManager.StoodUnit == null)
                {
                    closestDistance = terrain.Distance;
                    closestNode = terrain.TerrainNode.Node;
                }
            }

            return closestNode;
        }

        public bool AttackTargetUnit()
        {
            if (_targetUnit != null && !HasAttacked)
            {
                _unitManager.IsInBattle = true;

                _gameManager.BattleManager.CalculateValuesForBattleForecast(_targetUnit, _unitManager);
                _gameManager.BattleManager.SwitchToBattle(Team.Enemy);
                
                HasAttacked = true; 
                
                return true;
            }
            else
            {
                return false;
            }
        }

        public float GetDistanceToBestSafeSpot()
        {
            GetVisiblePlayerUnits();
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            float maxMinDistanceToPlayer = 0;
            Node furthestNode = null;

            foreach (Node node in movementRange)
            {
                float minDistanceToPlayer = float.MaxValue;

                foreach (VisibleUnitDetails playerUnitDetails in _visibleUnitsDetails)
                {
                    float distanceToPlayer = GetDistanceToUnit(playerUnitDetails.Unit, node);

                    if (distanceToPlayer < minDistanceToPlayer)
                    {
                        minDistanceToPlayer = distanceToPlayer;
                    }
                }

                if (minDistanceToPlayer > maxMinDistanceToPlayer)
                {
                    maxMinDistanceToPlayer = minDistanceToPlayer;
                    furthestNode = node;
                }
            }

            return furthestNode != null ? maxMinDistanceToPlayer : -1;
        }

        public Node GetBestSafeSpot()
        {
            GetVisiblePlayerUnits();
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            float maxMinDistanceToPlayer = 0;
            Node furthestNode = null;

            foreach (Node node in movementRange)
            {
                float minDistanceToPlayer = float.MaxValue;

                foreach (VisibleUnitDetails playerUnitDetails in _visibleUnitsDetails)
                {
                    float distanceToPlayer = GetDistanceToUnit(playerUnitDetails.Unit, node);

                    if (distanceToPlayer < minDistanceToPlayer)
                    {
                        minDistanceToPlayer = distanceToPlayer;
                    }
                }

                if (minDistanceToPlayer > maxMinDistanceToPlayer && node.NodeManager.StoodUnit == null)
                {
                    maxMinDistanceToPlayer = minDistanceToPlayer;
                    furthestNode = node;
                }
            }

            return furthestNode;
        }

        private int GetDistanceToUnit(UnitManager unit, Node fromNode)
        {
            Node startNode = fromNode;
            Node targetNode = unit.StoodNode.Node;

            var path = _unitManager.MovementRange.ReconstructPath(startNode, targetNode);
            return path.Count;
        }

        public Constants.Action GetHighestDesirabilityAction()
        {
            int maxDesirability = _fortDesirability;
            List<Constants.Action> actions = new List<Constants.Action> { Constants.Action.Fort };

            // Check each desirability, and if it's equal to the current max, add it to the list.
            // This accounts for when 2 desirabilities values are the same but they're the highest number still.
            // If it's greater, reset the list and update the maxDesirability.
            if (_retreatDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _retreatDesirability;
                actions.Add(Constants.Action.Retreat);
            }
            else if (_retreatDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Retreat);
            }

            if (_attackDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _attackDesirability;
                actions.Add(Constants.Action.Attack);
            }
            else if (_attackDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Attack);
            }

            if (_waitDesirability > maxDesirability)
            {
                actions.Clear();
                maxDesirability = _waitDesirability;
                actions.Add(Constants.Action.Wait);
            }
            else if (_waitDesirability == maxDesirability)
            {
                actions.Add(Constants.Action.Wait);
            }

            // If there's a tie, choose an action at random from the list of highest actions.
            if (actions.Count > 1)
            {
                int randomIndex = UnityEngine.Random.Range(0, actions.Count);
                return actions[randomIndex];
            }

            // If there's no tie, return the single highest action.
            return actions[0];
        }

        /// <summary>
        /// Gets the distance to the nearest fort to the Unit.
        /// </summary>
        /// <returns>The distance to the nearest fort.</returns>
        public float GetDistanceToNearestFort()
        {
            return GetDistanceToNearestUniqueTerrainType(Constants.Terrain.Fort);
        }

        public Node GetNearestFort()
        {
            return GetNearestUniqueTerrainNode(Constants.Terrain.Fort);
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

        public UnitManager GetBestUnitToAttack()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();
            List<VisibleUnitDetails> hurtUnits = new List<VisibleUnitDetails>();
            bool isAnyUnitHurt = false;

            foreach (VisibleUnitDetails unit in allUnits) 
            {
                var unitStats = unit.Unit.UnitStatsManager;

                if (unitStats.HealthPoints < unitStats.UnitBaseData.HealthPointsBaseValue)
                {
                    hurtUnits.Add(unit);
                    isAnyUnitHurt = true;                   
                }
            }

            if (!_nextTargetOverride)
            {

                if (isAnyUnitHurt)
                {
                    _targetUnit = GetNearestPlayer(hurtUnits);
                }
                else
                {
                    _targetUnit = GetNearestPlayer();
                }
            }

            return _targetUnit;
        }

        public UnitManager GetNearestPlayer()
        {
            List<VisibleUnitDetails> allUnits = GetVisiblePlayerUnits();
            UnitManager closestUnit = null;

            float closestDistance = MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in allUnits)
            {
                if (unit.Distance < closestDistance)
                {
                    closestUnit = unit.Unit;
                }
            }

            return closestUnit;
        }

        public UnitManager GetNearestPlayer(List<VisibleUnitDetails> units)
        {
            UnitManager closestUnit = null;

            float closestDistance = MAX_NODE_COST;

            foreach (VisibleUnitDetails unit in units)
            {
                if (unit.Distance < closestDistance)
                {
                    closestUnit = unit.Unit;
                }
            }

            return closestUnit;
        }

        public UnitManager FindNextTarget()
        {
            if (_visibleUnitsDetails.Count > 1)
            {
                foreach (VisibleUnitDetails unit in _visibleUnitsDetails)
                {
                    if (unit.Unit != _targetUnit)
                    {
                        Debug.Log("[AE] Getting next target unit. " + _targetUnit.name + " to " + unit.Unit.UnitData.name);
                        _nextTargetOverride = true;
                        _targetUnit = unit.Unit;
                        return _targetUnit;
                    }
                }
            }

            return null;           
        }

        public bool CanMoveToTargetAttackSpot(UnitManager target)
        {
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            List<Node> targetAttackSpots = new List<Node>();
            List<Node> validAttackSpots = new List<Node>();

            if (target.StoodNode.NorthNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.NorthNode.Node);
            }

            if (target.StoodNode.EastNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.EastNode.Node);
            }

            if (target.StoodNode.SouthNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.SouthNode.Node);
            }

            if (target.StoodNode.WestNode != null)
            {
                targetAttackSpots.Add(target.StoodNode.WestNode.Node);
            }

            foreach (Node n in targetAttackSpots)
            {
                if (n == _unitManager.StoodNode.Node)
                {
                    validAttackSpots.Add(n);
                }

                if (movementRange.Contains(n))
                {
                    if (n.NodeManager.StoodUnit == null)
                    {
                        validAttackSpots.Add(n);
                    }
                    else
                    {
                        if (n.NodeManager.StoodUnit == this)
                        {
                            validAttackSpots.Add(n);
                        }
                    }
                }
            }

            return validAttackSpots.Count > 0;
        }

        public Node GetPlayerValidAttackSpot(UnitManager player)
        {
            List<Node> movementRange = _unitManager.MovementRange.ReachableNodes;
            List<Node> attackSpots = new List<Node>();
            List<Node> validAttackSpots = new List<Node>();

            if (player.StoodNode.NorthNode != null)
            {
                attackSpots.Add(player.StoodNode.NorthNode.Node);
            }

            if (player.StoodNode.EastNode != null)
            {
                attackSpots.Add(player.StoodNode.EastNode.Node);
            }

            if (player.StoodNode.SouthNode != null)
            {
                attackSpots.Add(player.StoodNode.SouthNode.Node);
            }

            if (player.StoodNode.WestNode != null)
            {
                attackSpots.Add(player.StoodNode.WestNode.Node);
            }

            foreach (Node n in attackSpots)
            {
                if (n == _unitManager.StoodNode.Node)
                {
                    return n;
                }

                if (movementRange.Contains(n))
                {
                    if (n.NodeManager.StoodUnit == null)
                    {
                        validAttackSpots.Add(n);
                    }
                    else
                    {
                        if (n.NodeManager.StoodUnit == this)
                        {
                            return n;
                        }
                    }
                }
            }

            return validAttackSpots[UnityEngine.Random.Range(0, validAttackSpots.Count)];
        }

        public bool ArePlayersVisible()
        {
            GetVisiblePlayerUnits();
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

        public bool MoveUnitTo(Node node, bool isAttacking)
        {
            return _unitManager.MoveUnitToNode(node, isAttacking);
        }

        public bool MoveUnitTo(Node node, bool shouldFinalize, bool isAttacking)
        {
            return _unitManager.MoveUnitToNode(node, shouldFinalize, isAttacking, this);
        }

        public void StartMovingTo(Node targetNode, bool isAttacking)
        {
            IsMoving = true;
            _unitManager.MoveUnitToNode(targetNode, this, isAttacking);
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
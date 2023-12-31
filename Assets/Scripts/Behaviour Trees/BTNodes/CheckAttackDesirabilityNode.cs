namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'checkAttackDesirabilityNode' node on the behaviour tree.
    /// This node checks if the most desirable action for a unit is to attack.
    /// </summary>
    public class CheckAttackDesirabilityNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the CheckAttackDesirabilityNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public CheckAttackDesirabilityNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Evaluates the node by checking whether attacking is the highest desirability action for the unit.
        /// Returns SUCCESS if attacking is the most desirable action, otherwise it returns FAILURE.
        /// </summary>
        /// <returns>The state of the node after evaluation - SUCCESS if attack is desirable, otherwise FAILURE.</returns>
        public override BTNodeState Evaluate()
        {           
            return _unitAI.GetHighestDesirabilityAction() == Action.Attack ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }
    }
}
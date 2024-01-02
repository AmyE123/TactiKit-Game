namespace CT6GAMAI.BehaviourTrees
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'moveToFortNode' node on the behaviour tree.
    /// This node moves the unit to a nearby fort.
    /// </summary>
    public class MoveToFortNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the MoveToFortNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public MoveToFortNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Moves to a fort position whilst evaluating the states.
        /// </summary>
        /// <returns>
        /// Once the unit reaches the fort position, it returns SUCCESS.
        /// If this isn't able to be done, return FAILURE.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            _unitAI.UpdateDebugActiveActionUI(GetType().Name);

            return _unitAI.MoveUnitTo(_unitAI.GetNearestFort(), false) ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }

    }

}
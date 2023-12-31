namespace CT6GAMAI.BehaviourTrees
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// 'moveToSafeSpaceNode' node on the behaviour tree.
    /// This node moves the unit to a nearby safe spot.
    /// </summary>
    public class MoveToSafeSpaceNode : BTNode
    {
        private UnitAIManager _unitAI;

        /// <summary>
        /// Initializes the MoveToSafeSpaceNode class with a reference to the AI manager.
        /// </summary>
        /// <param name="unitAI">The AI which is attacking.</param>
        public MoveToSafeSpaceNode(UnitAIManager unitAI)
        {
            _unitAI = unitAI;
        }

        /// <summary>
        /// Moves to a safe position whilst evaluating the states.
        /// </summary>
        /// <returns>
        /// Once the unit reaches the safe position, it returns SUCCESS.
        /// If this isn't able to be done, return FAILURE.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            Debug.Log("[AI - BT]:" + _unitAI.name + "In MoveToSafeSpace Node");
            return _unitAI.MoveUnitTo(_unitAI.GetBestSafeSpot()) ? BTNodeState.SUCCESS : BTNodeState.FAILURE;
        }

    }

}
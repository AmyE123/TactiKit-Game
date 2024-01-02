namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// This represents a single node in a behaviour tree.
    /// </summary>
    [System.Serializable]
    public abstract class BTNode
    {
        /// <summary>
        /// The current state of the node.
        /// </summary>
        protected BTNodeState _btNodeState;

        /// <summary>
        /// A getter for the current state of the node.
        /// </summary>
        public BTNodeState BTNodeState => _btNodeState;

        /// <summary>
        /// An evaluate method which should be implemented by node classes with the specific behaviours.
        /// </summary>
        /// <returns>The state after evaluation.</returns>
        public abstract BTNodeState Evaluate();
    }
}
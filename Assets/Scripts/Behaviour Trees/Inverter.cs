namespace CT6GAMAI.BehaviourTrees
{
    using static CT6GAMAI.Constants;

    /// <summary>
    /// An Inverter node in a behaviour tree.
    /// The Inverter node inverts the success and failure states of its child node.
    /// </summary>
    public class Inverter : BTNode
    {
        private BTNode _node;

        /// <summary>
        /// Initializes the inverter class with a child.
        /// </summary>
        /// <param name="node">The nodes which will be inverted</param>
        public Inverter(BTNode node)
        {
            _node = node;
        }

        /// <summary>
        /// Evaluates the child node and inverts its result.
        /// If the child node succeeds, the Inverter fails.
        /// </summary>
        /// <returns>
        /// The inverted state of the child node.
        /// </returns>
        public override BTNodeState Evaluate()
        {
            switch (_node.Evaluate())
            {
                case BTNodeState.RUNNING:
                    _btNodeState = BTNodeState.RUNNING;
                    break;
                case BTNodeState.SUCCESS:
                    _btNodeState = BTNodeState.FAILURE;
                    break;
                case BTNodeState.FAILURE:
                    _btNodeState = BTNodeState.SUCCESS;
                    break;
                default:
                    break;
            }

            return _btNodeState;
        }
    }
}
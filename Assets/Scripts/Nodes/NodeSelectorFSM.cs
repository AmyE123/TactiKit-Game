namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A Finite-State Machine to control the selector state of a node
    /// </summary>
    public class NodeSelectorFSM
    {
        private NodeSelectorState _currentState;

        private NodeSelectorManager _manager;

        public NodeSelectorManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        public NodeSelectorFSM()
        {
            _currentState = NodeSelectorState.NoSelection;
        }

        /// <summary>
        /// Gets the current state of the selector.
        /// </summary>
        /// <returns>The current state of the selector.</returns>
        public NodeSelectorState GetState()
        {
            return _currentState;
        }

        /// <summary>
        /// Change the state of the selector.
        /// </summary>
        /// <param name="newState">The state to change to.</param>
        public void ChangeState(NodeSelectorState newState)
        {
            if (_currentState == newState)
                return;

            // Update the node's appearance based on the new state
            // TODO: Add more visual datas for each state
            switch (newState)
            {
                case NodeSelectorState.NoSelection:
                    break;
                case NodeSelectorState.DefaultSelected:
                    _manager.State.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;

                case NodeSelectorState.PlayerSelected:
                    _manager.State.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;

                case NodeSelectorState.EnemySelected:
                    _manager.State.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;
            }

            Debug.Log("SelectorFSM: Changing State - " + newState);

            _currentState = newState;
        }
    }
}
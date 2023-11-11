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

        private NodeState _nodeState;

        public NodeSelectorManager Manager
        {
            get { return _manager; }
            set { _manager = value; }
        }

        public NodeSelectorFSM()
        {
            _currentState = NodeSelectorState.NoSelection;
        }

        public void Initialize()
        {
            if (_manager != null)
            {
                _nodeState = _manager.State;
            }
        }

        public NodeSelectorState GetState()
        {
            return _currentState;
        }

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
                    _nodeState.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;

                case NodeSelectorState.PlayerSelected:
                    _nodeState.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;

                case NodeSelectorState.EnemySelected:
                    _nodeState.ChangeVisualData(_manager.SelectorSR, _manager.SelectorVisualDatas[0]);
                    break;
            }

            Debug.Log("SelectorFSM: Changing State - " + newState);

            _currentState = newState;
        }
    }
}
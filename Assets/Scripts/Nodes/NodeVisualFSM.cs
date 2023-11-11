namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A Finite-State Machine to control the visual state of a node
    /// </summary>
    public class NodeVisualFSM
    {
        private NodeVisualState _currentState;

        private NodeVisualManager _manager;

        private NodeState _nodeState;

        public NodeVisualManager Manager 
        {
            get { return  _manager; }
            set { _manager = value; }
        }

        public NodeVisualFSM()
        {
            _currentState = NodeVisualState.Default;
        }

        public void Initialize()
        {
            if (_manager != null)
            {
                _nodeState = _manager.State;
            }         
        }

        public NodeVisualState GetState()
        {
            return _currentState;
        }

        public void ChangeState(NodeVisualState newState)
        {
            if (_currentState == newState)
                return;

            // Update the node's appearance based on the new state
            switch (newState)
            {
                case NodeVisualState.Default:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[0]);
                    break;

                case NodeVisualState.HoveredBlue:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[1]);
                    break;

                case NodeVisualState.HoveredRed:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[2]);
                    break;

                case NodeVisualState.HoveredGreen:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[3]);
                    break;

                case NodeVisualState.SelectedBlue:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[4]);
                    break;

                case NodeVisualState.SelectedRed:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[5]);
                    break;

                case NodeVisualState.SelectedGreen:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[6]);
                    break;

                case NodeVisualState.AllEnemyRange:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[7]);
                    break;

                case NodeVisualState.SingularEnemyRange:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[8]);
                    break;

                case NodeVisualState.PointOfInterest:
                    _nodeState.ChangeVisualData(_manager.VisualSR, _manager.VisualDatas[9]);
                    break;
            }

            Debug.Log("VisualFSM: Changing State - " + newState);

            _currentState = newState;
        }
    }
}
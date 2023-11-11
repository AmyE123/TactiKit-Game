namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class NodeVisualManager : MonoBehaviour
    {
        [SerializeField] private NodeVisualFSM visualFSM;

        public NodeState State;
        public NodeStateVisualData[] VisualDatas;
        public SpriteRenderer VisualSR;

        public bool IsActive => _isActive;
        public bool IsDefault => _isDefault;
        public bool IsHovered => _isHovered;
        public bool IsPressed => _isPressed;
        public bool IsPath => _isPath;

        [SerializeField] private bool _isActive;
        [SerializeField] private bool _isDefault;
        [SerializeField] private bool _isHovered;
        [SerializeField] private bool _isPressed;
        [SerializeField] private bool _isPath;

        private void Start()
        {
            Debug.Log("VisualManager: Start");

            visualFSM = new NodeVisualFSM(gameObject);
            visualFSM.Manager = this;
            visualFSM.Initialize();          
        }

        public void SetDefault()
        {
            Debug.Log("VisualManager: SetDefault");

            _isActive = false;
            _isDefault = true;
            _isHovered = false;
            _isPressed = false;
            _isPath = false;

            visualFSM.ChangeState(NodeVisualState.Default);
        }

        public void SetHovered(NodeVisualColorState color)
        {
            Debug.Log("VisualManager: SetHovered");

            _isActive = true;
            _isDefault = false;
            _isHovered = true;
            _isPressed = false;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.HoveredBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.HoveredRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.HoveredGreen);
                    break;
            }          
        }

        public void SetPressed(NodeVisualColorState color)
        {
            Debug.Log("VisualManager: SetPressed");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = true;
            _isPath = false;

            switch (color)
            {
                case NodeVisualColorState.Blue:
                    visualFSM.ChangeState(NodeVisualState.SelectedBlue);
                    break;
                case NodeVisualColorState.Red:
                    visualFSM.ChangeState(NodeVisualState.SelectedRed);
                    break;
                case NodeVisualColorState.Green:
                    visualFSM.ChangeState(NodeVisualState.SelectedGreen);
                    break;
            }        
        }

        public void SetEnemyRange(NodeVisualEnemyColorState color)
        {
            Debug.Log("VisualManager: SetEnemyRange");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = true;
            _isPath = false;

            switch (color)
            {
                case NodeVisualEnemyColorState.SingularEnemy:
                    visualFSM.ChangeState(NodeVisualState.AllEnemyRange);
                    break;
                case NodeVisualEnemyColorState.AllEnemy:
                    visualFSM.ChangeState(NodeVisualState.SingularEnemyRange);
                    break;
            }           
        }

        public void SetPath()
        {
            Debug.Log("VisualManager: SetPath");

            _isActive = true;
            _isDefault = false;
            _isHovered = false;
            _isPressed = false;
            _isPath = true;

            visualFSM.ChangeState(NodeVisualState.PointOfInterest);
        }
    }
}

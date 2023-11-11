namespace CT6GAMAI
{
    using UnityEngine;
    using static CT6GAMAI.Constants;

    public class NodeSelectorManager : MonoBehaviour
    {
        [SerializeField] private NodeSelectorFSM selectorFSM;

        public NodeState State;
        public NodeStateVisualData[] SelectorVisualDatas;
        public SpriteRenderer SelectorSR;

        public bool IsActive => _isActive;
        public bool IsDefaultSelected => _isDefaultSelected;
        public bool IsPlayerSelected => _isPlayerSelected;
        public bool IsEnemySelected => _isEnemySelected;

        [SerializeField] private bool _isActive;
        [SerializeField] private bool _isDefaultSelected;
        [SerializeField] private bool _isPlayerSelected;
        [SerializeField] private bool _isEnemySelected;

        private void Start()
        {
            Debug.Log("SelectorManager: Start");

            selectorFSM = new NodeSelectorFSM(gameObject);
            selectorFSM.Manager = this;
            selectorFSM.Initialize();
        }

        public void SetInactive()
        {
            Debug.Log("SelectorManager: SetInactive");

            _isActive = false;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = false;
        }

        public void SetDefaultSelected()
        {
            Debug.Log("SelectorManager: SetDefaultSelected");

            _isActive = true;
            _isDefaultSelected = true;
            _isPlayerSelected = false;
            _isEnemySelected = false;

            selectorFSM.ChangeState(NodeSelectorState.DefaultSelected);
        }

        public void SetPlayerSelected()
        {
            Debug.Log("SelectorManager: SetPlayerSelected");

            _isActive = true;
            _isDefaultSelected = false;
            _isPlayerSelected = true;
            _isEnemySelected = false;

            selectorFSM.ChangeState(NodeSelectorState.PlayerSelected);
        }

        public void SetEnemySelected()
        {
            Debug.Log("SelectorManager: SetEnemySelected");

            _isActive = true;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = true;

            selectorFSM.ChangeState(NodeSelectorState.EnemySelected);
        }
    }
}

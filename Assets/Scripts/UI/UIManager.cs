namespace CT6GAMAI
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [Header("Information Managers")]
        [SerializeField] private UI_TileInfoManager _tileInfoManager;
        [SerializeField] private UI_UnitInfoManager _unitInfoManager;

        [Header("Battle Forecast UI Management")]
        [SerializeField] private UI_BattleForecastManager _battleForecastManager;
        [SerializeField] private UI_BattleForecastSideManager[] _battleForecastSideManagers;
        [SerializeField] private GameObject[] _uiObjectsToDisableForBattleForecasts;

        [Header("Battle Sequence UI Management")]
        [SerializeField] private UI_BattleSequenceManager _battleSequenceManager;
        [SerializeField] private GameObject _battleAnimationUI;
        [SerializeField] private GameObject[] _uiObjectsToDisableForBattleAnimations;

        [Header("Action UI Management")]
        [SerializeField] private UI_ActionItemsManager _actionItemsManager;
        [SerializeField] private GameObject[] _uiObjectsToDisableForActions;

        [Header("Debug UI Managers")]
        [SerializeField] private UI_DebugDesirabilityManager _debugDesirabilityManager;
        [SerializeField] private UI_DebugBehaviourTree _debugBehaviourTree;
        [SerializeField] private GameObject _debugButtons;
        [SerializeField] private GameObject _debugButtonsCanvas;
        [SerializeField] private GameObject[] _uiDebugObjectsToDisableForPlayerTurn;

        [Header("General UI Elements")]
        [SerializeField] private Image _vignette;

        private GameManager _gameManager;

        public UI_TileInfoManager TileInfoManager => _tileInfoManager;
        public UI_UnitInfoManager UnitInfoManager => _unitInfoManager;
        public UI_BattleForecastSideManager[] BattleForecastManagers => _battleForecastSideManagers;
        public UI_ActionItemsManager ActionItemsManager => _actionItemsManager;
        public UI_BattleForecastManager BattleForecastManager => _battleForecastManager;
        public UI_BattleSequenceManager BattleSequenceManager => _battleSequenceManager;
        public UI_DebugDesirabilityManager UI_DebugDesirabilityManager => _debugDesirabilityManager;
        public UI_DebugBehaviourTree UI_DebugBehaviourTree => _debugBehaviourTree;

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            if (_gameManager != null)
            {
                UpdateAllUIForActionItems();
                UpdateAllUIForBattleForecast();
                UpdateAllUIForBattle();
                UpdateDebugUIForPhases();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                _actionItemsManager.HideActionItems();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _debugDesirabilityManager.gameObject.SetActive(!_debugDesirabilityManager.gameObject.activeSelf);
                _debugBehaviourTree.gameObject.SetActive(!_debugBehaviourTree.gameObject.activeSelf);
                _debugButtons.SetActive(!_debugButtons.activeSelf);
            }
        }

        private void UpdateAllUIForActionItems()
        {
            SetCursorState(ActionItemsManager.IsActionItemsActive);
            SetVignette(ActionItemsManager.IsActionItemsActive);

            foreach (GameObject go in _uiObjectsToDisableForActions)
            {
                go.SetActive(!_actionItemsManager.IsActionItemsActive);
            }
        }

        private void UpdateAllUIForBattleForecast()
        {
            SetVignette(BattleForecastManager.AreBattleForecastsToggled);

            foreach (GameObject go in _uiObjectsToDisableForBattleForecasts)
            {
                go.SetActive(!BattleForecastManager.AreBattleForecastsToggled);
            }
        }

        private void UpdateAllUIForBattle()
        {
            _battleAnimationUI.SetActive(_gameManager.BattleManager.IsBattleActive);
            _debugButtonsCanvas.SetActive(!_gameManager.BattleManager.IsBattleActive);

            if (_gameManager.BattleManager.IsBattleActive)
            {
                SetVignette(true);

                foreach (GameObject go in _uiObjectsToDisableForBattleAnimations)
                {
                    go.SetActive(false);
                }
            }
        }

        private void UpdateDebugUIForPhases()
        {
            foreach (GameObject go in _uiDebugObjectsToDisableForPlayerTurn)
            {
                go.SetActive(_gameManager.TurnManager.ActivePhase != Constants.Phases.PlayerPhase);
            }
        }

        private void SetCursorState(bool isDisabled)
        {
            var gridCursor = _gameManager.GridManager.GridCursor;

            if (gridCursor != null)
            {
                if (gridCursor.SelectedNodeState != null)
                {
                    var cursorStateManager = gridCursor.SelectedNodeState.CursorStateManager;
                    var visualsStateManager = gridCursor.SelectedNodeState.VisualStateManager;

                    if (isDisabled)
                    {
                        cursorStateManager.SetDisabled();

                        foreach (NodeManager NM in _gameManager.GridManager.AllNodes)
                        {
                            NM.NodeState.VisualStateManager.SetDisabled();
                        }
                    }
                    else
                    {
                        cursorStateManager.SetEnabled();

                        foreach (NodeManager NM in _gameManager.GridManager.AllNodes)
                        {
                            NM.NodeState.VisualStateManager.SetEnabled();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the vignette active/inactive.
        /// </summary>
        /// <param name="isActive">Value to set the vignette.</param>
        public void SetVignette(bool isActive)
        {
            if (!isActive)
            {
                _vignette.DOFade(0, Constants.VIGNETTE_FADE_SPEED);
            }
            else
            {
                _vignette.DOFade(1, Constants.VIGNETTE_FADE_SPEED);
            }
        }
    }
}
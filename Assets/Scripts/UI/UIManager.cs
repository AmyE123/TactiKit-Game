namespace CT6GAMAI
{
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UI_TileInfoManager _tileInfoManager;
        [SerializeField] private UI_UnitInfoManager _unitInfoManager;
        [SerializeField] private UI_BattleForecastSideManager[] _battleForecastManagers;
        [SerializeField] private UI_ActionItemsManager _actionItemsManager;
        [SerializeField] private GameObject[] _uiObjectsToDisableForActions;

        [SerializeField] private Image _vignette;

        private GameManager _gameManager;
        private GlobalUnitsManager _unitsManager;
        private bool _vignetteEnabled;

        public UI_TileInfoManager TileInfoManager => _tileInfoManager;
        public UI_UnitInfoManager UnitInfoManager => _unitInfoManager;
        public UI_BattleForecastSideManager[] BattleForecastManagers => _battleForecastManagers;
        public UI_ActionItemsManager ActionItemsManager => _actionItemsManager;        

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _unitsManager = _gameManager.UnitsManager;
        }

        private void Update()
        {
            if (_gameManager != null)
            {
                UpdateAllUIForActionItems();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _actionItemsManager.HideActionItems();
            }
        }

        // TODO: Could THIS be in it's own class? UI_BattleForecastManager?
        public void SpawnBattleForecast(UnitData unitA, UnitData unitB)
        {
            _battleForecastManagers[0].ToggleBattleForecastSide(unitA);
            _battleForecastManagers[1].ToggleBattleForecastSide(unitB);
        }

        public void CancelBattleForecast()
        {
            _battleForecastManagers[0].CancelBattleForecast();
            _battleForecastManagers[1].CancelBattleForecast();
        }

        public void ToggleVignette()
        {
            if (_vignetteEnabled)
            {
                _vignette.DOFade(0, 0.5f);
            }
            else
            {
                _vignette.DOFade(1, 0.5f);
            }
        }

        private void UpdateAllUIForActionItems()
        {
            SetCursorState(ActionItemsManager.IsActionItemsActive);

            foreach (GameObject go in _uiObjectsToDisableForActions)
            {
                go.SetActive(!_actionItemsManager.IsActionItemsActive);
            }
        }

        private void SetCursorState(bool isDisabled)
        {
            var gridCursor = _gameManager.GridManager.GridCursor;

            if (gridCursor != null) 
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
}
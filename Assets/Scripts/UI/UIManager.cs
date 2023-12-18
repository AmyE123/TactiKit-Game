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
        [SerializeField] private Image _battleVignette;

        public UI_TileInfoManager TileInfoManager => _tileInfoManager;
        public UI_UnitInfoManager UnitInfoManager => _unitInfoManager;
        public UI_BattleForecastSideManager[] BattleForecastManagers => _battleForecastManagers;

        public UnitData[] testUnits;

        private void Update()
        {
            // TODO: Write function to deal with toggling battle forecast for both sides.
            // Could be in it's own class? UI_BattleForecastManager, and rename the side ones to UI_BattleForecastSideManager
            if (Input.GetKeyDown(KeyCode.T))
            {
                _battleForecastManagers[0].ToggleBattleForecastSide(testUnits[0]);
                _battleForecastManagers[1].ToggleBattleForecastSide(testUnits[1]);

                // Don't spam T as it will break the fade. This is just test code
                if (_battleForecastManagers[0].IsForecastToggled)
                {
                    _tileInfoManager.gameObject.SetActive(false);
                    _unitInfoManager.gameObject.SetActive(false);
                    _battleVignette.DOFade(1, 0.5f);
                }
                else
                {
                    _tileInfoManager.gameObject.SetActive(true);
                    _unitInfoManager.gameObject.SetActive(true);
                    _battleVignette.DOFade(0, 0.5f);
                }
            }
        }
    }
}
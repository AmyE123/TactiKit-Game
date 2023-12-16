namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class UI_TileInfoManager : MonoBehaviour
    {
        [SerializeField] TMP_Text TerrainValueText;
        [SerializeField] TMP_Text DefenseValueText;
        [SerializeField] TMP_Text MovementValueText;
        [SerializeField] TMP_Text HealthBuffValueText;

        private TerrainData _previousTerrainData;
        public TerrainData ActiveTerrainData;        

        private void RefreshTileInfoUI()
        {
            if(_previousTerrainData != ActiveTerrainData)
            {
                _previousTerrainData = ActiveTerrainData;

                TerrainValueText.text = ActiveTerrainData.TerrainType.ToString();
                DefenseValueText.text = ActiveTerrainData.DefenseBoost.ToString();
                MovementValueText.text = ActiveTerrainData.MovementCost.ToString();
                HealthBuffValueText.text = ActiveTerrainData.HealPercentageBoost.ToString();
            }
        }

        public void SetTerrainType(TerrainData data)
        {
            ActiveTerrainData = data;   
            RefreshTileInfoUI();
        }
    }

}
namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class UI_TileInfoManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _terrainValueText;
        [SerializeField] private TMP_Text _defenseValueText;
        [SerializeField] private TMP_Text _movementValueText;
        [SerializeField] private TMP_Text _healthBuffValueText;

        private TerrainData _previousTerrainData;
        public TerrainData ActiveTerrainData;        

        private void RefreshTileInfoUI()
        {
            if(_previousTerrainData != ActiveTerrainData)
            {
                _previousTerrainData = ActiveTerrainData;

                _terrainValueText.text = ActiveTerrainData.TerrainType.ToString();
                _defenseValueText.text = ActiveTerrainData.DefenseBoost.ToString();
                _movementValueText.text = ActiveTerrainData.MovementCost.ToString();
                _healthBuffValueText.text = ActiveTerrainData.HealPercentageBoost.ToString();
            }
        }

        public void SetTerrainType(TerrainData data)
        {
            ActiveTerrainData = data;   
            RefreshTileInfoUI();
        }
    }
}
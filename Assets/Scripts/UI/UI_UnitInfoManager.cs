namespace CT6GAMAI
{
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_UnitInfoManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nameValueText;
        [SerializeField] private TMP_Text _levelValueText;
        [SerializeField] private TMP_Text _equippedValueText;
        [SerializeField] private Image _equippedValueImage;
        [SerializeField] private Image _teamColourImage;
        [SerializeField] private GameObject _areaGO;

        private UnitData _previousUnitData;

        public UnitData ActiveUnitData;

        private void RefreshUnitInfoUI()
        {
            if (_previousUnitData != ActiveUnitData)
            {
                _previousUnitData = ActiveUnitData;

                SetTeamColourUI();

                _nameValueText.text = ActiveUnitData.UnitName.ToString();
                _levelValueText.text = "LV " + ActiveUnitData.ClassLevel.ToString();

                // TODO: Unit Inventory System
                var formattedWeaponName = Regex.Replace(ActiveUnitData.EquippedWeapon.ToString(), "(\\B[A-Z])", " $1");
                _equippedValueText.text = formattedWeaponName;
                _equippedValueImage.sprite = ActiveUnitData.EquippedWeaponImage;
            }
        }

        private void SetTeamColourUI()
        {
            if (ActiveUnitData.UnitTeam == Constants.Team.Player)
            {
                _teamColourImage.color = Constants.UI_PlayerColour;
            }
            else
            {
                _teamColourImage.color = Constants.UI_EnemyColour;
            }
        }

        public void SetUnitType(UnitData data)
        {
            _areaGO.SetActive(true);
            ActiveUnitData = data;
            RefreshUnitInfoUI();
        }

        public void SetUnitInfoUIInactive()
        {
            _areaGO.SetActive(false);
        }
    }
}
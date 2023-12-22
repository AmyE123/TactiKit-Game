namespace CT6GAMAI
{
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BattleSequenceSideManager : MonoBehaviour
    {
        [SerializeField] private Constants.Side _side;

        [SerializeField] private TMP_Text _equippedWeaponValueText;
        [SerializeField] private Image _equippedWeaponImage;
        [SerializeField] private TMP_Text _unitNameValueText;
        [SerializeField] private TMP_Text _currentHPValueText;
        [SerializeField] private TMP_Text _attackStatValueText;
        [SerializeField] private TMP_Text _hitStatValueText;
        [SerializeField] private TMP_Text _critStatValueText;
        [SerializeField] private Image _healthBarFill;

        [SerializeField] private UnitManager _activeUnitManager;

        private void Update()
        {
            if (_activeUnitManager != null)
            {
                UpdateHealthFill(_activeUnitManager);
            }
        }

        public void PopulateBattleSequenceUIData(UnitManager unit, int attackValue, int hitValue, int critValue)
        {
            _activeUnitManager = unit;

            _unitNameValueText.text = unit.UnitData.UnitName;

            var formattedWeaponName = Regex.Replace(unit.UnitData.EquippedWeapon.WeaponName.ToString(), "(\\B[A-Z])", " $1");
            _equippedWeaponValueText.text = formattedWeaponName;
            _equippedWeaponImage.sprite = unit.UnitData.EquippedWeapon.WeaponSprite;

            _currentHPValueText.text = unit.UnitStatsManager.HealthPoints.ToString();

            _attackStatValueText.text = attackValue.ToString();

            _hitStatValueText.text = hitValue.ToString();

            _critStatValueText.text = critValue.ToString() + "%";

            float healthFillAmount = CalculateHealthPercentage(unit.UnitStatsManager.HealthPoints, unit.UnitData.HealthPointsBaseValue);

            _healthBarFill.fillAmount = healthFillAmount;
        }

        private void UpdateHealthFill(UnitManager unit)
        {
            _currentHPValueText.text = unit.UnitStatsManager.HealthPoints.ToString();

            float healthFillAmount = CalculateHealthPercentage(unit.UnitStatsManager.HealthPoints, unit.UnitData.HealthPointsBaseValue);

            _healthBarFill.fillAmount = healthFillAmount;
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }
    }

}
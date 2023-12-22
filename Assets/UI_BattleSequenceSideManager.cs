namespace CT6GAMAI
{
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BattleSequenceSideManager : MonoBehaviour
    {
        [SerializeField] private Constants.Side _side;
        [SerializeField] private bool _battleSequenceToggled;

        [SerializeField] private TMP_Text _equippedWeaponValueText;
        [SerializeField] private Image _equippedWeaponImage;
        [SerializeField] private TMP_Text _unitNameValueText;
        [SerializeField] private TMP_Text _currentHPValueText;
        [SerializeField] private TMP_Text _attackStatValueText;
        [SerializeField] private TMP_Text _hitStatValueText;
        [SerializeField] private TMP_Text _critStatValueText;
        [SerializeField] private Image _healthBarFill;

        public void PopulateBattleSequenceUIData(UnitData unit, int attackValue, int hitValue, int critValue, int currentHP)
        {
            _unitNameValueText.text = unit.UnitName;

            var formattedWeaponName = Regex.Replace(unit.EquippedWeapon.WeaponName.ToString(), "(\\B[A-Z])", " $1");
            _equippedWeaponValueText.text = formattedWeaponName;
            _equippedWeaponImage.sprite = unit.EquippedWeapon.WeaponSprite;

            _currentHPValueText.text = currentHP.ToString();

            _attackStatValueText.text = attackValue.ToString();

            _hitStatValueText.text = hitValue.ToString();

            _critStatValueText.text = critValue.ToString() + "%";

            float healthFillAmount = CalculateHealthPercentage(currentHP, unit.HealthPointsBaseValue);

            _healthBarFill.fillAmount = healthFillAmount;
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }
    }

}
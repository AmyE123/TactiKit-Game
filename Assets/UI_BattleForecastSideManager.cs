namespace CT6GAMAI
{
    using DG.Tweening;
    using System.Text.RegularExpressions;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_BattleForecastSideManager : MonoBehaviour
    {
        public enum BattleForecastSide { Left, Right };

        [SerializeField] private BattleForecastSide _side;
        [SerializeField] private bool _forecastToggled;

        [SerializeField] private TMP_Text _unitNameValueText;
        [SerializeField] private Image _unitImage;
        [SerializeField] private TMP_Text _equippedWeaponValueText;
        [SerializeField] private Image _equippedWeaponImage;
        [SerializeField] private TMP_Text _currentHPValueText;
        [SerializeField] private TMP_Text _attackStatValueText;
        [SerializeField] private GameObject _doubleAttackGO;
        [SerializeField] private TMP_Text[] _hitStatValueTexts;
        [SerializeField] private TMP_Text _critStatValueText;
        [SerializeField] private TMP_Text _predictedRemainingHPValueText;
        [SerializeField] private Image[] _predictedRemainingHPAreaImages;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Image _healthBarFillDamage;
        [SerializeField] private RectTransform _damageIndicatorRectTransform;
 
        [SerializeField] private Color32 _deadUIColour;
        [SerializeField] private Color32 _aliveUIColour;       

        [SerializeField] private RectTransform _areaRT;

        public bool IsForecastToggled => _forecastToggled;

        private void Start()
        {
            FlashingDamage();
        }

        public void SpawnBattleForecastSide()
        {
            _forecastToggled = true;

            OpenUISide();
        }

        public void CancelBattleForecast()
        {
            _forecastToggled = false;

            CancelUISide();
        }

        public void PopulateBattleForecastData(UnitData unit, int attackValue, bool canDoubleAttack, int hitValue, int critValue, int currentHP, int forecastedHP)
        {
            _doubleAttackGO.SetActive(canDoubleAttack);

            _unitNameValueText.text = unit.UnitName;
            _unitImage.sprite = unit.UnitPortrait;

            var formattedWeaponName = Regex.Replace(unit.EquippedWeapon.WeaponName.ToString(), "(\\B[A-Z])", " $1");
            _equippedWeaponValueText.text = formattedWeaponName;
            _equippedWeaponImage.sprite = unit.EquippedWeapon.WeaponSprite;

            // TODO: Like health bar fill image (damage) value, set this to the current value of HP (before damage)
            _currentHPValueText.text = unit.HealthPointsBaseValue.ToString();

            _attackStatValueText.text = attackValue.ToString();

            foreach (TMP_Text hitStat in _hitStatValueTexts)
            {
                hitStat.text = hitValue.ToString() + "%";
            }

            _critStatValueText.text = critValue.ToString() + "%";

            _predictedRemainingHPValueText.text = forecastedHP.ToString();

            foreach (Image remHPImg in _predictedRemainingHPAreaImages)
            {
                if (forecastedHP <= 0)
                {
                    _predictedRemainingHPValueText.text = "X";
                    remHPImg.color = _deadUIColour;
                }
                else
                {
                    remHPImg.color = _aliveUIColour;
                }
            }

            float healthFillAmount = CalculateHealthPercentage(forecastedHP, unit.HealthPointsBaseValue);
            _damageIndicatorRectTransform.DOAnchorPosX(CalculateXPosition(forecastedHP, unit.HealthPointsBaseValue), 0.1f);

            float damageFillAmount = CalculateHealthPercentage(currentHP, unit.HealthPointsBaseValue);

            // TODO: Calculate prediction of health loss. Loss % of total health and then normalized to a floating point value between 0 and 1.
            _healthBarFill.fillAmount = healthFillAmount;

            // TODO: This is what the player had before.
            _healthBarFillDamage.fillAmount = damageFillAmount;
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }

        private float CalculateXPosition(int currentHealth, int maxHealth)
        {
            if (_side == BattleForecastSide.Right)
            {
                float healthPercentage = (float)currentHealth / maxHealth;
                float xPos = (1 - healthPercentage) * 200;

                if (xPos > 200)
                {
                    xPos = 200;
                }

                return xPos;
            }

            else
            {
                float healthPercentage = (float)currentHealth / maxHealth;
                float xPos = healthPercentage * 200;

                if (xPos < 0)
                {
                    xPos = 0;
                }

                return xPos;
            }

        }

        private void OpenUISide()
        {
            _areaRT.DOAnchorPosX(0, 0.3f).SetEase(Ease.Linear);
        }

        private void CancelUISide()
        {
            if (_side == BattleForecastSide.Left)
            {
                _areaRT.DOAnchorPosX(Constants.BATTLE_FORECAST_LEFT_X_POS_TO, 0.3f).SetEase(Ease.Linear);
            }
            else
            {
                _areaRT.DOAnchorPosX(Constants.BATTLE_FORECAST_RIGHT_X_POS_TO, 0.3f).SetEase(Ease.Linear);
            }
        }

        private void FlashingDamage()
        {
            _healthBarFillDamage.DOColor(Color.white, 0.6f).SetLoops(-1, LoopType.Yoyo);           
        }
    }
}
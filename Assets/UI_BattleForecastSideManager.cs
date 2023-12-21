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
        [SerializeField] private TMP_Text[] _hitStatValueTexts;
        [SerializeField] private TMP_Text _critStatValueText;
        [SerializeField] private TMP_Text _predictedRemainingHPValueText;
        [SerializeField] private Image[] _predictedRemainingHPAreaImages;
        [SerializeField] private Image _healthBarFill;
        [SerializeField] private Image _healthBarFillDamage;
 
        [SerializeField] private Color32 _deadUIColour;
        [SerializeField] private Color32 _aliveUIColour;       

        [SerializeField] private RectTransform _areaRT;

        public bool IsForecastToggled => _forecastToggled;

        private void Start()
        {
            FlashingDamage();
        }

        public void ToggleBattleForecastSide(UnitData unit)
        {
            _forecastToggled = !_forecastToggled;

            ToggleUISide();
            PopulateBattleForecastSide(unit);            
        }

        public void CancelBattleForecast()
        {
            _forecastToggled = false;

            CancelUISide();
        }

        private void PopulateBattleForecastSide(UnitData unit)
        {
            _unitNameValueText.text = unit.UnitName;
            _unitImage.sprite = unit.UnitPortrait;

            var formattedWeaponName = Regex.Replace(unit.EquippedWeapon.ToString(), "(\\B[A-Z])", " $1");
            _equippedWeaponValueText.text = formattedWeaponName;
            _equippedWeaponImage.sprite = unit.EquippedWeaponImage;

            // TODO: Like health bar fill image (damage) value, set this to the current value of HP (before damage)
            _currentHPValueText.text = unit.HealthPointsBaseValue.ToString();

            _attackStatValueText.text = "0";

            foreach (TMP_Text hitStat in _hitStatValueTexts)
            {
                hitStat.text = "000%";
            }

            _critStatValueText.text = "000%";

            var remainingHP = 10;
            _predictedRemainingHPValueText.text = remainingHP.ToString();

            foreach (Image remHPImg in _predictedRemainingHPAreaImages)
            {
                if (remainingHP < 0)
                {
                    _predictedRemainingHPValueText.text = "X";
                    remHPImg.color = _deadUIColour;
                }
                else
                {
                    remHPImg.color = _aliveUIColour;
                }
            }

            // TODO: Calculate prediction of health loss. Loss % of total health and then normalized to a floating point value between 0 and 1.
            _healthBarFill.fillAmount = 0.5f;

            // TODO: This is what the player had before.
            _healthBarFillDamage.fillAmount = 0.9f;
        }

        private void ToggleUISide()
        {            
            if (_forecastToggled)
            {                
                _areaRT.DOAnchorPosX(0, 0.3f).SetEase(Ease.Linear);
            }
            else
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
            _healthBarFillDamage.DOColor(Color.white, 1f).SetLoops(-1, LoopType.Yoyo);           
        }
    }
}
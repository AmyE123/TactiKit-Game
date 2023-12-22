namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    public class UnitStatsManager : MonoBehaviour
    {        
        [SerializeField] private UnitManager _unitManager;

        [SerializeField] private int _healthPoints;
        [SerializeField] private Image _healthBarFill;

        [SerializeField] private int _atk;
        [SerializeField] private bool _dblAtk;
        [SerializeField] private int _hit;
        [SerializeField] private int _crit;

        private UnitData _unitBaseData;

        public UnitData UnitBaseData => _unitBaseData;

        public int HealthPoints => _healthPoints;

        public int Atk => _atk;
        public bool DblAtk => _dblAtk;
        public int Hit => _hit;
        public int Crit => _crit;

        public void Start()
        {
            _unitBaseData = _unitManager.UnitData;
            _healthPoints = _unitBaseData.HealthPointsBaseValue;
        }

        public void Update() 
        {
            _healthBarFill.fillAmount = CalculateHealthPercentage(_healthPoints, _unitBaseData.HealthPointsBaseValue);
        }

        public int AdjustHealthPoints(int value)
        {
            // Add the value to current health points and clamp it within the valid range
            _healthPoints = Mathf.Clamp(_healthPoints + value, 0, _unitBaseData.HealthPointsBaseValue);
            CheckHealthState();

            return _healthPoints;
        }

        public UnitHealthState CheckHealthState()
        {
            if (_healthPoints <= 0)
            {
                Debug.Log("[BATTLE]: Unit death! HP at 0");
                return UnitHealthState.Dead;
            }
            else
            {
                return UnitHealthState.Alive;
            }
        }

        public void SetUnitStats(int atk, bool dblAtk, int hit, int crit)
        {
            _atk = atk;
            _dblAtk = dblAtk;
            _hit = hit;
            _crit = crit;
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }
    }
}
namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UnitStatsManager : MonoBehaviour
    {
        [SerializeField] private UnitManager _unitManager;

        [SerializeField] private int _healthPoints;
        [SerializeField] private Image _healthBarFill;

        private UnitData _unitBaseData;

        public int HealthPoints => _healthPoints;

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
            CheckHealthStatus();

            return _healthPoints;
        }

        public void CheckHealthStatus()
        {
            if (_healthPoints <= 0)
            {
                Debug.Log("[BATTLE]: Unit death! HP at 0");
            }
        }

        private float CalculateHealthPercentage(int currentHealth, int maxHealth)
        {
            return (float)currentHealth / maxHealth;
        }
    }
}
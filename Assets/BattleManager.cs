namespace CT6GAMAI
{
    using UnityEngine;

    public class BattleManager : MonoBehaviour
    {
        private GameManager _gameManager;
        private CameraManager _cameraManager;
        private UIManager _uiManager;
        private bool _isBattleActive;

        [SerializeField] private GameObject _unitModel;

        public bool IsBattleActive => _isBattleActive;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _cameraManager = _gameManager.CameraManager;
        }

        public void SwitchToBattle()
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[1]);
            _isBattleActive = true;
        }

        public void SwitchToMap()
        {
            _cameraManager.SwitchCamera(_cameraManager.Cameras[0]);
            _isBattleActive = false;
        }
    }
}
namespace CT6GAMAI
{
    using UnityEngine;
    using Cinemachine;
    using System.Linq;

    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] _cameras;                                            
        [SerializeField] private CinemachineVirtualCamera _startCamera;
        [SerializeField] private CinemachineVirtualCamera _currentCamera;
        
        public CinemachineVirtualCamera[] Cameras => _cameras;

        private void Start()
        {
            SetActiveCamera(_startCamera ?? _cameras.FirstOrDefault());
        }

        public void SwitchCamera(CinemachineVirtualCamera newCamera)
        {
            SetActiveCamera(newCamera);
        }

        private void SetActiveCamera(CinemachineVirtualCamera newCamera)
        {
            if (newCamera == null || newCamera == _currentCamera) return;

            if (_currentCamera != null)
            {
                _currentCamera.Priority = Constants.INACTIVE_CAMERA_PRIORITY;
            }

            _currentCamera = newCamera;
            _currentCamera.Priority = Constants.ACTIVE_CAMERA_PRIORITY;
        }
    }
}
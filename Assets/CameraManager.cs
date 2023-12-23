namespace CT6GAMAI
{
    using UnityEngine;
    using Cinemachine;
    using System.Linq;
    using DG.Tweening;
    using static CT6GAMAI.Constants;
    using System.Collections;

    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraStates _cameraState;

        [SerializeField] private CinemachineVirtualCamera[] _cameras;
        [SerializeField] private CinemachineVirtualCamera _currentCamera;

        [SerializeField] private CinemachineVirtualCamera _mapCamera;
        [SerializeField] private CinemachineVirtualCamera _battleCamera;

        public CinemachineVirtualCamera[] Cameras => _cameras;
        public CameraStates CameraState => _cameraState;
        

        private void Start()
        {
            SetActiveCamera(_mapCamera);
        }

        public void SwitchCamera(CinemachineVirtualCamera newCamera)
        {
            SetActiveCamera(newCamera);

            if (newCamera == _mapCamera)
            {
                _cameraState = CameraStates.Map;
            }
            if (newCamera == _battleCamera)
            {
                _cameraState = CameraStates.Battle;
            }
        }

        private void SetActiveCamera(CinemachineVirtualCamera newCamera)
        {
            if (newCamera == null || newCamera == _currentCamera) return;

            if (_currentCamera != null)
            {
                _currentCamera.Priority = INACTIVE_CAMERA_PRIORITY;
            }

            _currentCamera = newCamera;
            _currentCamera.Priority = ACTIVE_CAMERA_PRIORITY;
        }
    }
}
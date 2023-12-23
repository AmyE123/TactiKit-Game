namespace CT6GAMAI
{
    using UnityEngine;
    using Cinemachine;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manages camera switching and states for map view and battle view.
    /// </summary>
    public class CameraManager : MonoBehaviour
    {
        [Header("Camera Configuration")]
        [SerializeField] private CameraStates _cameraState;
        [SerializeField] private CinemachineVirtualCamera[] _cameras;

        [Header("Camera Settings")]
        [SerializeField] private CinemachineVirtualCamera _currentCamera;

        [Header("Camera Types")]
        [SerializeField] private CinemachineVirtualCamera _mapCamera;
        [SerializeField] private CinemachineVirtualCamera _battleCamera;

        /// <summary>
        /// Gets an array of all CinemachineVirtualCameras.
        /// </summary>
        public CinemachineVirtualCamera[] Cameras => _cameras;

        /// <summary>
        /// Gets the current state of the camera.
        /// </summary>
        public CameraStates CameraState => _cameraState;

        private void Start()
        {
            SetActiveCamera(_mapCamera);
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

        /// <summary>
        /// Switches the camera.
        /// </summary>
        /// <param name="newCamera">The new camera to be activated.</param>
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
    }
}
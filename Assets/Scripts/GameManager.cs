namespace CT6GAMAI
{
    using DG.Tweening;
    using UnityEngine;

    /// <summary>
    /// Manages the overall game state and interactions between game components.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance of GameManager.
        /// </summary>
        public static GameManager Instance;

        #region Private Manager References

        [SerializeField] private GlobalUnitsManager _unitsManager;
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private AudioManager _audioManager;
        [SerializeField] private UIManager _uiManager;
        [SerializeField] private CameraManager _cameraManager;
        [SerializeField] private BattleManager _battleManager;

        #endregion // Private Manager References

        #region Public Manager References

        /// <summary>
        /// Public getter for GlobalUnitsManager.
        /// </summary>
        public GlobalUnitsManager UnitsManager => _unitsManager;

        /// <summary>
        /// Public getter for GridManager.
        /// </summary>
        public GridManager GridManager => _gridManager;

        /// <summary>
        /// Public getter for AudioManager
        /// </summary>
        public AudioManager AudioManager => _audioManager;

        /// <summary>
        /// Public getter for UIManager
        /// </summary>
        public UIManager UIManager => _uiManager;

        /// <summary>
        /// Public getter for CameraManager
        /// </summary>
        public CameraManager CameraManager => _cameraManager;

        /// <summary>
        /// Public getter for BattleManager
        /// </summary>
        public BattleManager BattleManager => _battleManager;

        #endregion // Public Manager References

        void Awake()
        {
            Instance = this;
        }
    }
}
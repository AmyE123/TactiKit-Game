namespace CT6GAMAI
{
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

        #region Public Manager References

        /// <summary>
        /// Public getter for GlobalUnitsManager.
        /// </summary>
        public GlobalUnitsManager UnitsManager => _unitsManager;

        /// <summary>
        /// Public getter for GridManager.
        /// </summary>
        public GridManager GridManager => _gridManager;

        #endregion // Public Manager References

        #region Private Manager References

        [SerializeField] private GlobalUnitsManager _unitsManager;
        [SerializeField] private GridManager _gridManager;

        #endregion // Private Manager References

        void Awake()
        {
            Instance = this;
        }
    }
}
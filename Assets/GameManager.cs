namespace CT6GAMAI
{
    using UnityEngine;

    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance;

        #region Private Manager References

        [SerializeField] private GlobalUnitsManager _unitsManager;
        [SerializeField] private GridManager _gridManager;

        #endregion // Private Manager References

        #region Public Manager References

        public GlobalUnitsManager UnitsManager => _unitsManager;
        public GridManager GridManager => _gridManager;

        #endregion // Public Manager References

        void Awake()
        {
            Instance = this;
        }


    }
}
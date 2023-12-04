namespace CT6GAMAI
{
    /// <summary>
    /// Contains constants and enums used throughout the game.
    /// </summary>
    public static class Constants
    {
        #region Nodes
        /// <summary>
        /// Enumerations representing the different visual states of a node.
        /// </summary>
        public enum NodeVisualState { Default, HoveredBlue, HoveredRed, HoveredGreen, SelectedBlue, SelectedRed, SelectedGreen, AllEnemyRange, SingularEnemyRange, PointOfInterest }

        /// <summary>
        /// Enumerations representing the different color states of a node.
        /// </summary>
        public enum NodeVisualColorState { Blue, Red, Green }

        /// <summary>
        /// Enumerations representing the different enemy color states of a node.
        /// </summary>
        public enum NodeVisualEnemyColorState { SingularEnemy, AllEnemy }

        /// <summary>
        /// Enumerations representing the different states of a selector.
        /// </summary>
        public enum NodeSelectorState { NoSelection, DefaultSelected, PlayerSelected, EnemySelected }

        /// <summary>
        /// Enumerations representing different directions.
        /// </summary>
        public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }

        /// <summary>
        /// Enumerations representing different terrain types.
        /// </summary>
        public enum Terrain { Default, Forest, River, Fort }

        /// <summary>
        /// Constant string for the tag string used on nodes.
        /// </summary>
        public const string NODE_TAG_REFERENCE = "Node";

        /// <summary>
        /// A cost value to set on nodes which should be unpassable.
        /// Reason being is that using int.MaxValue leads to int overflow.
        /// </summary>
        public const int MAX_NODE_COST = 99999;
        #endregion //Nodes

        #region Units
        /// <summary>
        /// Enumerations representing different unit classes.
        /// </summary>
        public enum Class { Knight, Mercenary, Archer };

        /// <summary>
        /// Constant string for the tag string used on units.
        /// </summary>
        public const string UNIT_TAG_REFERENCE = "Unit";

        /// <summary>
        /// A value representing the amount of time a unit waits between moving tiles
        /// </summary>
        public const float MOVEMENT_DELAY = 0.3f;

        /// <summary>
        /// A value representing the speed a unit moves from one tile to the next
        /// </summary>
        public const float MOVEMENT_SPEED = 0.2f;

        /// <summary>
        /// A value representing the speed a unit rotates toward their look rotation
        /// </summary>
        public const float LOOK_ROTATION_SPEED = 0.1f;
        #endregion //Units                           
    }
}
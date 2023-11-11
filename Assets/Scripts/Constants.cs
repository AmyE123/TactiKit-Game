namespace CT6GAMAI
{
    public static class Constants
    {
        #region Nodes
        /// <summary>
        /// The enumerations representing the different visual states of a node
        /// </summary>
        public enum NodeVisualState { Default, HoveredBlue, HoveredRed, HoveredGreen, SelectedBlue, SelectedRed, SelectedGreen, AllEnemyRange, SingularEnemyRange, PointOfInterest }

        /// <summary>
        /// The enumerations representing the different color states of a node
        /// </summary>
        public enum NodeVisualColorState { Blue, Red, Green }

        /// <summary>
        /// The enumerations representing the different enemy color states of a node
        /// </summary>
        public enum NodeVisualEnemyColorState { SingularEnemy, AllEnemy }

        /// <summary>
        /// The enumerations representing the different states of a selector
        /// </summary>
        public enum NodeSelectorState { NoSelection, DefaultSelected, PlayerSelected, EnemySelected }

        public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }
        public enum Terrain { Default, Forest, River, Fort }

        public const string NODE_TAG_REFERENCE = "Node";
        #endregion //Nodes

        #region Units
        public enum Class { Knight, Mercenary, Archer };
        #endregion //Units                           
    }
}
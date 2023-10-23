namespace CT6GAMAI
{
    public static class Constants
    {
        #region Nodes
        public enum State { Default, HoveredBlue, HoveredRed, HoveredGreen, SelectedBlue, SelectedRed, SelectedGreen, AllEnemyRange, SingularEnemyRange, PointOfInterest }
        public enum Direction { North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest }
        public enum Terrain { Default, Forest, River, Fort }

        public const string NODE_TAG_REFERENCE = "Node";
        #endregion //Nodes

        #region Units
        public enum Class { Knight, Mercenary, Archer };
        #endregion //Units                           
    }
}
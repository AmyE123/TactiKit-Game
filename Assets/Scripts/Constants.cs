namespace CT6GAMAI
{
    public static class Constants
    {
        #region Nodes
        public enum State { Default, Selector, ConfirmedSelected, Selected, Pathway }
        public enum Terrain { Default, Forest, River, Fort }

        public const string NODE_TAG_REFERENCE = "Node";
        #endregion //Nodes

        #region Units
        public enum Class { Knight, Mercenary, Archer };
        #endregion //Units                           
    }
}
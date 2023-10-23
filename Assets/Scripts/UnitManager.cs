namespace CT6GAMAI
{
    using UnityEngine;

    public class UnitManager : MonoBehaviour
    {
        [SerializeField]
        private UnitData _unitData;

        public NodeManager StoodNode;

        public UnitData UnitData => _unitData;
    }
}
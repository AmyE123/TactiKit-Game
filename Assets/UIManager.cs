namespace CT6GAMAI
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UI_TileInfoManager _tileInfoManager;
        [SerializeField] private UI_UnitInfoManager _unitInfoManager;

        public UI_TileInfoManager TileInfoManager => _tileInfoManager;
        public UI_UnitInfoManager UnitInfoManager => _unitInfoManager;
    }
}
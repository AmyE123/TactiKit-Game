namespace CT6GAMAI
{
    using UnityEngine;

    public class UIManager : MonoBehaviour
    {
        [SerializeField] private UI_TileInfoManager _tileInfoManager;

        public UI_TileInfoManager TileInfoManager => _tileInfoManager;
    }
}
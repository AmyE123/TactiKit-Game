namespace CT6GAMAI
{
    using TMPro;
    using UnityEngine;

    public class DebugVersionDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _versionText;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            _versionText.text = "V" + Application.version.ToString();
        }
    }
}
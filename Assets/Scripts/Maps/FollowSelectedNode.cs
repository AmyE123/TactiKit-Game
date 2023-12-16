namespace CT6GAMAI
{
    using UnityEngine;

    public class FollowSelectedNode : MonoBehaviour
    {
        [SerializeField] private GridCursor _gridCursor; 

        void Update()
        {
            if (_gridCursor.SelectedNode != null)
            {
                transform.position = _gridCursor.SelectedNode.transform.position;
            }            
        }
    }
}

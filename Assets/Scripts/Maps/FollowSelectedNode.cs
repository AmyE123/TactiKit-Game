namespace CT6GAMAI
{
    using UnityEngine;

    public class FollowSelectedNode : MonoBehaviour
    {
        [SerializeField] private GridSelector _gridSelector; 

        void Update()
        {
            if (_gridSelector.SelectedNode != null)
            {
                transform.position = _gridSelector.SelectedNode.transform.position;
            }            
        }
    }
}

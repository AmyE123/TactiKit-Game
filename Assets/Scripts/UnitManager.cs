namespace CT6GAMAI
{
    using UnityEngine;

    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private UnitData _unitData;

        public NodeManager StoodNode;

        public UnitData UnitData => _unitData;

        private RaycastHit raycastHit;

        private void Start()
        {
            // TODO: Once player can move, update this every movement
            StoodNode = DetectStoodNode();
            StoodNode.StoodUnit = this;
        }

        NodeManager DetectStoodNode()
        {
            if (Physics.Raycast(transform.position, -transform.up, out raycastHit, 1))
            {
                if (raycastHit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
                {
                    var NodeManager = raycastHit.transform.parent.GetComponent<NodeManager>();
                    return NodeManager;
                }
                else
                {
                    Debug.Log("ERROR: Cast hit non-node object - " + raycastHit.transform.gameObject.name);
                    return null;
                }
            }
            else
            {
                Debug.Log("ERROR: Cast hit nothing");
                return null;
            }
        }
    }
}
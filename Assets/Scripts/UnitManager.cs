namespace CT6GAMAI
{
    using UnityEngine;
    using DG.Tweening;
    using System.Collections;

    /// <summary>
    /// Manager for the singular unit.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private UnitData _unitData;
        [SerializeField] private NodeManager _stoodNode;   
        
        private RaycastHit _stoodNodeRayHit;
        private GridSelector _gridSelector;
        private bool _isMoving = false;

        public bool IsMoving => _isMoving;
        public NodeManager StoodNode => _stoodNode;
        public UnitData UnitData => _unitData;

        private void Start()
        {
            // TODO: Once player can move, update this every movement
            _stoodNode = DetectStoodNode();
            _stoodNode.StoodUnit = this;
            _gridSelector = FindObjectOfType<GridSelector>();
        }

        NodeManager DetectStoodNode()
        {
            if (Physics.Raycast(transform.position, -transform.up, out _stoodNodeRayHit, 1))
            {
                if (_stoodNodeRayHit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
                {
                    var NodeManager = _stoodNodeRayHit.transform.parent.GetComponent<NodeManager>();
                    return NodeManager;
                }
                else
                {
                    Debug.Log("ERROR: Cast hit non-node object - " + _stoodNodeRayHit.transform.gameObject.name);
                    return null;
                }
            }
            else
            {
                Debug.Log("ERROR: Cast hit nothing");
                return null;
            }
        }

        /// <summary>
        /// Clears the stood unit value from the node.
        /// This is used when a unit moves spaces.
        /// </summary>
        public void ClearStoodUnit()
        {
            _stoodNode.StoodUnit = null;
        }

        public void MoveToNextNode(Node endPoint)
        {           

            var endPointPos = endPoint.UnitTransform.transform.position;

            var dir = (endPointPos - transform.position).normalized;
            var lookRot = Quaternion.LookRotation(dir);

            transform.DORotateQuaternion(lookRot, 0.1f);

            transform.DOMove(endPointPos, 0.2f).SetEase(Ease.InOutQuad);


        }

        public IEnumerator MoveToEndPoint()
        {
            _isMoving = true;

            for (int i = 1; i < _gridSelector.path.Count; i++)
            {
                Node n = _gridSelector.path[i];

                MoveToNextNode(n);

                yield return new WaitForSeconds(0.3f);

                if (i == _gridSelector.path.Count - 1)
                {
                    _gridSelector.OccupiedNode = _gridSelector.path[i];
                    _isMoving = false;
                    _gridSelector.UnitPressed = false;
                    _stoodNode = DetectStoodNode();
                    _stoodNode.StoodUnit = this;
                }

            }
        }
    }
}
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
            _stoodNode = DetectStoodNode();
            _stoodNode.StoodUnit = this;
            _gridSelector = FindObjectOfType<GridSelector>();
        }

        private NodeManager DetectStoodNode()
        {
            if (Physics.Raycast(transform.position, -transform.up, out _stoodNodeRayHit, 1))
            {
                return GetNodeFromRayHit(_stoodNodeRayHit);
            }
            else
            {
                Debug.Log("[ERROR]: Cast hit nothing");
                return null;
            }
        }

        private NodeManager GetNodeFromRayHit(RaycastHit hit)
        {
            if (hit.transform.gameObject.tag == Constants.NODE_TAG_REFERENCE)
            {
                return _stoodNodeRayHit.transform.parent.GetComponent<NodeManager>();
            }
            else
            {
                Debug.Log("[ERROR]: Cast hit non-node object - " + _stoodNodeRayHit.transform.gameObject.name);
                return null;
            }
        }

        /// <summary>
        /// Updates the stood unit value from the node.
        /// </summary>
        /// <param name="unit">The unit you want to update the stood node of.</param>
        public void UpdateStoodNode(UnitManager unit)
        {
            if (_stoodNode != null)
            {
                _stoodNode.StoodUnit = unit;
            }
        }

        /// <summary>
        /// Clears the stood unit value from the node.
        /// This is used when a unit moves spaces.
        /// </summary>
        public void ClearStoodUnit()
        {
            if (_stoodNode != null)
            {
                _stoodNode.StoodUnit = null;
            }
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

                _stoodNode = DetectStoodNode();

                yield return new WaitForSeconds(0.3f);

                if (i == _gridSelector.path.Count - 1)
                {
                    FinalizeMovementValues(i);
                }
            }
        }

        private void FinalizeMovementValues(int pathIndex)
        {
            _gridSelector.OccupiedNode = _gridSelector.path[pathIndex];
            _isMoving = false;
            _gridSelector.UnitPressed = false;
            _stoodNode = DetectStoodNode();
            UpdateStoodNode(this);
        }
    }
}
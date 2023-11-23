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

        public NodeManager StoodNode;

        public UnitData UnitData => _unitData;

        private RaycastHit raycastHit;

        public GridSelector _gridSelector;

        public bool IsMoving = false;

        private void Start()
        {
            // TODO: Once player can move, update this every movement
            StoodNode = DetectStoodNode();
            StoodNode.StoodUnit = this;
            _gridSelector = FindObjectOfType<GridSelector>();
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
            IsMoving = true;

            for (int i = 1; i < _gridSelector.path.Count; i++)
            {
                Node n = _gridSelector.path[i];

                MoveToNextNode(n);

                yield return new WaitForSeconds(0.3f);

                if (i == _gridSelector.path.Count - 1)
                {
                    _gridSelector.OccupiedNode = _gridSelector.path[i];
                    IsMoving = false;
                    _gridSelector.UnitPressed = false;
                    StoodNode = _gridSelector.path[i].NodeManager;
                    StoodNode.StoodUnit = this;
                }

            }
        }
    }
}
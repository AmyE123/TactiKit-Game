namespace CT6GAMAI
{
    using UnityEngine;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Manager for the singular unit.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private UnitData _unitData;
        [SerializeField] private NodeManager _stoodNode;
        [SerializeField] private Animator _animator;

        private GameManager _gameManager;
        private GridManager _gridManager;

        private RaycastHit _stoodNodeRayHit;
        private GridSelector _gridSelector;
        private bool _isMoving = false;

        public bool IsMoving => _isMoving;
        public NodeManager StoodNode => _stoodNode;
        public UnitData UnitData => _unitData;
        public Animator Animator => _animator;

        // TODO: Cleanup!
        public Material greyMat;
        public GameObject knightBaseObj;
        public List<SkinnedMeshRenderer> allSMRenderers;
        public List<MeshRenderer> allRenderers;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;

            _stoodNode = DetectStoodNode();
            _stoodNode.StoodUnit = this;

            // TODO: Cleanup of gridselector stuff
            _gridSelector = FindObjectOfType<GridSelector>();
        }

        private void Update()
        {
            // WIP code!!
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("Pressed key");
                TurnUnitInactive();
            }
        }

        // TODO: CLEANUP
        private void TurnUnitInactive()
        {
            allSMRenderers = knightBaseObj.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            allRenderers = knightBaseObj.GetComponentsInChildren<MeshRenderer>().ToList();

            foreach (Renderer renderer in allSMRenderers) 
            {
                Debug.Log("Grabbing " + renderer.name + "And changing mat");
                renderer.material = greyMat;
            }

            foreach (MeshRenderer renderer in allRenderers)
            {
                renderer.material = greyMat;
            }

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

        private void MoveToNextNode(Node endPoint)
        {
            var endPointPos = endPoint.UnitTransform.transform.position;

            var dir = (endPointPos - transform.position).normalized;
            var lookRot = Quaternion.LookRotation(dir);

            // TODO: Magic numbers can be cleaned up
            transform.DORotateQuaternion(lookRot, 0.1f);

            transform.DOMove(endPointPos, 0.2f).SetEase(Ease.InOutQuad);
        }

        private void FinalizeMovementValues(int pathIndex)
        {
            // TODO: This can be cleaned up
            //_gridSelector.OccupiedNode = _gridSelector.path[pathIndex];
            _gridManager.OccupiedNodes[0] = _gridManager.MovementPath[pathIndex].NodeManager;

            _isMoving = false;
            _gridSelector.UnitPressed = false;
            _stoodNode = DetectStoodNode();
            _gridManager.MovementPath.Clear();
            UpdateStoodNode(this);
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

   
        /// <summary>
        /// Move the unit to their selected end point
        /// </summary>
        public IEnumerator MoveToEndPoint()
        {
            _isMoving = true;

            for (int i = 1; i < _gridManager.MovementPath.Count; i++)
            {
                Node n = _gridManager.MovementPath[i];

                MoveToNextNode(n);

                _stoodNode = DetectStoodNode();

                yield return new WaitForSeconds(0.3f);

                if (i == _gridManager.MovementPath.Count - 1)
                {
                    FinalizeMovementValues(i);
                }
            }
        }

        /// <summary>
        /// Move the unit to an end point
        /// </summary>
        public IEnumerator MoveToPoint(Node targetNode)
        {
            // TODO: Make this work
            return null;
        }
    }
}
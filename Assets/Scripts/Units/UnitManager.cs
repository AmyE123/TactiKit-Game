namespace CT6GAMAI
{
    using UnityEngine;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// Manager for the singular unit.
    /// </summary>
    public class UnitManager : MonoBehaviour
    {
        [SerializeField] private MovementRange _movementRange;
        [SerializeField] private UnitAnimationManager _unitAnimationManager;

        [SerializeField] private UnitData _unitData;
        [SerializeField] private NodeManager _stoodNode;
        [SerializeField] private NodeManager _updatedStoodNode;
        [SerializeField] private Animator _animator;

        private GameManager _gameManager;
        private GridManager _gridManager;

        private RaycastHit _stoodNodeRayHit;
        private GridSelector _gridSelector;
        private bool _isMoving = false;
        private bool _isUnitInactive;
        private List<SkinnedMeshRenderer> _allSMRRenderers;
        private List<MeshRenderer> _allMRRenderers;
        private bool _isSelected = false;

        public Material greyMat;
        public Material normalMat;
        public GameObject knightBaseObj;
        public List<Renderer> AllRenderers;

        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

        public bool IsMoving => _isMoving;
        public NodeManager StoodNode => _stoodNode;
        public NodeManager UpdatedStoodNode => _updatedStoodNode;
        public UnitData UnitData => _unitData;
        public Animator Animator => _animator;
        public bool IsUnitInactive => _isUnitInactive;

        public MovementRange MovementRange => _movementRange;
        public UnitAnimationManager UnitAnimationManager => _unitAnimationManager;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _gridManager = _gameManager.GridManager;

            _stoodNode = DetectStoodNode();
            _stoodNode.StoodUnit = this;

            _updatedStoodNode = DetectStoodNode();
            _updatedStoodNode.StoodUnit = this;

            // TODO: Cleanup of gridselector stuff
            _gridSelector = FindObjectOfType<GridSelector>();
        }

        private void Update()
        {
            // For debugging purposes
            if (Input.GetKeyDown(KeyCode.I))
            {
                _isUnitInactive = !_isUnitInactive;
                SetUnitInactiveState(_isUnitInactive);
            }
        }

        private void GetAllRenderers()
        {
            _allSMRRenderers = knightBaseObj.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            _allMRRenderers = knightBaseObj.GetComponentsInChildren<MeshRenderer>().ToList();

            AllRenderers.AddRange(_allSMRRenderers.Cast<Renderer>());
            AllRenderers.AddRange(_allMRRenderers.Cast<Renderer>());
        }

        // TODO: Cleanup this messy 'Inactive' setting
        private void SetUnitInactiveState(bool isInactive)
        {
            if (AllRenderers.Count == 0)
            {
                GetAllRenderers();
            }

            Material matToSet = isInactive ? greyMat : normalMat;

            foreach (Renderer renderer in AllRenderers)
            {
                renderer.material = matToSet;
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
            if (hit.transform.gameObject.tag == NODE_TAG_REFERENCE)
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

            transform.DORotateQuaternion(lookRot, LOOK_ROTATION_SPEED);

            transform.DOMove(endPointPos, MOVEMENT_SPEED).SetEase(Ease.InOutQuad);
        }

        private void FinalizeMovementValues(int pathIndex)
        {
            // TODO: This can be cleaned up
            _gridManager.OccupiedNodes[0] = _gridManager.MovementPath[pathIndex].NodeManager;

            _isMoving = false;
            _isSelected = false;
            _gridSelector.UnitPressed = false;
            _gameManager.UnitsManager.SetActiveUnit(null);
            _stoodNode = DetectStoodNode();
            _updatedStoodNode = _stoodNode;
            _gridManager.MovementPath.Clear();            
            UpdateStoodNode(this);
        }

        /// <summary>
        /// Updates the stood unit value from the node.
        /// </summary>
        /// <param name="unit">The unit you want to update the stood node of.</param>
        public void UpdateStoodNode(UnitManager unit)
        {
            if (_updatedStoodNode != null)
            {
                _updatedStoodNode.StoodUnit = unit;
            }

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
            if (_updatedStoodNode != null)
            {
                _updatedStoodNode.StoodUnit = null;
            }

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

                _updatedStoodNode = DetectStoodNode();

                yield return new WaitForSeconds(MOVEMENT_DELAY);

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
            // TODO: Complete this functionality to make a unit move to a target node automatically
            return null;
        }
    }
}
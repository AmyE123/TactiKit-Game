namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    /// <summary>
    /// A manager for the node selector, containing controls for the NodeSelectorFSM
    /// </summary>
    public class NodeSelectorManager : MonoBehaviour
    {
        [SerializeField] private NodeSelectorFSM selectorFSM;

        [Header("State Booleans")]
        [SerializeField] private bool _isActiveSelection;
        [SerializeField] private bool _isDefaultSelected;
        [SerializeField] private bool _isPlayerSelected;
        [SerializeField] private bool _isEnemySelected;

        [Header("Selector Visual Data")]
        /// <summary>
        /// The image for the selector hand
        /// </summary>
        public Image SelectorImage;

        /// <summary>
        /// The sprite renderer for the selector
        /// </summary>
        public SpriteRenderer SelectorSR;

        /// <summary>
        /// The gameObject of the node decal (selection on the node) for the selector
        /// </summary>
        public GameObject SelectorNodeDecal;

        /// <summary>
        /// The gameObject of the canvas for the selector
        /// </summary>
        public GameObject SelectorCanvas;

        /// <summary>
        /// The different sprites for the selector
        /// </summary>
        // TODO: Update this into the data scriptable object
        public Sprite[] SelectorSprites;

        /// <summary>
        /// Visual data objects for the selector
        /// </summary>
        public NodeStateVisualData[] SelectorVisualDatas;


        #region Public Getters

        /// <summary>
        /// A bool indicating whether there is an active selection or not.
        /// This returns true if either Default Selected, Player Selected or Enemy Selected states are true.
        /// </summary>
        public bool IsActiveSelection => _isActiveSelection;

        /// <summary>
        /// A bool indicating whether the selector is in Default state.
        /// </summary>
        public bool IsDefaultSelected => _isDefaultSelected;

        /// <summary>
        /// A bool indicating whether the selector is over a player
        /// </summary>
        public bool IsPlayerSelected => _isPlayerSelected;

        /// <summary>
        /// A bool indicating whether the selector is over an enemy
        /// </summary>
        public bool IsEnemySelected => _isEnemySelected;

        #endregion // Public Getters

        #region Hidden In Inspector

        /// <summary>
        /// Reference to NodeState
        /// </summary>
        [HideInInspector] public NodeState State;

        #endregion // Hidden In Inspector

        private void Start()
        {
            selectorFSM = new NodeSelectorFSM();
            selectorFSM.Manager = this;

            State = GetComponent<NodeState>();

            RefreshSelector();
        }

        /// <summary>
        /// Refreshes the selector to make sure it is up to date with the right state.
        /// </summary>
        private void RefreshSelector()
        {
            switch (selectorFSM.GetState())
            {
                case NodeSelectorState.NoSelection:
                    SetSelectorVisuals(SelectorSprites[0], false);
                    break;
                case NodeSelectorState.DefaultSelected:
                    SetSelectorVisuals(SelectorSprites[0]);
                    break;
                case NodeSelectorState.PlayerSelected:
                    SetSelectorVisuals(SelectorSprites[0]);
                    break;
                case NodeSelectorState.EnemySelected:
                    Debug.Log("AHHH ENEMY!!");
                    SetSelectorVisuals(SelectorSprites[1], true);
                    break;
            }
        }

        /// <summary>
        /// Sets the selector visuals, the image of the selector, and its active state.
        /// </summary>
        /// <param name="selectorImage">The image that is shown where the selector is. This can be a hand, an enemy indicator, or something else.</param>
        /// <param name="isActive">Whether the selector is active or not.</param>
        private void SetSelectorVisuals(Sprite selectorImage, bool isActive = true)
        {
            SelectorImage.sprite = selectorImage;
            SelectorNodeDecal.SetActive(isActive);
            SelectorCanvas.SetActive(isActive);
        }

        /// <summary>
        /// Sets the selector to inactive.
        /// </summary>
        public void SetInactive()
        {
            _isActiveSelection = false;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = false;

            selectorFSM.ChangeState(NodeSelectorState.NoSelection);
            RefreshSelector();
        }

        /// <summary>
        /// Sets the selector to default selected.
        /// </summary>
        public void SetDefaultSelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = true;
            _isPlayerSelected = false;
            _isEnemySelected = false;

            selectorFSM.ChangeState(NodeSelectorState.DefaultSelected);
            RefreshSelector();
        }

        /// <summary>
        /// Sets the selector to player selected (when the selector is over a player)
        /// </summary>
        public void SetPlayerSelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = false;
            _isPlayerSelected = true;
            _isEnemySelected = false;

            selectorFSM.ChangeState(NodeSelectorState.PlayerSelected);
            RefreshSelector();
        }

        /// <summary>
        /// Sets the selector to enemy selected (when the selector is over an enemy)
        /// </summary>
        public void SetEnemySelected()
        {
            _isActiveSelection = true;
            _isDefaultSelected = false;
            _isPlayerSelected = false;
            _isEnemySelected = true;

            selectorFSM.ChangeState(NodeSelectorState.EnemySelected);
            RefreshSelector();
        }
    }
}

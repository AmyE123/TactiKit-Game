namespace CT6GAMAI
{
    using UnityEngine;
    using UnityEngine.UI;
    using static CT6GAMAI.Constants;

    public class NodeSelectorManager : MonoBehaviour
    {
        [SerializeField] private NodeSelectorFSM selectorFSM;

        public NodeState State;
        public NodeStateVisualData[] SelectorVisualDatas;
        public SpriteRenderer SelectorSR;

        public GameObject SelectorNodeDecal;
        public GameObject SelectorCanvas;
        public Image SelectorImage;

        public Sprite[] SelectorSprites;

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

        [SerializeField] private bool _isActiveSelection;
        [SerializeField] private bool _isDefaultSelected;
        [SerializeField] private bool _isPlayerSelected;
        [SerializeField] private bool _isEnemySelected;

        private void Start()
        {
            selectorFSM = new NodeSelectorFSM();
            selectorFSM.Manager = this;

            RefreshSelector();
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
                    SetSelectorVisuals(SelectorSprites[1]);
                    break;
                case NodeSelectorState.EnemySelected:
                    SetSelectorVisuals(SelectorSprites[2]);
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
    }
}

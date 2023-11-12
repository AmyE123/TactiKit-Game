namespace CT6GAMAI
{
    using UnityEngine;

    public class NodeState : MonoBehaviour
    {
        public NodeVisualManager VisualStateManager;
        public NodeSelectorManager SelectorStateManager;

        private void Start()
        {
            VisualStateManager = GetComponent<NodeVisualManager>();
            SelectorStateManager = GetComponent<NodeSelectorManager>();
        }

        public void ChangeVisualData(SpriteRenderer SR, NodeStateVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}
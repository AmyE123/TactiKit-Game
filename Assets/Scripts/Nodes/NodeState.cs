namespace CT6GAMAI
{
    using UnityEngine;

    public class NodeState : MonoBehaviour
    {
        public NodeVisualManager VisualStateManager;
        public NodeCursorManager CursorStateManager;

        private void Start()
        {
            VisualStateManager = GetComponent<NodeVisualManager>();
            CursorStateManager = GetComponent<NodeCursorManager>();
        }

        public void ChangeVisualData(SpriteRenderer SR, NodeVisualData VisualData)
        {
            SR.material = VisualData.Material;
            SR.color = VisualData.Color;
            SR.sprite = VisualData.Sprite;
        }
    }
}
namespace CT6GAMAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NodeVisualData", menuName = "ScriptableObjects/Nodes/NodeVisual", order = 1)]
    public class NodeData : ScriptableObject
    {
        [Header("Selector Properties")]
        public Sprite SelectorSprite;
        public Color SelectorColor;
        public Material SelectorMaterial;

        [Header("Selected Properties")]
        public Sprite SelectedSprite;
        public Color SelectedColor;
        public Material SelectedMaterial;
    }
}
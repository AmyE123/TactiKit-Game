namespace CT6GAMAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NodeVisualData", menuName = "ScriptableObjects/Nodes/NodeVisual", order = 1)]
    public class NodeData : ScriptableObject
    {
        [Header("Default Properties")]
        public Sprite DefaultSprite;
        public Color DefaultColor;
        public Material DefaultMaterial;

        [Header("Selector Properties")]
        public Sprite SelectorSprite;
        public Color SelectorColor;
        public Material SelectorMaterial;

        [Header("Confirmed Selected Properties")]
        public Sprite ConfirmedSelectedSprite;
        public Color ConfirmedSelectedColor;
        public Material ConfirmedSelectedMaterial;

        [Header("Selected Properties")]
        public Sprite SelectedSprite;
        public Color SelectedColor;
        public Material SelectedMaterial;

        [Header("Pathway Properties")]
        public Sprite PathwaySprite;
        public Color PathwayColor;
        public Material PathwayMaterial;
    }
}
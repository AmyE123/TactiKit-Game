namespace CT6GAMAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NodeVisualData", menuName = "ScriptableObjects/Nodes/NodeVisual", order = 1)]
    public class NodeVisualData : ScriptableObject
    {
        public Sprite Sprite;
        public Color Color;
        public Material Material;
    }
}
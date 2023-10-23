namespace CT6GAMAI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NodeStateVisualData", menuName = "ScriptableObjects/Nodes/NodeVisual", order = 1)]
    public class NodeStateVisualData : ScriptableObject
    {
        public Sprite Sprite;
        public Color Color;
        public Material Material;
    }
}
namespace CT6GAMAI
{
    using UnityEngine;

    public class NodeState : MonoBehaviour
    {
        public NodeData VisualData;
        public SpriteRenderer SpriteRenderer;

        bool isEnabled;


        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                isEnabled = true;
            }

            if (isEnabled)
            {
                if (Input.GetKeyDown(KeyCode.O))
                {
                    ChangeMode1();
                }
                if(Input.GetKeyDown(KeyCode.S))
                { 
                    ChangeMode2(); 
                }
            }
        }

        void ChangeMode1()
        {
            SpriteRenderer.material = VisualData.SelectedMaterial;
            SpriteRenderer.color = VisualData.SelectedColor;
            SpriteRenderer.sprite = VisualData.SelectedSprite;
        }

        void ChangeMode2()
        {
            SpriteRenderer.material = VisualData.SelectorMaterial;
            SpriteRenderer.color = VisualData.SelectorColor;
            SpriteRenderer.sprite = VisualData.SelectorSprite;
        }
    }
}
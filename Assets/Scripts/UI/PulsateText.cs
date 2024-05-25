namespace CT6GAMAI
{
    using UnityEngine;
    using TMPro;

    public class PulsateText : MonoBehaviour
    {
        public TextMeshProUGUI text; // Assign the TextMeshProUGUI component in the Inspector
        public float pulsateSpeed = 1f; // Speed of pulsation
        public float minAlpha = 0.3f; // Minimum alpha value

        private Color initialColor;

        void Start()
        {
            if (text == null)
            {
                text = GetComponent<TextMeshProUGUI>();
            }
            initialColor = text.color;
        }

        void Update()
        {
            // Calculate the pulsating alpha
            float alpha = (Mathf.Sin(Time.time * pulsateSpeed) + 1) / 2 * (1 - minAlpha) + minAlpha;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            text.color = newColor;
        }
    }
}
using DevicesSyncUnity.Messages;
using DevicesSyncUnity.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace DevicesSyncUnity.Debug
{
    public class TouchDisplay
    {
        // Constants

        protected static readonly float touchImageBaseSize = 25f; // Relative to the touch pressure
        protected static readonly float touchInfoMarginX = 5f;
        protected static readonly int touchInfoFontSize = 11;

        // Constructors

        public TouchDisplay(GameObject parent, RectTransform canvasRect, Color deviceColor)
        {
            this.canvasRect = canvasRect;
            this.deviceColor = deviceColor;

            // TODO: prefab
            GameObject = parent.AddChild("");
            rect = GameObject.AddComponent<RectTransform>();
            image = GameObject.AddComponent<Image>();

            info = GameObject.AddChild("Touch info");
            infoRect = info.AddComponent<RectTransform>();
            infoRect.anchorMin = infoRect.anchorMax = Vector2.one;
            infoRect.pivot = new Vector2(0f, 1f);
            infoRect.offsetMin = new Vector2(touchInfoMarginX, 0f);

            infoText = info.AddComponent<Text>();
            infoText.alignment = TextAnchor.UpperLeft;
            infoText.horizontalOverflow = HorizontalWrapMode.Overflow;
            infoText.verticalOverflow = VerticalWrapMode.Overflow;
            infoText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            infoText.fontSize = touchInfoFontSize;
        }

        // Properties

        public GameObject GameObject { get; protected set; }

        // Variables

        protected RectTransform canvasRect;
        protected Color deviceColor;

        protected RectTransform rect;
        protected Image image;

        protected GameObject info;
        protected RectTransform infoRect;
        protected Text infoText;

        // Methods

        public void UpdateDisplay(TouchesMessage touchesMessage, int touchIndex)
        {
            var touch = touchesMessage.touches[touchIndex];
            GameObject.name = "Touch " + touch.fingerId;

            // Configure the touch
            Vector2 touchSize = touchImageBaseSize * touch.pressure * Vector2.one;
            if (touch.radius > 0)
            {
                touchSize *= touch.radius;
            }

            Vector2 touchPosition = Vector2.Scale(touch.position, canvasRect.rect.size);
            touchPosition = new Vector2(touchPosition.x / Screen.width, touchPosition.y / Screen.height);

            // Display the touch
            rect.anchorMin = rect.anchorMax = rect.pivot = Vector2.zero;
            rect.offsetMin = touchPosition - touchSize / 2;
            rect.offsetMax = touchPosition + touchSize / 2;

            image.color = deviceColor;

            // Display the touch info
            infoText.text = "Finger Id: " + touch.fingerId + "\t\t";
            infoText.text += "Phase: " + touch.phase + "\n";
            infoText.text += "Tap Count: " + touch.tapCount + "\t";
            infoText.text += "Type: " + touch.type + "\n";
            infoText.text += "Position: " + touch.position + "\n";
        }
    }
}

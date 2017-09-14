using DeviceSyncUnity.Messages;
using DeviceSyncUnity.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DeviceSyncUnity.DebugDisplay
{
    public class MobileSyncDisplay : MonoBehaviour
    {
        // Constants

        protected readonly float goldenRatioConjugate = 0.618033988749895f; // To choose colors

        protected readonly int devicesListFontSize = 13;
        protected readonly Vector2 devicesListMargins = new Vector2(-5f, -5f);

        // Editor fields

        [SerializeField]
        protected TouchesSync touchesSync;

        [SerializeField]
        protected AccelerationSync accelerometerSync;

        [SerializeField]
        protected Canvas canvas;

        // Variables

        protected RectTransform canvasRect;

        protected float randomColorHue = 0;
        protected Dictionary<int, Color> deviceColors = new Dictionary<int, Color>();

        protected Text devicesListText;
        protected Dictionary<int, List<TouchDisplay>> touchesDisplays = new Dictionary<int, List<TouchDisplay>>();

        // Methods

        protected virtual void Awake()
        {
            canvasRect = canvas.GetComponent<RectTransform>();

            if (touchesSync != null)
            {
                touchesSync.ClientTouchesReceived += TouchesSync_TouchesReceived;
            }
            if (accelerometerSync != null)
            {
                accelerometerSync.ClientAccelerationReceived += AccelerometerSync_AccelerationReceived;
            }
        }

        protected virtual void Start()
        {
            NetworkServer.RegisterHandler(MsgType.Disconnect, ClientDisconnected);
        }

        protected virtual void TouchesSync_TouchesReceived(TouchesMessage touchesMessage)
        {
            var deviceColor = GetDeviceAssociatedColor(touchesMessage.SenderConnectionId);
            UpdateDevicesText();

            // Get or create the touch displays associated with the sender
            List<TouchDisplay> touchDisplays;
            if (!touchesDisplays.TryGetValue(touchesMessage.SenderConnectionId, out touchDisplays))
            {
                touchDisplays = new List<TouchDisplay>(touchesMessage.touchesAverage.Length);
                touchesDisplays.Add(touchesMessage.SenderConnectionId, touchDisplays);
            }

            // Get or create the touch displays parent gameobject
            GameObject touchDisplayParent;
            if (touchDisplays.Count == 0)
            {
                touchDisplayParent = canvas.gameObject.AddChild("Device " + touchesMessage.SenderConnectionId + " touches");
                touchDisplayParent.AddComponent<RectTransform>().Stretch();
            }
            else
            {
                touchDisplayParent = touchDisplays[0].GameObject.transform.parent.gameObject;
            }

            // Hide the previous touch displays
            foreach (var touchDisplay in touchDisplays)
            {
                touchDisplay.GameObject.SetActive(false);
            }

            // Display the touches
            for (int i = 0; i < touchesMessage.touchesAverage.Length; i++)
            {
                TouchDisplay touchDisplay;
                if (touchDisplays.Count <= i)
                {
                    touchDisplay = new TouchDisplay(touchDisplayParent, canvasRect, deviceColor);
                    touchDisplays.Add(touchDisplay);
                }
                else
                {
                    touchDisplay = touchDisplays[i];
                    touchDisplay.GameObject.SetActive(true);
                }
                touchDisplay.UpdateDisplay(touchesMessage, i);
            }
        }

        protected virtual void AccelerometerSync_AccelerationReceived(AccelerationMessage accelerometerMessage)
        {
            GetDeviceAssociatedColor(accelerometerMessage.SenderConnectionId);
            UpdateDevicesText();
            // TODO
        }

        protected void ClientDisconnected(NetworkMessage netMessage)
        {
            int connectionId = netMessage.conn.connectionId;
            
            // Remove from the devices list
            if (deviceColors.ContainsKey(connectionId))
            {
                deviceColors.Remove(connectionId);
                UpdateDevicesText();
            }

            // Clear the touches display
            List<TouchDisplay> touchDisplays;
            if (touchesDisplays.TryGetValue(connectionId, out touchDisplays))
            {
                foreach (var touchDisplay in touchDisplays)
                {
                    touchDisplay.GameObject.SetActive(false);
                }
            }
        }

        protected Color GetDeviceAssociatedColor(int deviceConnectionId)
        {
            Color deviceColor;
            if (!deviceColors.TryGetValue(deviceConnectionId, out deviceColor))
            {
                randomColorHue = (randomColorHue + goldenRatioConjugate) % 1;
                deviceColor = Color.HSVToRGB(randomColorHue, 0.9f, 1f);
                deviceColors[deviceConnectionId] = deviceColor;
            }
            return deviceColor;
        }

        protected void UpdateDevicesText()
        {
            // Initialize if needed
            if (devicesListText == null)
            {
                var devicesList = canvas.gameObject.AddChild("Sender Devices Info");
                var devicesListRect = devicesList.AddComponent<RectTransform>();
                devicesListRect.Stretch();
                devicesListRect.offsetMax = devicesListMargins;

                devicesListText = devicesList.AddComponent<Text>();
                devicesListText.alignment = TextAnchor.UpperRight;
                devicesListText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
                devicesListText.fontSize = devicesListFontSize;
            }

            // Display the list of sending devices
            devicesListText.text = "<b>Connected sending devices:</b>\n";
            foreach (var senderColor in deviceColors)
            {
                var hexColor = ColorUtility.ToHtmlStringRGB(senderColor.Value);
                devicesListText.text += "<color=#"+ hexColor + ">Device " + senderColor.Key + "</color>\n";
            }
        }
    }
}

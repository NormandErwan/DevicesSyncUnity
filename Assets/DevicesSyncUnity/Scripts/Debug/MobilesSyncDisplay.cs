using DevicesSyncUnity.Messages;
using DevicesSyncUnity.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DevicesSyncUnity.Debug
{
    /// <summary>
    /// Display the currents connected devices and their latest touches and acceleration information.
    /// </summary>
    public class MobilesSyncDisplay : MonoBehaviour
    {
        // Constants

        protected readonly float goldenRatioConjugate = 0.618033988749895f; // To choose colors

        protected readonly int devicesListFontSize = 13;
        protected readonly Vector2 devicesListMargins = new Vector2(-5f, -5f);

        // Editor fields

        [SerializeField]
        private DevicesInfoSync deviceInfoSync;

        [SerializeField]
        protected TouchesSync touchesSync;

        [SerializeField]
        protected AccelerationSync accelerationSync;

        [SerializeField]
        protected Canvas displayCanvas;

        // Variables

        protected RectTransform canvasRect;

        protected Text devicesListText;
        protected SortedDictionary<int, Color> deviceColors = new SortedDictionary<int, Color>();

        protected Dictionary<int, List<TouchDisplay>> touchesDisplays = new Dictionary<int, List<TouchDisplay>>();
        protected Dictionary<int, GameObject> touchesDisplaysParents = new Dictionary<int, GameObject>();

        // Methods

        protected virtual void Awake()
        {
            canvasRect = displayCanvas.GetComponent<RectTransform>();

            DevicesSync.DeviceDisconnected += DevicesSync_DeviceDisconnected;
            deviceInfoSync.DeviceInfoReceived += DeviceInfoSync_DeviceInfoReceived;

            if (touchesSync != null)
            {
                touchesSync.TouchesReceived += TouchesSync_ClientTouchesReceived;
            }
            if (accelerationSync != null)
            {
                accelerationSync.AccelerationMessageReceived += AccelereationSync_ClientAccelerationMessageReceived;
            }
        }

        protected virtual void OnDestroy()
        {
            DevicesSync.DeviceDisconnected -= DevicesSync_DeviceDisconnected;
            deviceInfoSync.DeviceInfoReceived -= DeviceInfoSync_DeviceInfoReceived;

            if (touchesSync != null)
            {
                touchesSync.TouchesReceived -= TouchesSync_ClientTouchesReceived;
            }
            if (accelerationSync != null)
            {
                accelerationSync.AccelerationMessageReceived -= AccelereationSync_ClientAccelerationMessageReceived;
            }
        }

        protected void DeviceInfoSync_DeviceInfoReceived(DeviceInfoMessage deviceInfoMessage)
        {
            float randomColorHue = (goldenRatioConjugate * deviceInfoMessage.SenderConnectionId) % 1;
            var deviceColor = Color.HSVToRGB(randomColorHue, 0.9f, 1f);
            deviceColors[deviceInfoMessage.SenderConnectionId] = deviceColor;
            UpdateDevicesText();
        }

        protected void DevicesSync_DeviceDisconnected(DeviceDisconnectedMessage deviceDisconnectedMessage)
        {
            deviceColors.Remove(deviceDisconnectedMessage.SenderConnectionId);
            UpdateDevicesText();

            GameObject touchDisplaysParent;
            if (touchesDisplaysParents.TryGetValue(deviceDisconnectedMessage.SenderConnectionId, out touchDisplaysParent))
            {
                touchDisplaysParent.SetActive(false);
            }
        }

        protected virtual void TouchesSync_ClientTouchesReceived(TouchesMessage touchesMessage)
        {
            // Get or create the touch displays associated with the sender
            List<TouchDisplay> touchDisplays;
            GameObject touchDisplaysParent;
            if (!touchesDisplays.TryGetValue(touchesMessage.SenderConnectionId, out touchDisplays))
            {
                touchDisplays = new List<TouchDisplay>(touchesMessage.touches.Length);
                touchesDisplays.Add(touchesMessage.SenderConnectionId, touchDisplays);

                touchDisplaysParent = displayCanvas.gameObject.AddChild("Device " + touchesMessage.SenderConnectionId + " touches");
                touchDisplaysParent.AddComponent<RectTransform>().Stretch();
                touchesDisplaysParents.Add(touchesMessage.SenderConnectionId, touchDisplaysParent);
            }
            else
            {
                touchDisplaysParent = touchesDisplaysParents[touchesMessage.SenderConnectionId];
            }

            // Hide the previous touch displays
            foreach (var touchDisplay in touchDisplays)
            {
                touchDisplay.GameObject.SetActive(false);
            }

            // Display the touches
            var deviceColor = deviceColors[touchesMessage.SenderConnectionId];
            for (int i = 0; i < touchesMessage.touches.Length; i++)
            {
                TouchDisplay touchDisplay;
                if (touchDisplays.Count <= i)
                {
                    touchDisplay = new TouchDisplay(touchDisplaysParent, canvasRect, deviceColor);
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

        protected virtual void AccelereationSync_ClientAccelerationMessageReceived(AccelerationMessage accelerometerMessage)
        {
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

        // TODO: prefab
        protected void UpdateDevicesText()
        {
            // Initialize if needed
            if (devicesListText == null)
            {
                var devicesList = displayCanvas.gameObject.AddChild("Sender Devices Info");
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

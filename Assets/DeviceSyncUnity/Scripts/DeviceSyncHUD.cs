using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceSyncUnity
{
    public class DeviceSyncHUD : MonoBehaviour
    {
        [Serializable]
        protected class DeviceSyncConnectionInfo
        {
            public string ServerIp { get; set; }
            public int ServerPort { get; set; }

            public DeviceSyncConnectionInfo()
            {
            }

            public DeviceSyncConnectionInfo(DeviceSyncClient deviceSyncClient)
            {
                ServerIp = deviceSyncClient.ServerIp;
                ServerPort = deviceSyncClient.ServerPort;
            }
        }

        // Constants

        protected const string connectionInfoFileName = "DeviceSyncConnectionInfo";

        // Editor fields

        [SerializeField]
        protected DeviceSyncClient deviceSyncClient;

        [SerializeField]
        protected RectTransform hudRect;

        [SerializeField]
        protected InputField serverIpInput;

        [SerializeField]
        protected InputField serverPortInput;

        [SerializeField]
        protected Toggle rememberConnectionInfo;

        [SerializeField]
        protected Button createServerButton;

        [SerializeField]
        protected Button connectClientButton;

        [SerializeField]
        protected Button cancelButton;

        // Variables

        protected string connectionInfoFilePath;
        protected bool autoStartConnection;

        // Methods

        protected void Awake()
        {
            autoStartConnection = deviceSyncClient.AutoStartConnection;
            deviceSyncClient.AutoStartConnection = false;
        }

        protected void Start()
        {
            deviceSyncClient.ConnectionStarted += HidePannel;

            connectionInfoFilePath = Application.persistentDataPath + "/" + connectionInfoFileName;
            LoadConnectionInfo();

            connectClientButton.onClick.AddListener(Connect);
            cancelButton.onClick.AddListener(Cancel);

            if (autoStartConnection)
            {
                Connect();
            }
        }

        protected void Connect()
        {
            TogglePanel(true);
        }

        protected void Cancel()
        {
            TogglePanel(false);
        }

        protected void TogglePanel(bool connecting)
        {
            serverIpInput.interactable = !connecting;
            serverPortInput.interactable = !connecting;
            connectClientButton.interactable = !connecting;
            cancelButton.interactable = connecting;
        }

        protected void LoadConnectionInfo()
        {
            DeviceSyncConnectionInfo connectionInfo = null;
            if (File.Exists(connectionInfoFilePath))
            {
                using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Open))
                {
                    connectionInfo = new BinaryFormatter().Deserialize(stream) as DeviceSyncConnectionInfo;
                }
            }

            if (connectionInfo == null)
            {
                connectionInfo = new DeviceSyncConnectionInfo(deviceSyncClient);
            }
            var hubUri = new Uri(connectionInfo.ServerIp);
            serverIpInput.text = hubUri.Host;
            serverPortInput.text = hubUri.Port.ToString();
        }

        protected void SaveConnectionInfo()
        {
            using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Create))
            {
                var connectionInfo = new DeviceSyncConnectionInfo(deviceSyncClient);
                new BinaryFormatter().Serialize(stream, connectionInfo);
            }
        }

        protected void HidePannel(object sender, EventArgs args)
        {
            hudRect.gameObject.SetActive(false);
        }
    }
}

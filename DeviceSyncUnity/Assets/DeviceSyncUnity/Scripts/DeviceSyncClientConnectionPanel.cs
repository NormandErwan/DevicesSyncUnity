using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

namespace DeviceSyncUnity
{
    public class DeviceSyncClientConnectionPanel : MonoBehaviour
    {
        [Serializable]
        protected class DeviceSyncClientConnectionInfo
        {
            public string HubConnectionUrl { get; set; }
            public string HubName { get; set; }

            public DeviceSyncClientConnectionInfo()
            {
            }

            public DeviceSyncClientConnectionInfo(DeviceSyncClient deviceSyncClient)
            {
                HubConnectionUrl = deviceSyncClient.HubConnectionUrl;
                HubName = deviceSyncClient.HubName;
            }
        }

        // Constants

        protected const string connectionInfoFileName = "DeviceSyncClientConnectionInfo";

        // Editor fields

        [SerializeField]
        protected DeviceSyncClient deviceSyncClient;

        [SerializeField]
        protected RectTransform panelRect;

        [SerializeField]
        protected InputField hubHostInput;

        [SerializeField]
        protected InputField hubPortInput;

        [SerializeField]
        protected InputField hubNameInput;

        [SerializeField]
        protected Button connectButton;

        [SerializeField]
        protected Button cancelButton;

        [SerializeField]
        protected Toggle rememberConnectionInfo;

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

            connectButton.onClick.AddListener(Connect);
            cancelButton.onClick.AddListener(Cancel);

            if (autoStartConnection)
            {
                Connect();
            }
        }

        protected void Connect()
        {
            TogglePanel(true);

            deviceSyncClient.HubConnectionUrl = "http://" + hubHostInput.text + ":" + hubPortInput.text;
            deviceSyncClient.HubName = hubNameInput.text;
            if (rememberConnectionInfo.isOn)
            {
                SaveConnectionInfo();
            }

            Logger.Log("Testing connection to the middleware.");
            if (TestConnection())
            {
                deviceSyncClient.CreateConnection();
                deviceSyncClient.ConnectionStartAsync();
            }
            else
            {
                Logger.LogError("The middleware can't be reached. Verify the host, the port and that the middleware is started.");
                TogglePanel(false);
            }
        }

        protected void Cancel()
        {
            deviceSyncClient.ConnectionCancel();
            TogglePanel(false);
        }

        protected void TogglePanel(bool connecting)
        {
            hubHostInput.interactable = !connecting;
            hubPortInput.interactable = !connecting;
            hubNameInput.interactable = !connecting;
            connectButton.interactable = !connecting;
            cancelButton.interactable = connecting;
        }

        protected void LoadConnectionInfo()
        {
            DeviceSyncClientConnectionInfo connectionInfo = null;
            if (File.Exists(connectionInfoFilePath))
            {
                using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Open))
                {
                    connectionInfo = new BinaryFormatter().Deserialize(stream) as DeviceSyncClientConnectionInfo;
                }
            }

            if (connectionInfo == null)
            {
                connectionInfo = new DeviceSyncClientConnectionInfo(deviceSyncClient);
            }
            var hubUri = new Uri(connectionInfo.HubConnectionUrl);
            hubHostInput.text = hubUri.Host;
            hubPortInput.text = hubUri.Port.ToString();
            hubNameInput.text = connectionInfo.HubName;
        }

        protected void SaveConnectionInfo()
        {
            using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Create))
            {
                var connectionInfo = new DeviceSyncClientConnectionInfo(deviceSyncClient);
                new BinaryFormatter().Serialize(stream, connectionInfo);
            }
        }

        protected bool TestConnection()
        {
            HttpWebResponse response = null;
            try
            {
                print(deviceSyncClient.HubConnectionUrl);
                var request = (HttpWebRequest)WebRequest.Create(deviceSyncClient.HubConnectionUrl + "/signalr");
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                response = (HttpWebResponse)e.Response;
            }
            return response != null && (int)response.StatusCode >= 200;
        }

        protected void HidePannel(object sender, EventArgs args)
        {
            panelRect.gameObject.SetActive(false);
        }
    }
}

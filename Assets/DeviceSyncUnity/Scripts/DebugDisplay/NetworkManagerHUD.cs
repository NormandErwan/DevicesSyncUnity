using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace DeviceSyncUnity.DebugDisplay
{
    public class NetworkManagerHUD : MonoBehaviour
    {
        [Serializable]
        protected class ConnectionInfo
        {
            public string Address { get; set; }
            public int Port { get; set; }

            public ConnectionInfo()
            {
            }

            public void SetNetworkManagerInfo()
            {
                Address = NetworkManager.singleton.networkAddress;
                Port = NetworkManager.singleton.networkPort;
            }
        }

        protected enum PanelList
        {
            Connection,
            Connecting,
            Connected
        }

        // Constants

        protected const string connectionInfoFileName = "NetworkManagerConnectionInfo";

        // Editor fields

        [SerializeField]
        protected RectTransform connectionPanel;

        [SerializeField]
        protected InputField networkAddressInput;

        [SerializeField]
        protected InputField networkPortInput;

        [SerializeField]
        protected Button startHostButton;

        [SerializeField]
        protected Button startServerButton;

        [SerializeField]
        protected Button startClientButton;

        [SerializeField]
        protected RectTransform connectingPanel;

        [SerializeField]
        protected Text connectingPanelText;

        [SerializeField]
        protected Button connectingCancelButton;

        [SerializeField]
        protected RectTransform connectedPanel;

        [SerializeField]
        protected Text connectedPanelText;

        [SerializeField]
        protected Button connectedStopButton;

        // Variables

        protected NetworkManager manager;
        protected string connectionInfoFilePath;

        // Methods

        protected virtual void Awake()
        {
            connectionInfoFilePath = Application.persistentDataPath + "/" + connectionInfoFileName;
            LoadConnectionInfo();

            startHostButton.onClick.AddListener(StartHost);
            startServerButton.onClick.AddListener(StartServer);
            startClientButton.onClick.AddListener(StartClient);
            connectingCancelButton.onClick.AddListener(CancelConnection);
            connectedStopButton.onClick.AddListener(StopConnection);
        }

        protected virtual void Start()
        {
            manager = NetworkManager.singleton;
        }

        protected virtual void StartHost()
        {
            SaveConnectionInfo();
            if (manager.StartHost() != null)
            {
                UpdatePanelText();
                ToggleShowPanel(PanelList.Connected);
            }
        }

        protected virtual void StartServer()
        {
            SaveConnectionInfo();
            if (manager.StartServer())
            {
                UpdatePanelText();
                ToggleShowPanel(PanelList.Connected);
            }
        }

        protected virtual void StartClient()
        {
            SaveConnectionInfo();
            manager.StartClient();
            StartCoroutine("CheckConnectingClient");
        }

        protected virtual IEnumerator CheckConnectingClient()
        {
            while (!ClientConnecting())
            {
                UpdatePanelText();
                ToggleShowPanel(PanelList.Connecting);
                yield return null;
            }
            while (ClientConnecting())
            {
                if (ManagerConnected())
                {
                    UpdatePanelText();
                    break;
                }
                yield return null;
            }
            ToggleShowPanel(ManagerConnected() ? PanelList.Connected : PanelList.Connection);
        }

        protected virtual void CancelConnection()
        {
            manager.StopClient();
            ToggleShowPanel(PanelList.Connection);
        }

        protected virtual void StopConnection()
        {
            if (manager.matchMaker != null)
            {
                manager.StopMatchMaker();
            }
            else
            {
                manager.StopHost();
            }
            ToggleShowPanel(PanelList.Connection);
        }

        protected virtual bool ClientConnecting()
        {
            return manager.client != null && manager.client.connection != null && manager.client.connection.connectionId != -1;
        }

        protected virtual bool ManagerConnected()
        {
            return manager.IsClientConnected() || NetworkServer.active || manager.matchMaker != null;
        }

        protected void LoadConnectionInfo()
        {
            ConnectionInfo connectionInfo = null;
            if (File.Exists(connectionInfoFilePath))
            {
                using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Open))
                {
                    connectionInfo = new BinaryFormatter().Deserialize(stream) as ConnectionInfo;
                }
            }

            if (connectionInfo != null)
            {
                networkAddressInput.text = connectionInfo.Address;
                networkPortInput.text = connectionInfo.Port.ToString();
            }
            else
            {
                networkAddressInput.text = "";
                networkPortInput.text = "";
            }
        }

        protected void SaveConnectionInfo()
        {
            manager.networkAddress = networkAddressInput.text;
            manager.networkPort = int.Parse(networkPortInput.text);

            using (Stream stream = File.Open(connectionInfoFilePath, FileMode.Create))
            {
                var connectionInfo = new ConnectionInfo();
                connectionInfo.SetNetworkManagerInfo();
                new BinaryFormatter().Serialize(stream, connectionInfo);
            }
        }

        protected void UpdatePanelText()
        {
            connectingPanelText.text = "Connecting to " + manager.networkAddress + ":" + manager.networkPort + "..";

            connectedPanelText.text = "";
            if (NetworkServer.active)
            {
                connectedPanelText.text += "Server: port=" + manager.networkPort;
                if (manager.useWebSockets)
                {
                    connectedPanelText.text += " (Using WebSockets)";
                }
                connectedPanelText.text += ". ";
            }
            if (manager.IsClientConnected())
            {
                connectedPanelText.text += "Client: address=" + manager.networkAddress + " port=" + manager.networkPort + ".";
            }
        }

        protected void ToggleShowPanel(PanelList panel)
        {
            connectionPanel.gameObject.SetActive(panel == PanelList.Connection);
            connectingPanel.gameObject.SetActive(panel == PanelList.Connecting);
            connectedPanel.gameObject.SetActive(panel == PanelList.Connected);
        }
    }
}

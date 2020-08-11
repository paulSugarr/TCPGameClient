using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using System;
using System.Threading;
using System.Text;

namespace Networking
{
    public class NetworkManager : MonoBehaviour
    {
        #region [Singletone]
        private static NetworkManager _instance;
        public static NetworkManager Instance 
        {
            get
            {
                if (_instance == null)
                {
                    var instance = FindObjectOfType<NetworkManager>();
                    if (instance != null)
                    {
                        Instance = instance;
                    }
                    else
                    {
                        Debug.LogError("No instance found");
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }
        private void Awake()
        {
            if (Instance != this)
            {
                Debug.LogError("More than one instance");
                Destroy(gameObject);
            }
        }
        #endregion
        private const string host = "127.0.0.1";
        private const int port = 8888;
        private TcpClient _client;
        private Thread clientReceiveThread;
        private int _id;
        public static int Id { get => Instance._id; set => Instance._id = value; }
        private void Start()
        {
            Connection();
        }
        private void Connection()
        {
            try
            {
                clientReceiveThread = new Thread(new ThreadStart(ListenForData));
                clientReceiveThread.IsBackground = true;
                clientReceiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.Log("On client connect exception " + e);
            }
        }

        private void ListenForData()
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(host, port);
                byte[] bytes = new byte[64];
                while (true)
                {
                    // Get a stream object for reading 				
                    using (NetworkStream stream = _client.GetStream())
                    {
                        int length;
                        // Read incomming stream into byte arrary. 					
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incommingData = new byte[length];
                            Array.Copy(bytes, 0, incommingData, 0, length);
                            // Convert byte array to string message. 						
                            CommandManager.TryExecute(bytes);
                        }
                    }
                }
            }
            catch (SocketException socketException)
            {
                Disconnect();
                Debug.LogError("Socket exception: " + socketException);
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }
        private void Disconnect()
        {
            if (_client != null)
                _client.Close();
            clientReceiveThread.Abort();
        }
    }
}

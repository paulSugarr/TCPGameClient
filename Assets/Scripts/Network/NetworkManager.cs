using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

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
        private NetworkStream _stream;
        private int _id;
        public static int Id { get => Instance._id; set => Instance._id = value; }
        private void Start()
        {
            StartCoroutine(NetworkProcess());
        }
        private IEnumerator NetworkProcess()
        {
            _client = new TcpClient();
            _client.Connect(host, port);
            _stream = _client.GetStream();
            Debug.Log("!");
            while (true)
            {

                byte[] data = new byte[64]; // буфер для получаемых данных
                int bytes = 0;
                while (_stream.DataAvailable)
                {
                    try
                    {
                        bytes = _stream.Read(data, 0, data.Length);
                    }
                    catch
                    {
                        Debug.Log("Подключение прервано!");
                        Disconnect();
                        break;
                    }
                    yield return null;
                }

                CommandManager.TryExecute(data);

                yield return null;
            }
        }
        private void Disconnect()
        {
            if (_stream != null)
                _stream.Close();//отключение потока
            if (_client != null)
                _client.Close();//отключение клиента
            //Environment.Exit(0); //завершение процесса
        }
    }
}

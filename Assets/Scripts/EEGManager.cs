using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System;

public class EEGManager : MonoBehaviour
{
    // Singleton instance
    public static EEGManager Instance { get; private set; }

    [Header("UDP Settings")]
    [SerializeField] private string ipAddress = "127.0.0.1";
    [SerializeField] private int port = 12345;

    // Focus Score (0.0 = focused, 1.0 = relaxed)
    public float focus_score { get; private set; } = 0.2f;
    private readonly object scoreLock = new object();

    private UdpClient udpClient;
    private Thread receiveThread;
    private volatile bool isReceiving = false;

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
        } else {
            Instance = this;
        }
    }

    void Start()
    {
        // Initialize UDP client
        try {
            udpClient = new UdpClient(port);
            isReceiving = true;
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            Debug.Log($"EEGManager: UDP Receiver started on {ipAddress}:{port}");
        } catch (Exception e) {
            Debug.LogError($"EEGManager: Failed to start UDP Receiver: {e.Message}");
        }
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        while (isReceiving) {
            try {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string message = Encoding.UTF8.GetString(data);
                if (float.TryParse(message, out float score)) {
                    score = Mathf.Clamp(score, 0f, 1f); // Ensure score is between 0.0 and 1.0
                    lock (scoreLock)
                    {
                        focus_score = score;
                    }
                    Debug.Log($"EEGManager: Received Unity score: {score}");
                } else {
                    Debug.LogWarning($"EEGManager: Invalid float received: {message}");
                }
            } catch (Exception e) {
                if (isReceiving) {
                    Debug.LogError($"EEGManager: UDP Receive error: {e.Message}");
                }
            }
        }
    }

    void OnDestroy()
    {
        if (Instance == this) {
            isReceiving = false;
            try {
                receiveThread?.Abort();
            } catch (Exception e) {
                Debug.LogWarning($"EEGManager: Error aborting thread: {e.Message}");
            }
            try {
                udpClient?.Close();
            } catch (Exception e) {
                Debug.LogWarning($"EEGManager: Error closing UDP client: {e.Message}");
            }
            Debug.Log("EEGManager: UDP Receiver stopped.");
            Instance = null;
        }
    }
}
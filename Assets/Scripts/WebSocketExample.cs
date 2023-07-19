using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class WebSocketExample : MonoBehaviour
{
     private ClientWebSocket webSocket;

    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }

    async void Connect() {
        webSocket = new ClientWebSocket();
        Uri serverUri = new Uri("ws://172.10.5.95:80/");

        try {
            await webSocket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Connected to server");

            // WebSocket으로부터 데이터를 수신하는 루프 시작
            ReceiveLoop();
        } catch (Exception e) {
            Debug.LogError("WebSocket connection error: " + e.Message);
        }
        return;
    }

    async void Start()
    {
        // webSocket = new ClientWebSocket();
        // Uri serverUri = new Uri("ws://172.10.5.95:80/");

        // try
        // {
        //     await webSocket.ConnectAsync(serverUri, CancellationToken.None);
        //     Debug.Log("Connected to server");

        //     // WebSocket으로부터 데이터를 수신하는 루프 시작
        //     ReceiveLoop();
        // }
        // catch (Exception e)
        // {
        //     Debug.LogError("WebSocket connection error: " + e.Message);
        // }
    }

    async void ReceiveLoop()
    {
        byte[] receiveBuffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                string message = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);
                Debug.Log("Received message: " + message);
            }
        }
    }

    void OnDestroy()
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed by client", CancellationToken.None).Wait();
        }
    }

    // WebSocket으로 데이터를 보낼 때
    async void SendWebSocketMessage(string message)
    {
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(message);
            await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
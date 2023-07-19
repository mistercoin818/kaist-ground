using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Net.WebSockets;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using TMPro;

public class Message {
    public string type;
    public string data;

    public Message(string type, string data) {
        this.type = type;
        this.data = data;
    }
}

public class Position {
    public float px;
    public float py;
    public float pz;
    public float rx;
    public float ry;
    public float rz;
    public Position(Vector3 position, Quaternion rotation) {
        this.px = position.x;
        this.py = position.y;
        this.pz = position.z;
        this.rx = rotation.eulerAngles.x;
        this.ry = rotation.eulerAngles.y;
        this.rz = rotation.eulerAngles.z;
    }
}

enum Stage { None, Connecting, Connected, InQueue, Waiting, Start }

public class SceneLoad : MonoBehaviour
{

    public ClientWebSocket webSocket;
    public Text textHP; 
    private Stage stage = Stage.None;
    public Slider progressbar;
    public TextMeshProUGUI loadtext;
    private static string loadScene;
    private static int loadType;

    public static void LoadSceneHandle(string _name, int _loadType) {
        loadScene = _name;
        loadType = _loadType;
        SceneManager.LoadScene("Loading");
    }

    IEnumerator LoadScene() {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadScene);
        operation.allowSceneActivation = false;            

        if (loadType == 0) {
            Debug.Log("새 게임");
        }
        else if (loadType == 1) {
            Debug.Log("헌 게임");
        }

        while (!operation.isDone) {
            yield return null;
            if (progressbar.value < 0.9f) {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 0.9f, Time.deltaTime);
            }
            if (operation.progress >= 0.9f) {
                progressbar.value = Mathf.MoveTowards(progressbar.value, 1f, Time.deltaTime);
            }

            if (progressbar.value >= 1f) {
                if (loadScene == "P2P") {
                    if (webSocket == null) {
                        loadtext.text = "Start Connecting to Server";
                        stage = Stage.Connecting;
                        Connect();
                    }
                    else if (webSocket.State == WebSocketState.Connecting) {
                        loadtext.text = "Connecting...";
                    }
                    else if (webSocket.State == WebSocketState.Open && stage == Stage.Connecting) {
                        stage = Stage.Connected;
                        loadtext.text = "Connected to Server";
                        stage = Stage.InQueue;
                        SendWebSocketMessage("info", "enqueue");
                    }
                    else if (stage == Stage.Waiting) {
                        loadtext.text = "Waiting for Opponent";
                    }
                    else if (stage == Stage.Start) {
                        operation.allowSceneActivation = true;
                    }
                }
                else {
                    loadtext.text = "Press SpaceBar";
                }
            }

            if  (Input.GetKeyDown(KeyCode.Space) && progressbar.value >= 1f && operation.progress >= 0.9f) {
                operation.allowSceneActivation = true;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene());
    }


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

    async void ReceiveLoop() {
        byte[] receiveBuffer = new byte[1024];

        while (webSocket.State == WebSocketState.Open) {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text) {
                string message = Encoding.UTF8.GetString(receiveBuffer, 0, result.Count);

                // JSON을 Message 객체로 변환
                Message res = JsonUtility.FromJson<Message>(message);

                // 변환된 Message 객체 사용
                if (res.type != "position") Debug.Log("Received message - Type: " + res.type + ", Data: " + res.data);
                if (res.type == "info") {
                    if (res.data == "waiting") {
                        stage = Stage.Waiting;
                    }
                    if (res.data == "start") {
                        stage = Stage.Start;
                    }
                    if (res.data == "win") {
                        Debug.Log("win");
                    }
                    if (res.data == "lose") {
                        Debug.Log("lose");
                    }
                }
                if (res.type == "position") {
                    Position pos = JsonUtility.FromJson<Position>(res.data);
                    GameObject opponent = GameObject.Find("Opponent");
                    OpponentManager opponentManager = opponent.GetComponent<OpponentManager>();
                    opponentManager.UpdatePosition(pos);
                }
                if (res.type == "myHP") {
                    int hp = int.Parse(res.data);
                    GameObject opponent = GameObject.Find("Opponent");
                    OpponentManager opponentManager = opponent.GetComponent<OpponentManager>();
                    opponentManager.UpdateMyHP(hp);
                }
                if (res.type == "opHP") {
                    int hp = int.Parse(res.data);
                    GameObject opponent = GameObject.Find("Opponent");
                    OpponentManager opponentManager = opponent.GetComponent<OpponentManager>();
                    opponentManager.UpdateOpHP(hp);
                }
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
    async public void SendWebSocketMessage(string type, string data)
    {
        var json = JsonUtility.ToJson(new Message(type, data));
        if (webSocket != null && webSocket.State == WebSocketState.Open)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(json);
            await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }

}

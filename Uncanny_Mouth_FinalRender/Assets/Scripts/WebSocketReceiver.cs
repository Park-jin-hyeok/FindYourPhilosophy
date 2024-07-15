using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;

public class WebSocketReceiver : MonoBehaviour
{
    private WebSocket ws;
    private ChatManager chatManager;
    private SpeechToText speechToText;
    public bool start = false;
    public bool end = false;
    private bool startMessageReceived = false;
    private bool endMessageReceived = false;

    void Start()
    {
        // Get a reference to ChatManager
        chatManager = GetComponent<ChatManager>();

        if (chatManager == null)
        {
            Debug.LogError("ChatManager not found on the same GameObject as WebSocketCom1.");
        }

        speechToText = GetComponent<SpeechToText>();

        if (speechToText == null)
        {
            Debug.LogError("speechToText not found on the same GameObject as WebSocketCom1.");
        }

        // Connect to WebSocket server
        ws = new WebSocket("ws://localhost:12345"); //10.210.68.51

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
            var message = JsonUtility.FromJson<Message>(e.Data);

            if (message.receiver == "com1") // 이 클라이언트를 대상으로한 메시지인지 리시버 인자를 통해서 확인 가능 
            {
                switch (message.header)
                {
                    case "reset": // 다음사용자를 위한 초기화
                        Debug.Log(message.header + " triggered");
                        ResetContent();
                        chatManager.ResetMemory();
                        break;
                    case "start":
                        Debug.Log(message.header + " triggered");
                        startMessageReceived = true;
                        chatManager.ResetMemory();
                        break;
                    case "send": //  키보드를 통한 유저 입력
                        Debug.Log(message.header + " triggered");
                        // chatManager.AskChatGPT(message.body); DO NOT TRY AND CALL COROUTINES FROM OTHER COMPONENTS!!!!!!!!!!
                        chatManager.doAskGPT = true;
                        chatManager.currentMsg = message.body;
                        break;
                    case "end": // 평가지 출력 
                        Debug.Log(message.header + " triggered");
                        // StartCoroutine(chatManager.AssessDialogueCoroutine());DO NOT TRY AND CALL COROUTINES FROM OTHER COMPONENTS!!!!!!!!!!
                        chatManager.doAssess = true; 
                        endMessageReceived = true;
                        break;
                    case "continue":
                        Debug.Log(message.header + " triggered");
                        speechToText.triggerRecording = true;
                        break; 
                    default:
                        Debug.Log("Unknown header: " + message.header); // 명령 불분명
                        break;
                }
            }
        };

        ws.Connect();

        // Identify as com1
        var identificationMessage = new { sender = "com1" };
        ws.Send(JsonConvert.SerializeObject(identificationMessage));
    }

    void Update()
    {
        if (startMessageReceived)
        {
            start = true;
            startMessageReceived = false; // Reset flag after handling
            Debug.Log("Start variable set to true");
        }

        if (endMessageReceived)
        {
            end = true;
            endMessageReceived = false; // Reset flag after handling
            Debug.Log("End variable set to true");
        }
    }

    public void SendResult(string result, bool idk) // 평가지 출력하는 클라이언트로 메시지 보내기
    {
        // send result
        SendMessageToX("com2", "assessment", result);
    }

    public void SendMessageToX(string X, string header, string body) { 
        var message = new { 
            sender = "com0", 
            receiver = X, 
            header = header, 
            body = body 
        }; 
        string jsonMessage = JsonConvert.SerializeObject(message); 
        ws.Send(jsonMessage); 
    }

    void ResetContent() // 구현 필요! 컨텐츠 초기화
    {
        // Implement the reset logic here
        Debug.Log("Content reset");
    }

    void OnDestroy()
    {
        // Close WebSocket connection
        if (ws != null)
        {
            ws.Close();
            Debug.Log("WebSocket Closed");
        }
    }

    [System.Serializable]
    public class Message // 받게되는 메시지 형식
    {
        public string sender;
        public string receiver;
        public string header;
        public string body;
    }
}

//public class WebSocketReceiver : MonoBehaviour
//{
//    private WebSocket ws;

//    public bool start = false;
//    public bool end = false;
//    private bool startMessageReceived = false;
//    private bool endMessageReceived = false;
//    private ChatManager chatManager;

//    void Start()
//    {

//        // Get a reference to ComponentB
//        chatManager = GetComponent<ChatManager>();

//        if (chatManager == null)
//        {
//            Debug.LogError("Chatmanager not found on the same GameObject as WebSocketReceiver.");
//        }

//        // WebSocket 서버에 연결
//        ws = new WebSocket("ws://localhost:12345");
//        ws.OnMessage += (sender, e) =>
//        {
//            Debug.Log("Message from server: " + e.Data);

//            if (e.Data == "Start")
//            {
//                startMessageReceived = true;
//            }
//            else if (e.Data == "End")
//            {
//                endMessageReceived = true;
//                StartCoroutine(chatManager.AssessDialogueCoroutine());
//            }
//            else if (e.Data == "EndDebug")
//            {
//                StartCoroutine(chatManager.AssessDialogueCoroutine_debug());
//            }
//        };
//        ws.Connect();
//    }

//    public void SendResult(string result, bool idk)
//    {
//        // send result
//        ws.Send(result);
//    }

//    void Update()
//    {
//        if (startMessageReceived)
//        {
//            start = true;
//            startMessageReceived = false; // Reset flag after handling
//            Debug.Log("Start variable set to true");
//        }

//        if (endMessageReceived)
//        {
//            end = true;
//            endMessageReceived = false; // Reset flag after handling
//            Debug.Log("End variable set to true");
//        }
//    }

//    void OnDestroy()
//    {
//        // WebSocket 연결 종료
//        if (ws != null)
//        {
//            ws.Close();
//            Debug.Log("WebSocket Closed");
//        }
//    }
//}

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

            if (message.receiver == "com1") // �� Ŭ���̾�Ʈ�� ��������� �޽������� ���ù� ���ڸ� ���ؼ� Ȯ�� ���� 
            {
                switch (message.header)
                {
                    case "reset": // ��������ڸ� ���� �ʱ�ȭ
                        Debug.Log(message.header + " triggered");
                        ResetContent();
                        chatManager.ResetMemory();
                        break;
                    case "start":
                        Debug.Log(message.header + " triggered");
                        startMessageReceived = true;
                        chatManager.ResetMemory();
                        break;
                    case "send": //  Ű���带 ���� ���� �Է�
                        Debug.Log(message.header + " triggered");
                        // chatManager.AskChatGPT(message.body); DO NOT TRY AND CALL COROUTINES FROM OTHER COMPONENTS!!!!!!!!!!
                        chatManager.doAskGPT = true;
                        chatManager.currentMsg = message.body;
                        break;
                    case "end": // ���� ��� 
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
                        Debug.Log("Unknown header: " + message.header); // ��� �Һи�
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

    public void SendResult(string result, bool idk) // ���� ����ϴ� Ŭ���̾�Ʈ�� �޽��� ������
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

    void ResetContent() // ���� �ʿ�! ������ �ʱ�ȭ
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
    public class Message // �ްԵǴ� �޽��� ����
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

//        // WebSocket ������ ����
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
//        // WebSocket ���� ����
//        if (ws != null)
//        {
//            ws.Close();
//            Debug.Log("WebSocket Closed");
//        }
//    }
//}

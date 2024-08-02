using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine.UI;
using TMPro;

public class WebSocketCom0 : MonoBehaviour
{
    private WebSocket ws;
    public TMP_InputField userInputField; // Assign this in the Inspector

    void Start()
    {
        ws = new WebSocket("ws://192.168.22.73:12345");

        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened");
            var identificationMessage = JsonConvert.SerializeObject(new { sender = "com0" });
            ws.Send(identificationMessage);
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
        };

        ws.Connect();
    }

    public void SendMessageToServer(string header, string body)
    {
        var message = JsonConvert.SerializeObject(new { sender = "com0", receiver = "com1", header, body });
        ws.Send(message);
    }

    public void SendResetMessage()
    {
        SendMessageToServer("reset", "");
    }
    public void SendMicMessage()
    {
        SendMessageToServer("mic_toggle", "");
    }

    public void SendStartMessage()
    {
        SendMessageToServer("start", "");
    }

    public void SendUserMessage()
    {
        string userMessage = userInputField.text;
        if (!string.IsNullOrEmpty(userMessage))
        {
            SendMessageToServer("send", userMessage);
            userInputField.text = ""; // Clear input field after sending
        }
    }

    public void SendEndMessage()
    {
        SendMessageToServer("end", "");
    }

    void OnDestroy()
    {
        // WebSocket 연결 종료
        if (ws != null)
        {
            ws.Close();
            Debug.Log("WebSocket Closed");
        }
    }
}


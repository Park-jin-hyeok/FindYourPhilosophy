using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class EndWebSocket : MonoBehaviour
{
    private WebSocket ws;
    public Button EndButton;

    void Start()
    {
        // WebSocket ������ ����
        ws = new WebSocket("ws://localhost:12345");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
        };
        ws.Connect();

        // ��ư Ŭ�� �̺�Ʈ ���
        EndButton.onClick.AddListener(() => SendMessageToServer("End"));
    }
    void Update()
    {
        // Ű���� �Է� ����
        //       if (Input.anyKeyDown)
        //       {
        //           foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
        //           {
        //               if (Input.GetKeyDown(kcode))
        //               {
        //                   string keyPressed = kcode.ToString();
        //                   Debug.Log("Key Pressed: " + keyPressed);
        //                   SendMessageToServer(keyPressed);
        //                   break;
        //               }
        //           }
        //       }
    }
    void SendMessageToServer(string message)
    {
        if (ws.IsAlive)
        {
            ws.Send(message);
        }
    }

    void OnDestroy()
    {
        // WebSocket ���� ����
        if (ws != null)
        {
            ws.Close();
            Debug.Log("Server Closed");
        }
    }
}

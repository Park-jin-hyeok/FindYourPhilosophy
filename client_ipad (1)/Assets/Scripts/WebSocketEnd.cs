using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;
    public Button StartButton;

    void Start()
    {
        // WebSocket 서버에 연결
        ws = new WebSocket("ws://192.168.22.73:12345");
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
        };
        ws.Connect();

        // 버튼 클릭 이벤트 등록
        StartButton.onClick.AddListener(() => SendMessageToServer("Start"));
    }

    void Update()
    {
        // 키보드 입력 감지
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
        // WebSocket 연결 종료
        if (ws != null)
        {
            ws.Close();
            Debug.Log("Server Closed");
        }
    }
}

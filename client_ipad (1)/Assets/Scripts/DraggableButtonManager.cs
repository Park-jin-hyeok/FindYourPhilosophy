using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WebSocketSharp;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UIElements;

public class DraggableButtonManager : MonoBehaviour
{
    private WebSocket ws;
    public static DraggableButtonManager Instance;
    public TMP_InputField userInputField; // Assign this in the Inspector

    public DraggableButton button1;
    public DraggableButton button2;
    public DraggableButton button3;
    public DraggableButton button4;
    public bool correct = false;

    public UnityEngine.UI.Button endButton; // EndButton 참조 (Inspector에서 할당)
    public UnityEngine.UI.Button continueButton; // ContinueButton 참조 (Inspector에서 할당)
    public UnityEngine.UI.Button stopButton; // StopButton 참조 (Inspector에서 할당)

    public RawImage overlayImage; // 투명도를 변경할 RawImage
    public SceneChanger sceneChanger; // SceneChanger 참조

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // WebSocket 서버에 연결
        ws = new WebSocket("ws://192.168.246.73:12345");
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



    public void CheckAllButtonsSnapped()
    {
        if (button1.IsSnapped() && button2.IsSnapped() && button3.IsSnapped() && button4.IsSnapped())
        {
            AllButtonsSnapped();
        }
    }

    private void AllButtonsSnapped()
    {
        SendMessageToServer("start", "");

        StartCoroutine(FadeOverlayImageAndChangeScene());
        // 여기에서 필요한 로직을 추가할 수 있습니다.
    }

    private IEnumerator FadeOverlayImageAndChangeScene()
    {
        float duration = 2f; // 투명도가 변하는 시간
        float elapsed = 0f;
        Color color = overlayImage.color;
        float initialAlpha = color.a;
        float targetAlpha = 1f; // 최종 투명도 (0: 완전히 투명, 1: 불투명)

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
            color.a = alpha;
            overlayImage.color = color;
            yield return null;
        }

        // 최종 투명도 설정
        color.a = targetAlpha;
        overlayImage.color = color;

        // 이미지가 다 보이면 씬 전환 시작
        StartCoroutine(sceneChanger.FadeImageAndChangeScene());
    }

    public void SendMessageToServer(string header, string body)
    {
        var message = JsonConvert.SerializeObject(new { sender = "com0", receiver = "com1", header, body });
        ws.Send(message);
    }

    public void SendEndMessage()
    {
        SendMessageToServer("end", "");
    }

    public void SendStopMessage()
    {
        SendMessageToServer("continue", "");

        // StopButton 비활성화
        stopButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
    }

    public void SendMicMessage()
    {
        SendMessageToServer("continue", "");

        // EndButton과 ContinueButton 비활성화
        stopButton.gameObject.SetActive(true);
        endButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
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

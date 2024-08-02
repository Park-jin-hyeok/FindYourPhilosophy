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

    public UnityEngine.UI.Button endButton; // EndButton ���� (Inspector���� �Ҵ�)
    public UnityEngine.UI.Button continueButton; // ContinueButton ���� (Inspector���� �Ҵ�)
    public UnityEngine.UI.Button stopButton; // StopButton ���� (Inspector���� �Ҵ�)

    public RawImage overlayImage; // ������ ������ RawImage
    public SceneChanger sceneChanger; // SceneChanger ����

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // WebSocket ������ ����
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
        // ���⿡�� �ʿ��� ������ �߰��� �� �ֽ��ϴ�.
    }

    private IEnumerator FadeOverlayImageAndChangeScene()
    {
        float duration = 2f; // ������ ���ϴ� �ð�
        float elapsed = 0f;
        Color color = overlayImage.color;
        float initialAlpha = color.a;
        float targetAlpha = 1f; // ���� ���� (0: ������ ����, 1: ������)

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / duration);
            color.a = alpha;
            overlayImage.color = color;
            yield return null;
        }

        // ���� ���� ����
        color.a = targetAlpha;
        overlayImage.color = color;

        // �̹����� �� ���̸� �� ��ȯ ����
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

        // StopButton ��Ȱ��ȭ
        stopButton.gameObject.SetActive(false);
        endButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
    }

    public void SendMicMessage()
    {
        SendMessageToServer("continue", "");

        // EndButton�� ContinueButton ��Ȱ��ȭ
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
        // WebSocket ���� ����
        if (ws != null)
        {
            ws.Close();
            Debug.Log("Server Closed");
        }
    }
}

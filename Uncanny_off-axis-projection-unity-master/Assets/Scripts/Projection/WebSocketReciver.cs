//using System;
//using UnityEngine;
//using WebSocketSharp;
//public class WebSocketReceiver : MonoBehaviour
//{
//    private WebSocketSharp.WebSocket ws;
//    private Vector3 targetPosition;
//    private bool isMessageReceived = false;

//    void Start()
//    {
//        ws = new WebSocketSharp.WebSocket("ws://localhost:12345");
//        ws.OnOpen += (sender, e) =>
//        {
//            Debug.Log("WebSocket connected!");
//        };
//        ws.OnError += (sender, e) =>
//        {
//            Debug.LogError($"WebSocket error: {e.Message}");
//        };
//        ws.OnClose += (sender, e) =>
//        {
//            Debug.Log("WebSocket connection closed.");
//        };
//        ws.OnMessage += (sender, e) =>
//        {
//            Debug.Log("Text message received from server: " + e.Data);
//            ProcessMessage(e.Data);
//        };
//        ws.Connect();
//    }

//    void Update()
//    {
//        if (isMessageReceived)
//        {
//            // Smoothly move the eye towards the target position
//            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5);
//            isMessageReceived = false;
//        }
//    }

//    private void ProcessMessage(string message)
//    {
//        try
//        {
//            string[] values = message.Split(',');
//            if (values.Length == 3)
//            {
//                float x = float.Parse(values[0]);
//                float y = float.Parse(values[1]);
//                float w = float.Parse(values[2]);
//                targetPosition = new Vector3(x, y, w); // or adjust as needed for your setup
//                Debug.Log($"Parsed face position: x={x}, y={y}, w={w}");
//                isMessageReceived = true;
//            }
//            else
//            {
//                Debug.LogError("Received message does not contain 3 values.");
//            }
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Error parsing face position: " + ex.Message);
//        }
//    }

//    void OnDestroy()
//    {
//        if (ws != null)
//        {
//            ws.Close();
//        }
//    }
//}

using Apt.Unity.Projection;
using System;
using System.Text;
using UnityEngine;
using WebSocketSharp;


public class WebSocketReceiver : MonoBehaviour
{
    private WebSocketSharp.WebSocket ws;
    private Vector3 targetPosition;
    private bool isMessageReceived = false;

    public TrackerBase Tracker;
    public GameObject target;

    private KalmanFilter kfX;
    private KalmanFilter kfY;
    private KalmanFilter kfZ;

    private float timeSinceLastMessage = 0f;
    private const float maxTimeWithoutSignal = 0.5f; // Adjust this value as needed

    void Start()
    {
        ws = new WebSocketSharp.WebSocket("ws://localhost:12345");
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connected!");
            ws.Send("Unity connected");
        };
        ws.OnError += (sender, e) =>
        {
            Debug.LogError($"WebSocket error: {e.Message}");
        };
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };
        ws.OnMessage += (sender, e) =>
        {
            if (e.IsText)
            {
                // Do something with e.Data.
                ProcessMessage(e.Data);

                return;
            }

            if (e.IsBinary)
            {
                Debug.Log("Binary message received from server");
                string decodedMessage = Encoding.UTF8.GetString(e.RawData);
                ProcessMessage(decodedMessage);

                return;
            }
        };
        ws.Connect();

        // Initialize Kalman filters
        kfX = new KalmanFilter(0.125f, 32f, 1023f, 0f);
        kfY = new KalmanFilter(0.125f, 32f, 1023f, 0f);
        kfZ = new KalmanFilter(0.125f, 32f, 1023f, 0f);
    }

    void Update()
    {
        timeSinceLastMessage += Time.deltaTime;

        if (isMessageReceived)
        {
            // Apply Kalman filter to each coordinate
            float smoothedX = kfX.Update(targetPosition.x);
            float smoothedY = kfY.Update(targetPosition.y);
            float smoothedZ = kfZ.Update(targetPosition.z);

            Tracker.Translation = new Vector3(smoothedX, smoothedY, smoothedZ);
            BinocularCameraManager.Instance.target_position_real.x = smoothedX;
            BinocularCameraManager.Instance.target_position_real.y = smoothedY;
            BinocularCameraManager.Instance.target_position_real.z = -smoothedZ;

            isMessageReceived = false;
            timeSinceLastMessage = 0f;
        }
        else if (timeSinceLastMessage < maxTimeWithoutSignal)
        {
            // Extrapolate using the Kalman filter's prediction if no new signal is received
            float predictedX = kfX.Predict();
            float predictedY = kfY.Predict();
            float predictedZ = kfZ.Predict();

            Tracker.Translation = new Vector3(predictedX, predictedY, predictedZ);
            BinocularCameraManager.Instance.target_position_real.x = predictedX;
            BinocularCameraManager.Instance.target_position_real.y = predictedY;
            BinocularCameraManager.Instance.target_position_real.z = -predictedZ;
        }
    }

    private void ProcessMessage(string message)
    {
        try
        {
            Debug.Log(message);
            string[] values = message.Split(',');
            if (values.Length == 3)
            {
                float x = -float.Parse(values[0]);
                float y = -float.Parse(values[1]);
                float w = float.Parse(values[2]);
                targetPosition = new Vector3(x, y, w); // or adjust as needed for your setup
                Debug.Log($"Parsed face position: x={x}, y={y}, w={w}");
                isMessageReceived = true;
            }
            else
            {
                Debug.LogError("Received message does not contain 3 values.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing face position: " + ex.Message);
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
        }
    }
}

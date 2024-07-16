using Apt.Unity.Projection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMovement : MonoBehaviour
{
    public GameObject Tracker;
    public bool toggleInterpolation;

    private ProjectionPlaneCamera projectionCamera;
    private Vector3 initialLocalPosition;

    private Vector3 beforeLaction;

    public int interpolationFramesCount = 20; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    void Start()
    {
        Debug.Log("init");
        projectionCamera = GetComponent<ProjectionPlaneCamera>();

        // Save the initial local position

        beforeLaction = projectionCamera.transform.position;
        initialLocalPosition = projectionCamera.transform.localPosition;

        // Start the logging coroutine
        StartCoroutine(LogPositionEverySecond());
    }

    void Update()
    {
        if (Tracker == null)
            return;
        if (toggleInterpolation)
        {
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;

            projectionCamera.transform.position = Vector3.Lerp(beforeLaction, Tracker.transform.position, interpolationRatio);
            beforeLaction = Tracker.transform.position;

            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
        }
        else
        {
            projectionCamera.transform.position = Tracker.transform.position;
        }
    }

    IEnumerator LogPositionEverySecond()
    {
        while (true)
        {
            // Debug.Log(Tracker.transform.position);
            yield return new WaitForSeconds(1);  // Wait for 1 second before logging again
        }
    }
}

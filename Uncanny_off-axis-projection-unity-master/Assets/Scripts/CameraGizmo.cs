using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;  // Color of the gizmo
    public float gizmoRadius = 0.5f;         // Radius of the gizmo
    public float lineLength = 1.0f;

    void OnDrawGizmos()
    {
        // Set the gizmo color
        Gizmos.color = gizmoColor;

        // Draw a wireframe sphere at the transform's position
        Gizmos.DrawWireSphere(transform.position, gizmoRadius);

        // Get the start and end points for the line
        Vector3 startPoint = transform.position - transform.forward * lineLength;
        Vector3 endPoint = transform.position + transform.forward * lineLength;

        // Draw a line from the start point to the end point along the Y-axis
        Gizmos.DrawLine(startPoint, endPoint);
    }

    void Update()
    {
        // Lock the Y and Z positions to zero
        transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);

        // Lock the rotation to zero
        transform.localRotation = Quaternion.identity;
    }
}
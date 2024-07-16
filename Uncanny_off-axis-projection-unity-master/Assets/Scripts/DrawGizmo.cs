using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
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
        Vector3 startPoint = transform.position - transform.right * lineLength;
        Vector3 endPoint = transform.position + transform.right * lineLength;

        // Draw a line from the start point to the end point along the Y-axis
        Gizmos.DrawLine(startPoint, endPoint);
    }
}


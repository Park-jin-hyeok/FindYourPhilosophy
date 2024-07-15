using System;
using UnityEngine;

public class Back_Rotate : MonoBehaviour
{
    // 회전 속도를 정의하는 변수 (초당 회전 각도)
    public float secondsToDegrees = 600f / 60f; // 초당 10도 회전
    public Transform[] clockwiseObjects;
    public Transform[] counterClockwiseObjects;
    public bool spin;

    void Update()
    {
        TimeSpan timespan = DateTime.Now.TimeOfDay;
        float angle = (float)timespan.TotalSeconds * secondsToDegrees;


        // 시계 방향 회전
        foreach (Transform obj in clockwiseObjects)
        {
            if (obj.name == "Plane291")
            {
                obj.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
            else if (obj.name == "Tube117")
            {
                obj.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }
            else if (obj.name == "Tube118")
            {
                obj.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }
            else
            {
                if (obj != null)
                {
                    obj.localRotation = Quaternion.Euler(0f, 0f, angle);
                }
            }

        }
        // 반시계 방향 회전
        foreach (Transform obj in counterClockwiseObjects)
        {
            if (obj.name == "Plane291")
            {
                obj.localRotation = Quaternion.Euler(0f, angle, 0f);
            }
            else if (obj.name == "Tube117")
            {
                obj.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }
            else if (obj.name == "Tube118")
            {
                obj.localRotation = Quaternion.Euler(0f, -angle, 0f);
            }
            else
            {
                if (obj != null)
                {
                    obj.localRotation = Quaternion.Euler(0f, 0f, -angle);
                }
            }
        }

    }
}

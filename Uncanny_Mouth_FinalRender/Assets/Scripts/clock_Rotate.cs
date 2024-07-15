using System;
using UnityEngine;

public class clock_Rotate : MonoBehaviour
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
            if (obj != null)
            {
                obj.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
        // 반시계 방향 회전
        foreach (Transform obj in counterClockwiseObjects)
        {
            if (obj != null)
            {
                obj.localRotation = Quaternion.Euler(0f, 0f, -angle); // 반시계 방향이므로 각도에 마이너스를 붙임
            }
        }

    }
}

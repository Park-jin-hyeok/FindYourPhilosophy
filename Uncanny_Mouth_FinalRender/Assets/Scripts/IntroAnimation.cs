using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroAnimation : MonoBehaviour
{
    private const float
        minutesToDegrees = 360f / 60f,
        secondsToDegrees = 360f / 60f;

    public Transform minutes, seconds;

    private float duringTime = 0;
    private const float initialSpeed = 1f;
    private const float increasedSpeed = 100f;

    private float currentSpeed = initialSpeed;

    void Update()
    {
        duringTime += Time.deltaTime;

        if (duringTime < 3)
        {
            // 3�� �������� �ʱ� �ӵ��� �̵�
            TimeSpan timespan = DateTime.Now.TimeOfDay;
            minutes.localRotation =
                Quaternion.Euler(0f, 0f, (float)timespan.TotalMinutes * minutesToDegrees);
            seconds.localRotation =
                Quaternion.Euler(0f, 0f, (float)timespan.TotalSeconds * secondsToDegrees);
        }
        else
        {
            // 3�� ���Ŀ��� �ӵ� ����
            currentSpeed = increasedSpeed;
            float newMinutesRotation = minutes.localRotation.eulerAngles.z + currentSpeed * Time.deltaTime / 1;
            float newSecondsRotation = seconds.localRotation.eulerAngles.z + currentSpeed * Time.deltaTime * 12;

            // �� ������ �����մϴ�.
            minutes.localRotation = Quaternion.Euler(0f, 0f, newMinutesRotation);
            seconds.localRotation = Quaternion.Euler(0f, 0f, newSecondsRotation);
        }
    }
}

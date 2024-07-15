using System;
using UnityEngine;

public class Back_Rotate : MonoBehaviour
{
    // ȸ�� �ӵ��� �����ϴ� ���� (�ʴ� ȸ�� ����)
    public float secondsToDegrees = 600f / 60f; // �ʴ� 10�� ȸ��
    public Transform[] clockwiseObjects;
    public Transform[] counterClockwiseObjects;
    public bool spin;

    void Update()
    {
        TimeSpan timespan = DateTime.Now.TimeOfDay;
        float angle = (float)timespan.TotalSeconds * secondsToDegrees;


        // �ð� ���� ȸ��
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
        // �ݽð� ���� ȸ��
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

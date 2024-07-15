using System;
using UnityEngine;

public class clock_Rotate : MonoBehaviour
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
            if (obj != null)
            {
                obj.localRotation = Quaternion.Euler(0f, 0f, angle);
            }
        }
        // �ݽð� ���� ȸ��
        foreach (Transform obj in counterClockwiseObjects)
        {
            if (obj != null)
            {
                obj.localRotation = Quaternion.Euler(0f, 0f, -angle); // �ݽð� �����̹Ƿ� ������ ���̳ʽ��� ����
            }
        }

    }
}

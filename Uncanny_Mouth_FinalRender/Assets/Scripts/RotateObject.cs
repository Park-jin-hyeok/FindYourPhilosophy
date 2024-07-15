using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // ȸ�� �ӵ��� �����ϴ� ���� (�ʴ� ȸ�� ����)
    public float rotationSpeed = 100f;
    public float rotationSpeed1 = 20f;
    public float rotationSpeed2 = 30f;

    void Update()
    {   //�ð����
        if (gameObject.name == "1")
        {
            transform.Rotate(Vector3.down, rotationSpeed1 * Time.deltaTime);
        }
        else if (gameObject.name == "2")
        {
            transform.Rotate(Vector3.down, rotationSpeed1 * Time.deltaTime);
        }
        //����� forward Ȥ�� back
        //z�� ���� �ݽð����
        else if (gameObject.name == "1-1")
        {
            transform.Rotate(Vector3.back, rotationSpeed1 * Time.deltaTime);
        }
        else if (gameObject.name == "2-1")
        {
            transform.Rotate(Vector3.back, rotationSpeed1 * Time.deltaTime);
        }
        else if (gameObject.name == "3-1")
        {
            transform.Rotate(Vector3.back, rotationSpeed1 * Time.deltaTime);
        }
        //z���� �������� �ð� ����
        else if (gameObject.name == "4")
        {
            transform.Rotate(Vector3.forward, rotationSpeed1 * Time.deltaTime);
        }
        else if (gameObject.name == "5")
        {
            transform.Rotate(Vector3.back, rotationSpeed1 * Time.deltaTime);
        }

        else if (gameObject.name == "3-2")
        {
            transform.Rotate(Vector3.forward, rotationSpeed1 * Time.deltaTime);
        }

    }
}

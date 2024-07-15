using UnityEngine;

public class RotateObject : MonoBehaviour
{
    // 회전 속도를 정의하는 변수 (초당 회전 각도)
    public float rotationSpeed = 100f;
    public float rotationSpeed1 = 20f;
    public float rotationSpeed2 = 30f;

    void Update()
    {   //시계방향
        if (gameObject.name == "1")
        {
            transform.Rotate(Vector3.down, rotationSpeed1 * Time.deltaTime);
        }
        else if (gameObject.name == "2")
        {
            transform.Rotate(Vector3.down, rotationSpeed1 * Time.deltaTime);
        }
        //얘부터 forward 혹은 back
        //z축 기준 반시계방향
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
        //z축을 기준으로 시계 방향
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

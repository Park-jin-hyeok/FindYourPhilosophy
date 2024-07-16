using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    // 카메라 참조 변수
    private Camera mainCamera;

    // 초기화
    void Start()
    {
        // 메인 카메라 찾기
        mainCamera = Camera.main;
    }

    // 매 프레임마다 호출
    void Update()
    {
        if (mainCamera != null)
        {
            // 카메라를 바라보도록 회전
            transform.LookAt(mainCamera.transform);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Apt.Unity.Projection
{
    [RequireComponent(typeof(ProjectionPlaneCamera))]
    public class BasicMovement : MonoBehaviour
    {
        public TrackerBase Tracker;
        public float LerpSpeed = 5f; // 보간 속도 설정

        private ProjectionPlaneCamera projectionCamera;
        private Vector3 initialLocalPosition;

        void Start()
        {
            projectionCamera = GetComponent<ProjectionPlaneCamera>();
            initialLocalPosition = projectionCamera.transform.localPosition;
        }

        void Update()
        {
            if (Tracker == null)
                return;

            if (Tracker.IsTracking)
            {
                // 선형 보간을 사용하여 카메라 위치 업데이트
                Vector3 targetPosition = initialLocalPosition + Tracker.Translation;
                projectionCamera.transform.localPosition = Vector3.Lerp(projectionCamera.transform.localPosition, targetPosition, Time.deltaTime * LerpSpeed);
            }
        }
    }
}

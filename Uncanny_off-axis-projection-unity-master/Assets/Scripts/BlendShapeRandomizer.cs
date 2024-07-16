using System.Collections;
using UnityEngine;

public class BlendShapeRandomizer : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // BlendShape이 적용된 모델
    public int blendShapeIndex = 0; // BlendShape의 인덱스

    private void Start()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer가 설정되지 않았습니다.");
            return;
        }

        StartCoroutine(RandomizeBlendShape());
    }

    private IEnumerator RandomizeBlendShape()
    {
        while (true)
        {
            // 랜덤 시간 간격 설정 (3 ~ 7초 사이)
            float randomInterval = Random.Range(0.5f, 1f);

            // BlendShape 값을 0에서 100으로 증가
            yield return StartCoroutine(ChangeBlendShapeValue(0, 100, randomInterval / 2));

            // 다시 랜덤 시간 간격 설정 (3 ~ 7초 사이)
            randomInterval = Random.Range(0.5f, 1f);

            // BlendShape 값을 100에서 0으로 감소
            yield return StartCoroutine(ChangeBlendShapeValue(100, 0, randomInterval / 2));

            float waitTime = Random.Range(2f, 5f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator ChangeBlendShapeValue(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsed / duration);
            skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, currentValue);
            yield return null;
        }
        skinnedMeshRenderer.SetBlendShapeWeight(blendShapeIndex, endValue);
    }
}

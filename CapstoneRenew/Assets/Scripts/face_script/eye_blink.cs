using System.Collections;
using UnityEngine;

public class eye_blink : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer; // BlendShape�� ����� ��
    public float speed = 10;
    private void Start()
    {
        if (skinnedMeshRenderer == null)
        {
            Debug.LogError("SkinnedMeshRenderer�� �������� �ʾҽ��ϴ�.");
            return;
        }

        StartCoroutine(RandomizeBlendShape());
    }

    private IEnumerator RandomizeBlendShape()
    {
        while (true)
        {
            // BlendShape ���� 0���� 100���� ����
            float randomInterval = Random.Range(0.5f, 1.3f);
            yield return StartCoroutine(ChangeBlendShapeValue(0, 100, randomInterval / speed));

            // BlendShape ���� 100���� 0���� ����
            randomInterval = Random.Range(0.5f, 1.3f);
            yield return StartCoroutine(ChangeBlendShapeValue(100, 0, randomInterval / speed));

            // ������ �� ��� �ð� �߰� (2 ~ 5�� ����)
            float waitTime = Random.Range(1f, 3f);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator ChangeBlendShapeValue(float startValue, float endValue, float duration)
    {
        float elapsed = 0f;
        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsed / duration);
            for (int i = 0; i < blendShapeCount; i++)
            {
                skinnedMeshRenderer.SetBlendShapeWeight(i, currentValue);
            }
            yield return null;
        }
        for (int i = 0; i < blendShapeCount; i++)
        {
            skinnedMeshRenderer.SetBlendShapeWeight(i, endValue);
        }
    }
}

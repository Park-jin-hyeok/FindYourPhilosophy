using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    public GameObject Client;
    public int sceneChangeNum = 0;
    public RawImage imageToFade; // Assign this in the inspector
    public float fadeDuration = 1.0f; // Duration of the fade

    private DraggableButtonManager buttonManager; // DraggableButtonManager ����

    private void Start()
    {
        buttonManager = DraggableButtonManager.Instance;
        StartCoroutine(FadeImageFromFullToZero());
    }

    void Update()
    {
        if (buttonManager != null && buttonManager.correct)
        {
            buttonManager.correct = false; // �� ��ȯ�� �� ���� ����ǵ��� ����
        }
    }

    IEnumerator FadeImageFromFullToZero()
    {
        float elapsedTime = 0.0f;
        Color startColor = imageToFade.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - elapsedTime / fadeDuration);
            imageToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }

    public IEnumerator FadeImageAndChangeScene() // public���� ����
    {
        float elapsedTime = 0.0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            Color newColor = new Color(0, 0, 0, alpha);
            imageToFade.color = newColor;
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
}


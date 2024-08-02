using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SecondSceneManager : MonoBehaviour
{
    public RawImage imageToFade; // Assign this in the inspector
    public float fadeDuration = 1.0f; // Duration of the fade
    public Button endButton; // Assign this in the inspector

    private void Start()
    {
        StartCoroutine(FadeImageFromZeroToFull());
        endButton.onClick.AddListener(OnEndButtonClick);
    }

    private void OnEndButtonClick()
    {
        StartCoroutine(FadeImageAndChangeScene());
    }

    IEnumerator FadeImageFromZeroToFull()
    {
        float elapsedTime = 0.0f;
        Color startColor = imageToFade.color;
        startColor.a = 1; // ���� �� ������ �������ϰ� ����
        imageToFade.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(1.0f - elapsedTime / fadeDuration);
            imageToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
    }

    IEnumerator FadeImageAndChangeScene()
    {
        float elapsedTime = 0.0f;
        Color startColor = imageToFade.color;
        startColor.a = 0; // ���� �� ������ �����ϰ� ����
        imageToFade.color = startColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            imageToFade.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(0); // ù ��° ������ ���ư���
    }
}

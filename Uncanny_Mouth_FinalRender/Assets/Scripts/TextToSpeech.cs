using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

public class TextToSpeech : MonoBehaviour
{
    public AudioSource audioSource;
    private string clientId = "4u7r17k8x6";
    private string clientSecret = "Y3EGOZgZLWfzRsFDYxixyYM5cncNytvwCSfMq9LH";
    public string filePath = "D://temp.mp3"; // 저장할 파일 경로
    private WebSocketReceiver wsReceiver;


    private void Start()
    {
        // WebSocketReceiver 인스턴스를 찾습니다.
        wsReceiver = FindObjectOfType<WebSocketReceiver>();

        if (wsReceiver == null)
        {
            Debug.LogError("Ws not found on the same GameObject as TTS.");
        }

        //StartCoroutine(ConvertAndPlayCoroutine("안녕하세요. 철학에 대해 어떤 질문이 있으신가요?"));
    }

    private void Update()
    {
        if (wsReceiver != null)
        {
            if (wsReceiver.start)
            {
                wsReceiver.start = false; // Reset the start flag after handling
                StartCoroutine(ConvertAndPlayCoroutine("안녕하세요. 철학에 대해 어떤 질문이 있으신가요?"));
                
            }

            if (wsReceiver.end)
            {
                wsReceiver.end = false; // Reset the end flag after handling
                StartCoroutine(ConvertAndPlayCoroutine("만나서 반가웠습니다. 당신의 철학적 상태를 분석해 주겠습니다."));
            }
        }
    }

    public IEnumerator ConvertAndPlayCoroutine(string textToSpeak)
    {
        // 텍스트를 클로바 TTS API로 보내고 오디오 파일을 다운로드하여 재생합니다.
        yield return DownloadAndPlayAudioAsync(textToSpeak);
    }

    private async Task DownloadAndPlayAudioAsync(string text)
    {
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent($"speaker=nmovie&volume=0&speed=-1&pitch=1&alpha=-1&format=mp3&text={text}",
                                            Encoding.UTF8,
                                            "application/x-www-form-urlencoded");

            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY-ID", clientId);
            client.DefaultRequestHeaders.Add("X-NCP-APIGW-API-KEY", clientSecret);


            HttpResponseMessage response = await client.PostAsync("https://naveropenapi.apigw.ntruss.com/tts-premium/v1/tts", content);

            if (response.IsSuccessStatusCode)
            {

                using (Stream input = await response.Content.ReadAsStreamAsync())
                using (FileStream output = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {

                    await input.CopyToAsync(output);
                }
                Debug.Log(filePath + " was created");



                StartCoroutine(PlayAudioFromPath(filePath));
            }
            else
            {
                Debug.LogError($"Error: {response.StatusCode}");
            }
        }
    }

    private IEnumerator PlayAudioFromPath(string filePath)
    {
        Debug.Log("PlayAudioFromPath Called");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
            else
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
        }
        Debug.Log("Exiting coroutine");
        yield break;
    }

    

}
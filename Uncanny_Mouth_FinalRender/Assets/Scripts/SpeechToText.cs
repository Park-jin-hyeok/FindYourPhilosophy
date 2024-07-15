using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.IO;
using System;
using UnityEngine.UI;
using System.Linq.Expressions;

public class SpeechToText : MonoBehaviour
{
    private string _microphoneID = null;
    private AudioClip _recording = null;
    private int _recordingLengthSec = 10; // 녹음 시간 (초)
    private int _recordingHZ = 44100; // 샘플링 레이트

    public bool triggerRecording = false;
    private bool _isRecording = false;

    private bool fakeMicMode = false;

    static string resultText;
    public Text resultTextUI; // UI 텍스트 요소를 참조하는 변수

    // ChatManager의 인스턴스를 참조하는 변수
    private ChatManager chatManager;


    private void Start()
    {
        // Get a reference to ChatManager
        chatManager = GetComponent<ChatManager>();

        try
        {
            if (chatManager == null)
            {
                Debug.LogError("ChatManager not found on the same GameObject as SpeechToText.");
            }



            _microphoneID = Microphone.devices[0];
        }
        catch
        {
            Debug.Log("Activating Fake Mic Mode");
            fakeMicMode = true;
        }
    }

    private void StartRecording()
    {
        _isRecording = true;
        Debug.Log("Start recording");
        if (!fakeMicMode)
        {
            _recording = Microphone.Start(_microphoneID, false, _recordingLengthSec, _recordingHZ);
            
        }
        else
        {

        }
    }

    private void StopRecording()
    {
        Debug.Log(Microphone.IsRecording(_microphoneID));
        _isRecording = false;
        Debug.Log("StopRecording");
        if (!fakeMicMode)
        {
            if (!Microphone.IsRecording(_microphoneID))
            {
                return;
            }
            if (Microphone.IsRecording(_microphoneID))
            {
                Microphone.End(_microphoneID);
                Debug.Log("Stop recording");
                

                if (_recording == null)
                {
                    Debug.LogError("Nothing recorded");
                    return;
                }

                Debug.Log($"Recorded AudioClip length: {_recording.length} seconds");
                Debug.Log($"Recorded AudioClip samples: {_recording.samples}");

                byte[] byteData = WavUtility.FromAudioClip(_recording);
                SaveWavFile("D:/temp_1.wav", _recording);

                if (byteData == null || byteData.Length == 0)
                {
                    Debug.LogError("WavUtility returned an empty byte array");
                }
                else
                {
                    Debug.Log($"Wav byte array length: {byteData.Length}");
                }

                StartCoroutine(PostVoice("https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor", byteData));
                _recording = null;
            }
            return;
        }
        else
        {
            return;
        }

    }

    private void Update()
    {

        if (triggerRecording == true) // recording triggered 
        {
            triggerRecording = false;

            if (_isRecording == false) // if not recording record 
            {
                Debug.Log("Recording Start");
                _isRecording = true;
                // recording is triggered 

                if (!fakeMicMode)
                {
                    StartRecording();
                }
                else
                {

                }
            }
            else // if recording 
            {
                Debug.Log("Recording Finished");
                _isRecording = false;

                if(!fakeMicMode)
                {
                    StopRecording();
                }
                else
                {

                }
            }

            
        }
    }


    public class WavUtility
    {
        const int BlockSize_16Bit = 2;

        public static AudioClip ToAudioClip(string filePath)
        {
            if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith(Application.dataPath))
            {
                Debug.LogWarning("This only supports files that are stored using Unity's Application data path.");
                return null;
            }
            byte[] fileBytes = File.ReadAllBytes(filePath);
            return ToAudioClip(fileBytes, 0);
        }

        public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
        {
            int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
            UInt16 audioFormat = BitConverter.ToUInt16(fileBytes, 20);

            string formatCode = FormatCode(audioFormat);
            Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

            UInt16 channels = BitConverter.ToUInt16(fileBytes, 22);
            int sampleRate = BitConverter.ToInt32(fileBytes, 24);
            UInt16 bitDepth = BitConverter.ToUInt16(fileBytes, 34);

            int headerOffset = 16 + 4 + subchunk1 + 4;
            int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);

            float[] data;
            switch (bitDepth)
            {
                case 8:
                    data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 16:
                    data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 24:
                    data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                case 32:
                    data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
                    break;
                default:
                    throw new Exception(bitDepth + " bit depth is not supported.");
            }

            AudioClip audioClip = AudioClip.Create(name, data.Length, (int)channels, sampleRate, false);
            audioClip.SetData(data, 0);
            return audioClip;
        }

        private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            float[] data = new float[wavSize];

            sbyte maxValue = sbyte.MaxValue;

            int i = 0;
            while (i < wavSize)
            {
                data[i] = (float)source[i] / maxValue;
                ++i;
            }

            return data;
        }

        private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 16-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(Int16);
            int convertedSize = wavSize / x;

            float[] data = new float[convertedSize];

            Int16 maxValue = Int16.MaxValue;

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = 3;
            int convertedSize = wavSize / x;

            int maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            byte[] block = new byte[sizeof(int)];

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                Buffer.BlockCopy(source, offset, block, 1, x);
                data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
        {
            int wavSize = BitConverter.ToInt32(source, headerOffset);
            headerOffset += sizeof(int);
            Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

            int x = sizeof(float);
            int convertedSize = wavSize / x;

            Int32 maxValue = Int32.MaxValue;

            float[] data = new float[convertedSize];

            int offset = 0;
            int i = 0;
            while (i < convertedSize)
            {
                offset = i * x + headerOffset;
                data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
                ++i;
            }

            Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

            return data;
        }

        public static byte[] FromAudioClip(AudioClip audioClip)
        {
            string file;
            return FromAudioClip(audioClip, out file, false);
        }

        public static byte[] FromAudioClip(AudioClip audioClip, out string filepath, bool saveAsFile = true, string dirname = "recordings")
        {
            MemoryStream stream = new MemoryStream();

            const int headerSize = 44;

            UInt16 bitDepth = 16;

            int fileSize = audioClip.samples * BlockSize_16Bit + headerSize;

            WriteFileHeader(ref stream, fileSize);
            WriteFileFormat(ref stream, audioClip.channels, audioClip.frequency, bitDepth);
            WriteFileData(ref stream, audioClip, bitDepth);

            byte[] bytes = stream.ToArray();

            Debug.AssertFormat(bytes.Length == fileSize, "Unexpected AudioClip to wav format byte count: {0} == {1}", bytes.Length, fileSize);

            if (saveAsFile)
            {
                filepath = string.Format("{0}/{1}/{2}.{3}", Application.persistentDataPath, dirname, DateTime.UtcNow.ToString("yyMMdd-HHmmss-fff"), "wav");
                Directory.CreateDirectory(Path.GetDirectoryName(filepath));
                File.WriteAllBytes(filepath, bytes);
            }
            else
            {
                filepath = null;
            }

            stream.Dispose();

            return bytes;
        }

        private static int WriteFileHeader(ref MemoryStream stream, int fileSize)
        {
            int count = 0;
            int total = 12;

            byte[] riff = Encoding.ASCII.GetBytes("RIFF");
            count += WriteBytesToMemoryStream(ref stream, riff);

            int chunkSize = fileSize - 8;
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(chunkSize));

            byte[] wave = Encoding.ASCII.GetBytes("WAVE");
            count += WriteBytesToMemoryStream(ref stream, wave);

            Debug.AssertFormat(count == total, "Unexpected wav descriptor byte count: {0} == {1}", count, total);

            return count;
        }

        private static int BytesPerSample(UInt16 bitDepth)
        {
            return bitDepth / 8;
        }

        private static int WriteFileFormat(ref MemoryStream stream, int channels, int sampleRate, UInt16 bitDepth)
        {
            int count = 0;
            int total = 24;

            byte[] id = Encoding.ASCII.GetBytes("fmt ");
            count += WriteBytesToMemoryStream(ref stream, id);

            int subchunk1Size = 16;
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk1Size));

            UInt16 audioFormat = 1;
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(audioFormat));

            UInt16 numChannels = Convert.ToUInt16(channels);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(numChannels));

            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(sampleRate));

            int byteRate = sampleRate * channels * BytesPerSample(bitDepth);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(byteRate));

            UInt16 blockAlign = Convert.ToUInt16(channels * BytesPerSample(bitDepth));
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(blockAlign));

            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(bitDepth));

            Debug.AssertFormat(count == total, "Unexpected wav fmt byte count: {0} == {1}", count, total);

            return count;
        }

        private static int WriteFileData(ref MemoryStream stream, AudioClip audioClip, UInt16 bitDepth)
        {
            int count = 0;
            int total = 8;

            float[] data = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(data, 0);

            byte[] bytes = ConvertAudioClipDataToInt16ByteArray(data);

            byte[] id = Encoding.ASCII.GetBytes("data");
            count += WriteBytesToMemoryStream(ref stream, id);

            int subchunk2Size = Convert.ToInt32(audioClip.samples * BlockSize_16Bit);
            count += WriteBytesToMemoryStream(ref stream, BitConverter.GetBytes(subchunk2Size));

            Debug.AssertFormat(count == total, "Unexpected wav data id byte count: {0} == {1}", count, total);

            count += WriteBytesToMemoryStream(ref stream, bytes);

            Debug.AssertFormat(bytes.Length == subchunk2Size, "Unexpected AudioClip to wav subchunk2 size: {0} == {1}", bytes.Length, subchunk2Size);

            return count;
        }

        private static byte[] ConvertAudioClipDataToInt16ByteArray(float[] data)
        {
            MemoryStream dataStream = new MemoryStream();

            int x = sizeof(Int16);

            Int16 maxValue = Int16.MaxValue;

            int i = 0;
            while (i < data.Length)
            {
                dataStream.Write(BitConverter.GetBytes(Convert.ToInt16(data[i] * maxValue)), 0, x);
                ++i;
            }
            byte[] bytes = dataStream.ToArray();

            Debug.AssertFormat(data.Length * x == bytes.Length, "Unexpected float[] to Int16 to byte[] size: {0} == {1}", data.Length * x, bytes.Length);

            dataStream.Dispose();

            return bytes;
        }

        private static int WriteBytesToMemoryStream(ref MemoryStream stream, byte[] bytes)
        {
            int count = bytes.Length;
            stream.Write(bytes, 0, count);
            return count;
        }

        private static string FormatCode(UInt16 code)
        {
            switch (code)
            {
                case 1:
                    return "PCM";
                case 2:
                    return "ADPCM";
                case 3:
                    return "IEEE";
                case 7:
                    return "μ-law";
                case 65534:
                    return "WaveFormatExtensable";
                default:
                    Debug.LogWarning("Unknown wav code format:" + code);
                    return "";
            }
        }

    }

    public void RecognizeFile(string filePath)
    {
        // file to byte 
        string url = "https://naveropenapi.apigw.ntruss.com/recog/v1/stt?lang=Kor";
        byte[] bytesS = ConvertWavFileToBytes(filePath);

        // send to 
        StartCoroutine(PostVoice(url, bytesS));
    }

    public static byte[] ConvertWavFileToBytes(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The specified file was not found.", filePath);
        }

        return File.ReadAllBytes(filePath);
    }

    private void SaveWavFile(string filePath, AudioClip clip)
    {
        // Convert the AudioClip data to WAV format
        byte[] wavData = WavUtility.FromAudioClip(clip);

        // Write the WAV data to a file
        File.WriteAllBytes(filePath, wavData);

        Debug.Log("Saved recording to " + filePath);
    }



    private void DisplayResultText(string text)
    {
        if (resultTextUI != null)
        {
            resultTextUI.text = text;
        }
        else
        {
            Debug.LogWarning("resultTextUI is not assigned.");
        }
    }

    private IEnumerator PostVoice(string url, byte[] byteData)
    {
        WWWForm form = new WWWForm();
        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            Debug.Log("Good");
            request.uploadHandler = new UploadHandlerRaw(byteData);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            request.SetRequestHeader("X-NCP-APIGW-API-KEY-ID", "109r2t226l");
            request.SetRequestHeader("X-NCP-APIGW-API-KEY", "mY5ed0RkOVEsbcCzgMCrzQEE2LBdgzXonoE4kp6L");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                Debug.LogError(request.responseCode);
            }
            else
            {
                resultText = request.downloadHandler.text;
                Debug.Log("STT Result: " + resultText);
                DisplayResultText(resultText);

                // 인식된 텍스트를 ChatManager에 전달하여 GPT에 요청
                if (chatManager != null)
                {
                    chatManager.AskChatGPT(resultText);
                }
                else
                {
                    Debug.LogWarning("ChatManager is not assigned.");
                }
            }
        }
    }
}

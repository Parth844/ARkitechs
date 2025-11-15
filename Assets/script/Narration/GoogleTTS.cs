using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleTTS : MonoBehaviour
{
    public string apiKey;
    public AudioSource audioSource;

    [Serializable]
    public class TTSResponse
    {
        public string audioContent;
    }

    public IEnumerator Speak(string text)
    {
        string url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={apiKey}";

        string json = "{ \"input\": { \"text\": \"" + text + "\" }, " +
                      "\"voice\": { \"languageCode\": \"en-US\", \"ssmlGender\": \"NEUTRAL\" }, " +
                      "\"audioConfig\": { \"audioEncoding\": \"LINEAR16\" }}";

        UnityWebRequest req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("TTS Error: " + req.error);
            yield break;
        }

        TTSResponse res = JsonUtility.FromJson<TTSResponse>(req.downloadHandler.text);

        byte[] audioData = Convert.FromBase64String(res.audioContent);

        AudioClip clip = Convert16BitPCM(audioData);
        audioSource.PlayOneShot(clip);
    }

    // Convert LINEAR16 PCM -> Unity AudioClip
    AudioClip Convert16BitPCM(byte[] data)
    {
        int sampleCount = data.Length / 2;
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            short value = BitConverter.ToInt16(data, i * 2);
            samples[i] = value / 32768f;
        }

        AudioClip clip = AudioClip.Create("tts_audio", sampleCount, 1, 16000, false);
        clip.SetData(samples, 0);
        return clip;
    }
}

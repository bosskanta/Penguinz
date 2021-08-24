using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class AudioPlayer : MonoBehaviour
{
    public GameObject thisNote;
    public AudioSource audioSource;
    public Button toggleButton, replayButton;
    public TMP_Text playtime, length;

    private AudioClip audioClip;
    private string audioPath;
    private bool isStart, showPause;

    // isToggle == true -> Play (show || button)
    // isToggle == false -> Stop (show > button)

    private async void Start()
    {
        string audioFile = thisNote.name.Split(' ')[2] + ".wav";
        audioPath = Path.Combine(Application.persistentDataPath, "audio", audioFile);

        audioClip = await LoadClip(audioPath);

        audioSource = GetComponent<AudioSource>();

        audioSource.clip = audioClip;

        float len = audioClip.length;
        string minute = ((int)(len / 60)).ToString();
        string second = ((int)(len % 60)).ToString();

        if (minute.Length == 1) 
        {
            minute = "0" + minute;
        }

        if (second.Length == 1)
        {
            second = "0" + second;
        }

        length.text = $"/ {minute}:{second}";

        isStart = false;
        showPause = false;
    }

    async Task<AudioClip> LoadClip(string path)
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
        {
            uwr.SendWebRequest();

            // wrap tasks in try/catch, otherwise it'll fail silently
            try
            {
                while (!uwr.isDone)
                {
                    await Task.Delay(5);
                }

                if (uwr.result == UnityWebRequest.Result.ConnectionError)
                {
                    Debug.Log($"{uwr.error}");
                }
                else
                {
                    //print(path);
                    audioClip = DownloadHandlerAudioClip.GetContent(uwr);
                }
            }
            catch (Exception err)
            {
                Debug.Log($"{err.Message}, {err.StackTrace}");
            }
        }

        //print("loaded audio clip : " + path);
        return audioClip;
    }

    public void OnToggle()
    {
        // This happen once.
        if (!isStart)
        {
            isStart = true;
            audioSource.Play();
            ShowPause(true);
        } 
        else if (isStart && showPause)
        {
            audioSource.Pause();
            ShowPause(false);
        }
        else if (!showPause)
        {
            audioSource.UnPause();
            ShowPause(true);
        }
    }

    public void OnReplay()
    {
        audioSource.Stop();
        isStart = false;
        ShowPause(false);
        playtime.text = "00:00";
    }

    private void ShowPause(bool isPause)
    {
        showPause = isPause;

        if (isPause)
        {
            toggleButton.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Icons/pause");
        }
        else
        {
            toggleButton.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Icons/play");
        }
    }

    private void Update()
    {
        if (audioSource.isPlaying)
        {
            string minute = ((int)(audioSource.time / 60)).ToString();
            if (minute.Length == 1) minute = "0" + minute;
            string second = ((int)(audioSource.time % 60)).ToString();
            if (second.Length == 1) second = "0" + second;

            playtime.text = $"{minute}:{second}";
        }

        // Audio plays to the end.
        else if (!audioSource.isPlaying && showPause)
        {
            isStart = false;
            ShowPause(false);
        }
    }
}

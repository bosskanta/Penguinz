using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioRecord : MonoBehaviour
{
    public TMP_Text recordStatus;
    public Button recordButton;
    public Sprite recordSprite;
    public Sprite disabledSprite;

    private string micDevice;
    private float startRecordingTime;
    private AudioClip recording;
    private float delay = 0.3f;

    public void Start()
    {
        micDevice = Microphone.devices[0];
    }

    public void TouchRecord()
    {
        if(!Microphone.IsRecording(micDevice))
        {
            StartRecord();
        }
        else
        {
            StopRecord();
        }
    }

    public void StartRecord()
    {
        recording = Microphone.Start(micDevice, false, 1800, 44100);
        startRecordingTime = Time.time;

        recordButton.transform.LeanScale(Vector2.zero, delay).setEaseInExpo().setOnComplete(SpriteRecording);

        recordStatus.text = $"Recording...\nTouch again to stop";
    }

    public void StopRecord()
    {
        Microphone.End(micDevice);
        recordStatus.text = "Recorded";

        recordButton.transform.LeanScale(Vector2.zero, delay).setEaseInExpo().setOnComplete(SpriteDisable);

        //Trim the audioclip by the length of the recording
        AudioClip recordingNew = AudioClip.Create(recording.name, (int)((Time.time - startRecordingTime) * recording.frequency), recording.channels, recording.frequency, false);
        float[] data = new float[(int)((Time.time - startRecordingTime) * recording.frequency)];
        recording.GetData(data, 0);
        recordingNew.SetData(data, 0);
        recording = recordingNew;

        SavWav.Save(NoteManager.instance.Id, recording);
    }

    private void SpriteRecording()
    {
        recordButton.image.sprite = recordSprite;
        recordButton.transform.LeanScale(Vector2.one, delay).setEaseOutExpo();
    }

    private void SpriteDisable()
    {
        recordButton.image.sprite = disabledSprite;
        recordButton.enabled = false;
        recordButton.transform.LeanScale(Vector2.one, delay).setEaseOutExpo();
    }
}

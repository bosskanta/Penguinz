using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoteController : MonoBehaviour
{
    public CanvasGroup background;
    public Transform notePanel;

    private float delay = 0.17f;
    private void OnEnable()
    {
        background.alpha = 0;
        background.LeanAlpha(1, 0.5f).delay = delay;

        notePanel.localPosition = new Vector2(0, -Screen.height);
        notePanel.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = delay;

        if (NoteValueHolder.instance.isDateChangeAble)
        {
            NoteManager.instance.SetChangeAbleDate();
        }
        else
        {
            NoteManager.instance.SetShownDate();
        }

    }

    private void Update()
    {
        // Adjust current hour:minute
        if (!NoteValueHolder.instance.isDateChangeAble)
        {
            NoteManager.instance.SetShownDate();
        }
    }

    public void CloseNote()
    {
        background.LeanAlpha(0, 0.5f);
        notePanel.LeanMoveLocalY(-Screen.height, 0.3f).setEaseInExpo().setOnComplete(OnCloseNote);
    }

    private void OnCloseNote()
    {
        gameObject.SetActive(false);
        NoteManager.instance.ResetRecording();
        NoteManager.instance.ResetNote();
        MyAnimation.instance.Interact();
    }
}

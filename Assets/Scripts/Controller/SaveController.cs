using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    [SerializeField] private GameObject checkAfter;
    public CanvasGroup background, noteBackground;
    public Transform savedPanel;
    public Transform notePanel;
    public GameObject note;

    public void OnClickSave()
    {
        if (NoteManager.instance.SaveNote())
        {
            background.LeanAlpha(1, 0.5f);
            gameObject.SetActive(true);
        } 
        //else if (NoteManager.instance.Level.Equals("")) // Shake what level text
        //{
        //    NoteManager.instance.ShakeText();
        //}
    }

    private void OnEnable()
    {
        noteBackground.alpha = 0;
        notePanel.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo().setOnComplete(OnCloseNote);
        savedPanel.localScale = Vector2.zero;
        savedPanel.LeanScale(Vector2.one, 0.3f).setEaseOutBack();
    }

    public void CloseSavedScreen()
    {
        // This reset note data after 0.5 second
        background.LeanAlpha(0, 0.5f).setOnComplete(NoteManager.instance.ResetNote).delay = 0.5f;
        background.LeanAlpha(0, 0.5f);
        savedPanel.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnCloseSavedScreen);
    }

    private void OnCloseSavedScreen()
    {
        CalendarManager.instance.RefreshCalendar();
        MyAnimation.instance.Interact();
        checkAfter.SetActive(true);
        gameObject.SetActive(false);
    }

    private void OnCloseNote()
    {
        note.SetActive(false);

        NoteValueHolder.instance.ResetSelectedDate();
        NoteManager.instance.defaultDate.gameObject.SetActive(false);
        NoteManager.instance.changeableDate.gameObject.SetActive(false);
        NoteManager.instance.changeableDate.transform.GetChild(3).gameObject.SetActive(false);
    }
}

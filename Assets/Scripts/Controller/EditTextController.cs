using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EditTextController : MonoBehaviour
{
    public CanvasGroup bg;
    public Transform noteText;
    public TMPro.TMP_InputField input;

    private void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.3f);

        noteText.localScale = Vector2.zero;
        noteText.LeanScale(Vector2.one, 0.3f).setEaseOutBack();

        Note note = CalendarManager.instance.GetThisMonthNotes().Find(n => n.id == NoteValueHolder.instance.ID);
        input.text = note.text;
    }

    public void Save()
    {
        NoteManager.instance.EditNoteText(NoteValueHolder.instance.ID, input.text);

        // Change text of the gameobject
        GameObject note = GameObject.Find(
            NoteValueHolder.instance.NoteLevel + " " + NoteValueHolder.instance.TimeStamp + " " + NoteValueHolder.instance.ID + " " + NoteValueHolder.instance.DayOfWeek
        );

        if (input.text.Equals(""))
        {
            note.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                        .fontStyle = FontStyles.Italic;
            note.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                .text = $"This note has no note text.";
        } 
        else
        {
            note.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                        .fontStyle = FontStyles.Normal;
            note.transform.GetChild(2).gameObject.GetComponent<TMPro.TMP_Text>().text = input.text;
        }

        Close();
    }

    public void Close()
    {
        bg.LeanAlpha(0, 0.3f);
        noteText.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

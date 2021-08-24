using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteNoteController : MonoBehaviour
{
    public CanvasGroup bg;
    public Transform deleteBox;
    public TMPro.TMP_Text[] texts;

    private void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.3f);

        deleteBox.localScale = Vector2.zero;
        deleteBox.LeanScale(Vector2.one, 0.3f).setEaseOutBack();

        texts[0].text = NoteValueHolder.instance.NoteLevel;
        switch (NoteValueHolder.instance.NoteLevel)
        {
            case "Annoy":
                texts[0].color = MyColors.Annoy; break;
            case "Angry":
                texts[0].color = MyColors.Angry; break;
            case "Furious":
                texts[0].color = MyColors.Furious; break;
        }

        texts[1].text = NoteValueHolder.instance.TimeStamp;
    }

    public void DeleteNote()
    {
        Destroy(GameObject.Find(
            NoteValueHolder.instance.NoteLevel + " " + NoteValueHolder.instance.TimeStamp + " " + NoteValueHolder.instance.ID + " " + NoteValueHolder.instance.DayOfWeek
        ));
        Close();

        NoteManager.instance.DeleteNote(NoteValueHolder.instance.ID);
        NoteManager.instance.ResetNote();
        CalendarManager.instance.RefreshCalendar();
        DayManager.instance.RefreshContent();
    }

    public void Close()
    {
        bg.LeanAlpha(0, 0.3f);
        deleteBox.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

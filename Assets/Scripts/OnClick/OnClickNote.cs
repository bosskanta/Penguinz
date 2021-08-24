using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OnClickNote : MonoBehaviour
{
    public void DisplayDeleteBox(GameObject thisNote)
    {
        SetNoteValueHolder(thisNote);

        GameObject deleteBox = GameObject.Find("DeleteParent").transform.GetChild(0).gameObject;
        deleteBox.SetActive(true);
    }

    public void DisplayTextBox(GameObject thisNote)
    {
        SetNoteValueHolder(thisNote);

        GameObject textBox = GameObject.Find("NoteTextParent").transform.GetChild(0).gameObject;
        textBox.SetActive(true);
    }

    private void SetNoteValueHolder(GameObject thisNote)
    {
        string[] str = thisNote.name.Split(' ');
        NoteValueHolder.instance.NoteLevel = str[0];
        NoteValueHolder.instance.TimeStamp = str[1];
        NoteValueHolder.instance.ID = str[2];
        NoteValueHolder.instance.DayOfWeek = str[3];
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoteValueHolder : MonoBehaviour
{
    public static NoteValueHolder instance;

    public string NoteLevel { get; set; }
    public string TimeStamp { get; set; }
    public string ID { get; set; }
    public string DayOfWeek { get; set; }
    public DateTime SelectedDate { get; set; }
    public bool isDateChangeAble { get; set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance)
        {
            Destroy(gameObject);
        }

        SelectedDate = DateTime.Now.Date;
        isDateChangeAble = false;
    }

    public void ResetSelectedDate()
    {
        SelectedDate = DateTime.Now.Date;
        isDateChangeAble = false;
    }
}

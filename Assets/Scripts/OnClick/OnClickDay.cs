using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickDay : MonoBehaviour
{
    private RectTransform toastTransform;

    private void Awake()
    {
        toastTransform = (RectTransform)GameObject.Find("Toast").transform;
    }

    public void DisplayDay()
    {
        string dayNum = gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().text;
        int dayNumInt = Int32.Parse(dayNum);

        int month = CalendarManager.instance.calendarDate.Month; 
        int year = CalendarManager.instance.calendarDate.Year;

        if (dayNumInt > 20 && gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().color.Equals(MyColors.WhiteAlpha))
        {
            month--;
            if (month == 0)
            {
                month = 12;
                year--;
            }
        }
        else if (dayNumInt < 15 && gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().color.Equals(MyColors.WhiteAlpha))
        {
            month++;
            if (month > 12)
            {
                month = 1;
                year++;
            }
        }

        DateTime clickedDay = new DateTime(year, month, dayNumInt);
        DateTime today = DateTime.Now.Date;

        // Display ONLY the day on month
        if (clickedDay <= today && !gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().color.Equals(MyColors.WhiteAlpha))
        {
            DayManager.instance.SetContent(dayNum);
            GameObject dayScreen = GameObject.Find("Day").transform.GetChild(0).gameObject;
            dayScreen.gameObject.SetActive(true);
        }
        else if ( clickedDay > today && CalendarManager.instance.IsToastActive() == false)
        {
            ShowToast("This day is yet to come", 416);
        }
        else if (gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().color.Equals(MyColors.WhiteAlpha)
            && clickedDay.Month < CalendarManager.instance.calendarDate.Month
            && CalendarManager.instance.IsToastActive() == false)
        {
            ShowToast("The day is on earlier month", 464);
        }
        else if (gameObject.transform.GetChild(2).GetComponent<TMPro.TMP_Text>().color.Equals(MyColors.WhiteAlpha)
           && clickedDay.Month > CalendarManager.instance.calendarDate.Month
           && CalendarManager.instance.IsToastActive() == false)
        {
            ShowToast("The day is on next month", 464);
        }
    }

    private void ShowToast(string message, int width)
    {
        CalendarManager.instance.SetToastActive(true);
        toastTransform.sizeDelta = new Vector2(width, toastTransform.rect.height);
        toastTransform.GetChild(1).gameObject.GetComponent<TMPro.TMP_Text>().text = message;
        toastTransform.localScale = Vector3.zero;
        toastTransform.LeanScale(Vector3.one, 0.1f);
        toastTransform.LeanScale(Vector3.zero, 0.07f).setOnComplete(DeactivateToast).delay = 1f;
    }

    private void DeactivateToast()
    {
        CalendarManager.instance.SetToastActive(false);
    }
}

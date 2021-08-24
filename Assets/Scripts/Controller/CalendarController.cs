using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarController : MonoBehaviour
{
    public Transform calendar;
    private void OnEnable()
    {
        calendar.localPosition = new Vector2(0, -Screen.height);
        calendar.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.2f;
    }
    public void CloseCalendar()
    {
        calendar.LeanMoveLocalY(-Screen.height, 0.3f).setEaseInExpo().setOnComplete(OnClose).delay = 0.15f;
    }
    private void OnClose()
    {
        gameObject.SetActive(false);
        MyAnimation.instance.Interact();
    }
}

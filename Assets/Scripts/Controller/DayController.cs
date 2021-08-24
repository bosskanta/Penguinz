using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayController : MonoBehaviour
{
    public Transform day;

    private void OnEnable()
    {
        day.localPosition = new Vector2(((RectTransform)day.transform).rect.width, 0);
        day.LeanMoveLocalX(0, 0.5f).setEaseOutExpo().delay = 0.15f;
    }

    public void CloseToCalendar()
    {
        day.LeanMoveLocalX(((RectTransform)day.transform).rect.width, 0.5f).setEaseOutExpo().setOnComplete(OnClose).delay = 0.15f; ;
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

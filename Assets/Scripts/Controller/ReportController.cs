using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportController : MonoBehaviour
{
    public CanvasGroup background;
    public Transform reportPanel;
    private void OnEnable()
    {
        background.alpha = 0;
        background.LeanAlpha(1, 0.5f);

        reportPanel.localScale = Vector2.zero;
        reportPanel.LeanScale(Vector2.one, 0.5f).setEaseOutBack().delay = 0.3f;
    }
    public void CloseReport()
    {
        background.LeanAlpha(0, 0.5f);
        reportPanel.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }
    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

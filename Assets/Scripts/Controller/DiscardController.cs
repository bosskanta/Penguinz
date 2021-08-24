using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscardController : MonoBehaviour
{
    public CanvasGroup background;
    public Transform discardPanel;

    private float delay = 0.3f;

    private void OnEnable()
    {
        background.alpha = 0;
        background.LeanAlpha(1, delay);

        discardPanel.localScale = Vector2.zero;
        discardPanel.LeanScale(Vector2.one, delay).setEaseOutBack();
    }

    public void OnClickDiscard()
    {
        // This reset note data after 0.5 second
        background.LeanAlpha(0, delay).setOnComplete(AfterClickDiscard).delay = 0.5f;
        CloseDiscardPanel();
    }

    public void AfterClickDiscard()
    {
        NoteManager.instance.ResetNote();
        NoteManager.instance.DeleteAudio(NoteManager.instance.Id);
    }

    public void CloseDiscardPanel()
    {
        background.LeanAlpha(0, delay);
        discardPanel.LeanScale(Vector2.zero, delay).setEaseInBack().setOnComplete(OnCloseDiscardPanel);
    }

    private void OnCloseDiscardPanel()
    {
        gameObject.SetActive(false);
    }
}

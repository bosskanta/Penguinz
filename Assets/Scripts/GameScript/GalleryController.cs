using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryController : MonoBehaviour
{
    public CanvasGroup bg;
    public Transform box;

    private void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.3f);

        box.localScale = Vector2.zero;
        box.LeanScale(new Vector2(1.5f, 1.5f), 0.3f).setEaseOutBack();
    }

    public void Close()
    {
        bg.LeanAlpha(0, 0.3f);
        box.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

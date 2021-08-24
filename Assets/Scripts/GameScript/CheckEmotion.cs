using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckEmotion : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    public CanvasGroup bg;
    public Transform box;

    private void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.3f);

        box.localScale = Vector2.zero;
        box.LeanScale(new Vector2(1.5f, 1.5f), 0.3f).setEaseOutBack();
    }

    public void ConfirmNewLevel()
    {
        GameFileManager.instance.SaveNewGame((int)_slider.value);

        bg.LeanAlpha(0, 0.3f);
        box.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

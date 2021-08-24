using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseGameController : MonoBehaviour
{
    [SerializeField] Slider _slider;
    public CanvasGroup bg;
    public Transform popup;

    private void OnEnable()
    {
        if (NoteManager.instance.IsTakingNoteBefore)
        {
            bg.alpha = 0;
            bg.LeanAlpha(1, 0.3f);
            print("Taking note before : " + NoteManager.instance.GetLatestLevelBefore());
        }
        print("Not taking note : " + NoteManager.instance.Level);

        popup.localScale = Vector2.zero;
        popup.LeanScale(Vector2.one, 0.3f).setEaseOutBack().delay = 0.2f;
    }

    public void Close()
    {
        bg.LeanAlpha(0, 0.3f);
        popup.LeanScale(Vector2.zero, 0.3f).setEaseInBack();
    }

    public void Cancle()
    {
        GameFileManager.instance.GameName = "";
        bg.LeanAlpha(0, 0.3f);
        popup.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);
    }

    private void OnClose()
    {
        _slider.value = 1;
        NoteManager.instance.Level = 1;
        MyAnimation.instance.Interact();

        GameObject playButton = GameObject.Find("ChoicesPopup").transform.GetChild(0).GetChild(0).gameObject;
        playButton.SetActive(false);
        gameObject.SetActive(false);
    }
}

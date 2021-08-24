using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AngerCheckController : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _levelImage;
    [SerializeField] private TMP_Text _levelText;

    public CanvasGroup bg;
    public Transform popup;
    public GameObject chooseGame;

    private void Start()
    {
        NoteManager.instance.Level = 1;

        _slider.onValueChanged.AddListener((v) => {
            switch (v)
            {
                case 1:
                    _levelText.text = "Furious";
                    break;

                case 2:
                    _levelText.text = "Angry";
                    break;

                case 3:
                    _levelText.text = "Annoy";
                    break;
            }

            NoteManager.instance.Level = (int)v;
            _levelImage.sprite = Resources.Load<Sprite>($"Arts/{_levelText.text}");
        });
    }

    private void OnEnable()
    {
        bg.alpha = 0;
        bg.LeanAlpha(1, 0.3f);

        popup.localScale = Vector2.zero;
        popup.LeanScale(Vector2.one, 0.3f).setEaseOutBack().delay = 0.2f;
    }

    public void Close()
    {
        bg.LeanAlpha(0, 0.3f);
        popup.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(ResetLevel);
    }

    public void ToChooseGame(bool isSkip)
    {
        if (isSkip)
        {
            NoteManager.instance.Level = -1;
        }

        bg.LeanAlpha(0, 0.3f);
        popup.LeanScale(Vector2.zero, 0.3f).setEaseInBack().setOnComplete(OnClose);

        chooseGame.SetActive(true);
    }

    private void ResetLevel()
    {
        NoteManager.instance.Level = 1;
        _slider.value = 1;
        OnClose();
    }

    private void OnClose()
    {
        gameObject.SetActive(false);
    }
}

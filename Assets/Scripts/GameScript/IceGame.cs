using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IceGame : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _levelImage;
    [SerializeField] private TMP_Text _levelText;

    public static IceGame instance;
    public Transform ice, animal, progressBar, buttonPanel;
    public TMP_Text counterText;
    public GameObject checkEmotion;
    public Image[] figures;

    private float startPosition;
    private float threshold, barProgress;
    private int count, maxCount, state, imageNumber;
    private int level;
    private bool isStarted;

    public int PlayTimes { get; set; }

    private float distance = 10f, delay = 0.1f;

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

        if (NoteManager.instance.Level != -1)
        {
            level = NoteManager.instance.Level;
            GameFileManager.instance.Level = level;
        }

        print(GameFileManager.instance.GameName + " : " + level);

        switch (level)
        {
            case 1:
                maxCount = 100; break;

            case 2:
                maxCount = 80; break;

            default:
                maxCount = 60; break;
        }

        // Debug
        maxCount = 10;

        startPosition = ice.localPosition.x;
        count = 0;
        state = 1;

        threshold = maxCount / 3;
        barProgress = 1f / maxCount;

        // Random image
        imageNumber = Random.Range(1, 4);
        animal.gameObject.SetActive(false);
        animal.GetComponent<CanvasGroup>().alpha = 0;
        animal.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Ice/{imageNumber}");

        buttonPanel.localScale = Vector2.zero;

        progressBar.gameObject.SetActive(false);
        progressBar.GetChild(0).localScale = new Vector2(0, 1);

        isStarted = false;
        PlayTimes = 1;
    }

    private void Start()
    {
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

                case 4:
                    _levelText.text = "Normal";
                    break;

                case 5:
                    _levelText.text = "Happy";
                    break;
            }

            level = (int)v;
            _levelImage.sprite = Resources.Load<Sprite>($"Arts/{_levelText.text}");
        });
    }

    private void OnEnable()
    {
        string levelStr = "";

        switch (level)
        {
            case 1:
                _slider.value = 1;
                levelStr = "Furious";
                break;

            case 2:
                _slider.value = 2;
                levelStr = "Angry";
                break;

            case 3:
                _slider.value = 3;
                levelStr = "Annoy";
                break;
        }
        
        _levelImage.sprite = Resources.Load<Sprite>($"Arts/{levelStr}");
        _levelText.text = levelStr;
    }

    public void ResetGame()
    {
        PlayTimes++;

        counterText.text = "Smash the ice\nto rescue the animals!";
        counterText.fontSize = 76;

        count = 0;
        state = 1;
        threshold = maxCount / 3;

        progressBar.gameObject.SetActive(false);
        progressBar.GetChild(0).localScale = new Vector2(0, 1);

        buttonPanel.localScale = Vector2.zero;

        isStarted = false;

        // Change image back to Ice
        ice.gameObject.SetActive(true);
        ice.GetComponent<CanvasGroup>().alpha = 1;
        ice.GetComponent<Image>().sprite = Resources.Load<Sprite>("Arts/ice1");
        ice.GetComponent<Button>().enabled = true;

        imageNumber = Random.Range(1, 4);
        animal.gameObject.SetActive(false);
        animal.localScale = new Vector2(0.2f, 0.2f);
        animal.GetComponent<CanvasGroup>().alpha = 0;
        animal.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Ice/{imageNumber}");
    }

    public void OnSmash()
    {
        count++;

        if (!isStarted)
        {
            isStarted = true;
            progressBar.gameObject.SetActive(true);
            counterText.text = "Tap tap tap!";
        }

        progressBar.GetChild(0).LeanScaleX(progressBar.GetChild(0).localScale.x + barProgress, 0);

        if (count >= maxCount)
        {
            OnFinished();
            return;
        }
        else if (count >= threshold)
        {
            state++;
            threshold = maxCount / 3 * state;

            ice.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/ice{state}");

            if (state == 3)
            {
                counterText.text = "Almost there!";
            }
            else
            {
                counterText.fontSize += 16;
            }
        }

        // Shaking animation
        ice.LeanMoveLocalX(startPosition - distance, delay);
        ice.LeanMoveLocalX(startPosition + distance, delay/2).delay = delay;
        ice.LeanMoveLocalX(startPosition, delay).delay = delay + (delay/2) ;
    }

    private void OnFinished()
    {
        ice.GetComponent<Button>().enabled = false;

        // Hide progress bar
        progressBar.gameObject.SetActive(false);

        // Hide
        //counterText.transform.LeanScale(new Vector2(0.5f, 0.5f), 1f);
        //counterText.GetComponent<CanvasGroup>().LeanAlpha(0, 1f);
        counterText.gameObject.SetActive(false);
        ice.GetComponent<CanvasGroup>().LeanAlpha(0, 1f);

        animal.gameObject.SetActive(true);
        animal.GetComponent<CanvasGroup>().LeanAlpha(1, 1.2f).setOnComplete(ShowAnimal);
    }

    private void ShowAnimal()
    {
        ice.gameObject.SetActive(false);

        animal.LeanScale(Vector2.one, 1.5f).setEaseOutBack().setOnComplete(Save);
        buttonPanel.LeanScale(Vector2.one, 1f).setEaseOutExpo().delay = 1f;

        Hooray();
    }

    private void UpdateGallery()
    {
        // Read ice_data again
        if (GameFileManager.instance.IceData.img1)
        {
            figures[0].sprite = Resources.Load<Sprite>($"Ice/1");
            figures[0].GetComponent<RectTransform>().sizeDelta = new Vector2(240, 240);
        }
        
        if (GameFileManager.instance.IceData.img2)
        {
            figures[1].sprite = Resources.Load<Sprite>($"Ice/2");
            figures[1].GetComponent<RectTransform>().sizeDelta = new Vector2(240, 240);
        }

        if (GameFileManager.instance.IceData.img3)
        {
            figures[2].sprite = Resources.Load<Sprite>($"Ice/3");
            figures[2].GetComponent<RectTransform>().sizeDelta = new Vector2(240, 240);
        }
    }

    private void Save()
    {
        // Decide to check emotion
        if (level != -1)
        {
            ShowEmotionCheck();
        }
        else
        {
            // Save only total game count
            GameFileManager.instance.SaveGame();
        }

        // Save play data
        GameFileManager.instance.SaveIce(imageNumber);
        UpdateGallery();

    }

    private void Hooray()
    {
        LeanTween.cancel(counterText.gameObject);

        counterText.transform.localScale = Vector2.zero;
        counterText.fontSize += 16;
        counterText.text = "Hooray!";

        counterText.gameObject.SetActive(true);
        counterText.GetComponent<CanvasGroup>().LeanAlpha(1, 1f);
        counterText.transform.LeanScale(Vector2.one, 1f).setEaseOutExpo();
    }

    public void ShowEmotionCheck()
    {
        checkEmotion.SetActive(true);
    }
}

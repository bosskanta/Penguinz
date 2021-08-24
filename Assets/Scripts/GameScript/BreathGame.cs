using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BreathGame : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Image _levelImage;
    [SerializeField] private TMP_Text _levelText;

    public static BreathGame instance;
    public TMP_Text guideText, timeText, cycleText;
    public Image circle, toggle;
    public GameObject buttonPanel, checkEmotion;
    public CanvasGroup[] dots;
    public Animator colorChanger, smile;

    private bool isStart, changeText, isChangingState;
    private float nextTick, nextDotTime, countFactor;
    private int displayTime, state, dotState, cycle, maxCycle, dotIndex;
    private int level;

    private float startScale = 0.45f;
    public int PlayTimes { get; set; }

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

        PlayTimes = 1;
        isStart = false;
        changeText = false;
        isChangingState = false;

        circle.gameObject.SetActive(false);
        timeText.gameObject.SetActive(false);
        toggle.gameObject.SetActive(false);
        cycleText.gameObject.SetActive(false);
        buttonPanel.gameObject.SetActive(false);
        buttonPanel.transform.localScale = Vector2.zero;

        countFactor = 1.25f;

        state = 0;
        cycle = 1;
        dotIndex = 0;

        maxCycle = GetMaxCycle();        
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

    private int GetMaxCycle()
    {
        if (NoteManager.instance.Level != -1)
        {
            level = NoteManager.instance.Level;
            GameFileManager.instance.Level = level;
        }

        int maxCycle = 3;

        if (level == 1)
        {
            maxCycle = 5;
        }
        else if (level == 2)
        {
            maxCycle = 4;
        }

        maxCycle = 1;
        //return maxCycle;
        return maxCycle;
    }

    public void Toggle()
    {
        if (isStart)
        {
            isStart = false;
            toggle.sprite = Resources.Load<Sprite>($"Icons/play");

            LeanTween.pause(circle.gameObject);
            Time.timeScale = 0;
        }
        else
        {
            isStart = true;
            toggle.sprite = Resources.Load<Sprite>($"Icons/pause");

            LeanTween.resume(circle.gameObject);
            Time.timeScale = 1;
        }
    }

    public void StartGame()
    {
        isStart = true;
        GameObject.Find("Canvas").GetComponent<Button>().enabled = false;

        nextTick = Time.time;
        nextDotTime = nextTick;
        displayTime = 4;

        // Animate scale blow up
        circle.gameObject.SetActive(true);
        circle.transform.LeanScale(Vector2.one, 5f);

        timeText.text = "";
        timeText.gameObject.SetActive(true);
        toggle.gameObject.SetActive(true);
        cycleText.gameObject.SetActive(true);
        cycleText.text = $"CYCLE {cycle} OF {maxCycle}";

        guideText.text = "INHALE";
        guideText.fontSize = guideText.fontSize + 64;
    }

    public void ResetGame()
    {
        PlayTimes++;

        state = 0;
        cycle = 1;
        dotIndex = 0;
        dotState = 0;

        buttonPanel.gameObject.SetActive(false);
        buttonPanel.transform.localScale = Vector2.zero;

        guideText.text = "TOUCH TO START";
        guideText.fontSize = 76;

        foreach (CanvasGroup cg in dots)
        {
            cg.gameObject.SetActive(true);
        }

        smile.SetTrigger("Back");
        circle.gameObject.SetActive(false);
        circle.transform.localScale = new Vector2(startScale, startScale);

        timeText.transform.localScale = Vector2.one;
        toggle.transform.localScale = Vector2.one;
        cycleText.transform.localScale = Vector2.one;

        timeText.gameObject.SetActive(false);
        toggle.gameObject.SetActive(false);
        cycleText.gameObject.SetActive(false);

        GameObject.Find("Canvas").GetComponent<Button>().enabled = true;
    }

    public void Update()
    {
        if (isStart)
        {
            if (Time.time >= nextDotTime)
            {
                if (dotIndex < 16 && dotState == 0)
                {
                    nextDotTime = Time.time + 0.3125f;
                    dots[dotIndex].LeanAlpha(1, 0.3125f);
                    dotIndex++;
                }
                else if (dotIndex >= 0 && dotState == 2)
                {
                    nextDotTime = Time.time + 0.625f;
                    dots[dotIndex].LeanAlpha(0, 0.625f);
                    dotIndex--;
                }
            }
        }

        if (!isChangingState && isStart)
        {
            if (displayTime > 0 && Time.time >= nextTick)
            {
                nextTick = Time.time + countFactor;
                DisplayTime(displayTime);
                displayTime -= 1;
            }
            else if (displayTime <= 0)
            {
                isChangingState = true;
                Time.timeScale = 0;

                nextTick = Time.time + countFactor;

                changeText = true;

                // Change state
                state++;
                state = state % 3;

                if (state == 2)
                {
                    displayTime = 8;
                }
                else if (state == 0)
                {
                    displayTime = 4;
                    cycle++;
                }
                else if (state == 1)
                {
                    displayTime = 4;
                }

                Time.timeScale = 1;
                isChangingState = false;
            }
        }
    }

    private void DisplayTime(int timeToDisplay)
    {
        if (changeText)
        {
            changeText = false;

            dotState++;
            dotState = dotState % 3;

            dotIndex = dotState == 2 ? 15 : 0;

            // Exhale
            if (state == 2)
            {
                // Animate scale down
                circle.transform.LeanScale(new Vector2(startScale, startScale), 10f);
                guideText.text = "EXHALE";
            }
            else if (state == 0)
            {
                if (cycle > maxCycle)
                {
                    OnFinished();
                    return;
                }

                FlickCycleText();

                // Animate scale blow up
                circle.transform.LeanScale(Vector2.one, 5f);

                // Update cycle text
                cycleText.text = $"CYCLE {cycle} OF {maxCycle}";
                guideText.text = "INHALE";
            }
            else if (state == 1)
            {
                guideText.text = "HOLD";
            }

            // ChangeColor
            colorChanger.SetTrigger("ChangeState");
        }
        
        timeText.text = string.Format("{0}", timeToDisplay);
    }

    private void OnFinished()
    {
        foreach (CanvasGroup cg in dots)
        {
            cg.gameObject.SetActive(false);
            cg.alpha = 0;
        }

        isStart = false;

        float delay = 1f;

        // Animation
        smile.SetTrigger("Smile");
        guideText.transform.localScale = Vector2.zero;
        guideText.text = "Hooray!";
        guideText.transform.LeanScale(Vector2.one, delay).setEaseOutBack();

        circle.transform.localPosition = Vector2.zero;
        circle.transform.LeanScale(new Vector2(5f, 5f), 1.5f).setEaseOutBack().setOnComplete(Save);

        timeText.transform.LeanScale(Vector2.zero, 0.3f).setEaseInExpo();
        toggle.transform.LeanScale(Vector2.zero, 0.3f).setEaseInExpo();
        cycleText.transform.LeanScale(Vector2.zero, 0.3f).setEaseInExpo();
    }

    private void Save()
    {
        if (level != -1)
        {
            ShowEmotionCheck();
        }
        else
        {
            // Save only total game count
            GameFileManager.instance.SaveGame();
        }

        buttonPanel.gameObject.SetActive(true);
        buttonPanel.transform.LeanScale(Vector2.one, 0.5f).setEaseOutBack();
    }

    public void ShowEmotionCheck()
    {
        checkEmotion.SetActive(true);
    }

    private void FlickCycleText()
    {
        cycleText.color = new Color32(40, 181, 181, 255);
        cycleText.transform.LeanScale(Vector2.one, 0).setOnComplete(FlickBack).delay = 0.6f;
    }

    private void FlickBack()
    {
        cycleText.color = MyColors.White;
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : MonoBehaviour
{
    [SerializeField] private Slider _sliderAfter;
    [SerializeField] private Image _levelImageAfter;
    [SerializeField] private TMP_Text _levelTextAfter;

    [SerializeField] private Slider _slider;
    [SerializeField] private Image _levelImage;
    [SerializeField] private TMP_Text _levelText;

    public static NoteManager instance;

    public GameObject textInput;
    public TMP_Text whatLevelTMP, recordStatus, defaultDate;
    public GameObject changeableDate;
    public Button recordButton;
    public Vector2 shakeXY;
    public float delay;

    private string totalFile, dayFile, noteFile;
    private Vector2 textPosition;
    /*
     * Work with JSON
     */
    private TotalLevel totalLevel;
    private List<Day> dayList;
    private List<Note> noteList = null;

    // Report Manager
    public TotalLevel GetTotalLevel()
    {
        return totalLevel;
    }

    public List<Day> GetMostNoteDay()
    {
        int max = dayList.Max(d => d.countWeekly);

        if (max == 0)
        {
            return new List<Day>();
        }

        List<Day> mostDayList = dayList.FindAll(d => d.countWeekly == max);
        return mostDayList;
    }

    // Game File Manager
    public int GetLatestLevelBefore()
    {
        return noteList[noteList.Count - 1].levelBefore;
    }

    // Calendar Manager
    public List<Note> GetNotes(int year, int month)
    {
        List<Note> find = noteList.FindAll(n => n.month == month && n.year == year);

        find = find.OrderBy(n => n.year)
                .ThenBy(n => n.month)
                .ToList();

        return find;
    }

    public string Id { get; set; }
    public int Level { get; set; }
    public string Text { get; set; }
    public bool IsTakingNoteBefore { get; set; }

    private void OnEnable()
    {
        Level = 1;
    }

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

        defaultDate.gameObject.SetActive(false);
        changeableDate.gameObject.SetActive(false);
        changeableDate.transform.GetChild(3).gameObject.SetActive(false);

        textPosition = whatLevelTMP.rectTransform.localPosition;

        // Rest note panel info
        totalFile = Path.Combine(Application.persistentDataPath, "note_count.json");
        dayFile = Path.Combine(Application.persistentDataPath, "day_count.json");
        noteFile = Path.Combine(Application.persistentDataPath, "notes.json");

        ResetNote();

        IsTakingNoteBefore = false;
    }

    private void Start()
    {
        _slider.onValueChanged.AddListener((v) => {
            switch(v)
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

            Level = (int)v;
            _levelImage.sprite = Resources.Load<Sprite>($"Arts/{_levelText.text}");
        });

        _sliderAfter.onValueChanged.AddListener((v) => {
            switch (v)
            {
                case 1:
                    _levelTextAfter.text = "Furious";
                    break;

                case 2:
                    _levelTextAfter.text = "Angry";
                    break;

                case 3:
                    _levelTextAfter.text = "Annoy";
                    break;

                case 4:
                    _levelTextAfter.text = "Normal";
                    break;

                case 5:
                    _levelTextAfter.text = "Happy";
                    break;
            }

            _levelImageAfter.sprite = Resources.Load<Sprite>($"Arts/{_levelTextAfter.text}");
        });
    }

    // This function save day_stats.json and total_level.json
    private void SaveStats(bool inWeek, int dayIndex)
    {
        switch (Level)
        {
            case 3:
                dayList[dayIndex].annoy++;
                totalLevel.totalAnnoy++;

                if (inWeek)
                {
                    totalLevel.weeklyAnnoy++;
                }

                break;

            case 2:
                dayList[dayIndex].angry++;
                totalLevel.totalAngry++;
                   
                if (inWeek)
                {
                    totalLevel.weeklyAngry++;
                }

                break;

            case 1:
                dayList[dayIndex].furious++;
                totalLevel.totalFurious++;

                if (inWeek)
                {
                    totalLevel.weeklyFurious++;
                }

                break;
        }

        if (inWeek)
        {
            dayList[dayIndex].countWeekly++;
        }
    }

    public void SaveLevelAfter()
    {
        noteList[noteList.Count - 1].levelAfter = (int)_sliderAfter.value;

        NoteCollection collection = new NoteCollection
        {
            notes = noteList.ToArray()
        };
        string jsonString = JsonUtility.ToJson(collection, true);
        File.WriteAllText(noteFile, jsonString, System.Text.Encoding.UTF8);
    }

    public bool SaveNote()
    {
        //if (Level.Equals(""))
        //{
        //    whatLevelTMP.text = "Please choose anger level.";
        //    whatLevelTMP.color = MyColors.Furious;
        //    return false;
        //}

        // Save
        Text = textInput.GetComponent<TMP_InputField>().text;

        Note note;
        DateTime saveDate;

        // Today date note
        if (defaultDate.IsActive())
        {
            bool hasAudio = false;

            if (IsAudioFileExist(Id) != "NOT FOUND")
            {
                hasAudio = true;
            }

            note = new Note
            {
                id = Id,
                levelBefore = (int)_slider.value,
                levelAfter = -1,
                text = Text,
                day = DateTime.Now.Day,
                month = DateTime.Now.Month,
                year = DateTime.Now.Year,
                hour = DateTime.Now.Hour,
                minute = DateTime.Now.Minute,
                hasAudio = hasAudio
            };

            noteList.Add(note);

            int dayIndex = dayList.FindIndex(e => e.day.Equals(DateTime.Now.DayOfWeek.ToString()));
            SaveStats(true, dayIndex);
        }
        else
        {
            saveDate = NoteValueHolder.instance.SelectedDate;
            string hours = changeableDate.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            string minutes = changeableDate.transform.GetChild(2).GetComponent<TMP_InputField>().text;

            // Empty hours or minutes
            if (hours.Equals("") || minutes.Equals(""))
            {
                changeableDate.transform.GetChild(3).gameObject.SetActive(true);
                changeableDate.transform.GetChild(3).GetComponent<TMP_Text>().text = "REQUIRED";
                return false;
            }
            // Invalid bound of hours or minutes
            else if (Int32.Parse(hours) > 23 || Int32.Parse(minutes) > 59)
            {
                changeableDate.transform.GetChild(3).gameObject.SetActive(true);
                changeableDate.transform.GetChild(3).GetComponent<TMP_Text>().text = "Must be in (00:00 - 23:59)";
                changeableDate.transform.GetChild(1).GetComponent<TMP_InputField>().text = "";
                changeableDate.transform.GetChild(2).GetComponent<TMP_InputField>().text = "";
                return false;
            }
            // Save note, selected date
            else
            {
                // Today
                // Hours or minutes is not coming
                if (NoteValueHolder.instance.SelectedDate == DateTime.Now.Date)
                {
                    int h = Int32.Parse(hours);
                    int m = Int32.Parse(minutes);

                    if (h > DateTime.Now.Hour || m > DateTime.Now.Minute)
                    {
                        changeableDate.transform.GetChild(3).gameObject.SetActive(true);
                        changeableDate.transform.GetChild(3).GetComponent<TMP_Text>().text = "The time is yet to come";
                        
                        return false;
                    }
                }
                
                bool hasAudio = false;

                if (IsAudioFileExist(Id) != "NOT FOUND")
                {
                    hasAudio = true;
                }

                note = new Note
                {
                    id = Id,
                    levelBefore = (int)_slider.value,
                    levelAfter = -1,
                    text = Text,
                    day = saveDate.Day,
                    month = saveDate.Month,
                    year = saveDate.Year,
                    hour = Int32.Parse(hours),
                    minute = Int32.Parse(minutes),
                    hasAudio = hasAudio
                };

                noteList.Add(note);

                // Check if saveDate is in week or not.
                bool inWeek = false;

                if (saveDate.Month == DateTime.Now.Month && saveDate.Year == DateTime.Now.Year)
                {
                    // If saveDate is today.
                    if (saveDate.Day == DateTime.Now.Day)
                    {
                        inWeek = true;
                    }
                    // Else, check if saveDate is in the same week as today
                    else
                    {
                        int sundayOfSaveDate = saveDate.Day - (int)saveDate.DayOfWeek;

                        int sundayOfToday = DateTime.Now.Day - (int)DateTime.Now.DayOfWeek;

                        if (sundayOfSaveDate == sundayOfToday)
                        {
                            inWeek = true;
                        }
                    }
                }

                int dayIndex = dayList.FindIndex(e => e.day.Equals(saveDate.DayOfWeek.ToString()));

                if (inWeek)
                {
                    SaveStats(true, dayIndex);
                }
                else
                {
                    SaveStats(false, dayIndex);
                }
            }
        }

        print("Saved note id : " + Id + ", " + Level);
        WriteFile();

        IsTakingNoteBefore = true;

        // Set slider after
        _sliderAfter.value = Level;

        string levelStr = "";

        switch (Level)
        {
            case 1:
                levelStr = "Furious";
                break;

            case 2:
                levelStr = "Angry";
                break;

            case 3:
                levelStr = "Annoy";
                break;
        }

        _levelImageAfter.sprite = Resources.Load<Sprite>($"Arts/{levelStr}");
        _levelTextAfter.text = levelStr;

        return true;
    }

    public void ResetWeekly()
    {
        totalLevel.weeklyAnnoy = 0;
        totalLevel.weeklyAngry = 0;
        totalLevel.weeklyFurious = 0;

        foreach (Day day in dayList)
        {
            day.countWeekly = 0;
        }

        WriteFile();
    }

    public void ResetRecording()
    {
        // Stop mic recording
        if (Microphone.IsRecording(Microphone.devices[0]))
        {
            Microphone.End(Microphone.devices[0]);
        }

        recordButton.enabled = true;
        recordButton.image.sprite = Resources.Load<Sprite>("Icons/active_microphone");
        recordStatus.text = "Touch to record";
    }

    public void ResetNote()
    {
        // Update note state from file
        ReadFile();
        ResetRecording();

        // Reset current note state
        if (noteList.Count > 0)
        {
            int id = Convert.ToInt32(noteList[noteList.Count - 1].id) + 1;
            Id = id.ToString();
        }
        else // This is gonna be the the first note
        {
            Id = "1";
        }

        _slider.value = 1;
        Level = 1;
        Text = "";

        if (!IsAudioFileExist(Id).Equals("NOT FOUND"))
        {
            DeleteAudio(Id);
        }

        // Reset colors, texts and record button sprite
        whatLevelTMP.text = "How is your anger?";

        defaultDate.gameObject.SetActive(false);

        changeableDate.transform.GetChild(1).GetComponent<TMP_InputField>().text = "";
        changeableDate.transform.GetChild(2).GetComponent<TMP_InputField>().text = "";
        changeableDate.gameObject.SetActive(false);

        textInput.GetComponent<TMP_InputField>().text = "";
    }

    public void DeleteAudio(string id)
    {
        /*
         * If audio file of the current note ID is EXIST!
         */
        string filePath = IsAudioFileExist(id);
        if (filePath != "NOT FOUND")
        {
            File.Delete(filePath);
            print("Deleted audio : " + id + ".wav");
        }
    }

    public string IsAudioFileExist(string id)
    {
        //print("Check audio file existense");
        string[] files = Directory.GetFiles(Path.Combine(Application.persistentDataPath, "audio"));

        if (files != null)
        {
            foreach (string filePath in files)
            {
                string name = Path.GetFileName(filePath);
                name = name.Replace(".wav", "");
                if (id.Equals(name))
                {
                    return filePath;
                }
            }
        }

        return "NOT FOUND";
    }

    private void WriteFile()
    {
        // Write to "note_stats.json"
        NoteCollection collection = new NoteCollection
        {
            notes = noteList.ToArray()
        };
        string jsonString = JsonUtility.ToJson(collection, true);
        File.WriteAllText(noteFile, jsonString, System.Text.Encoding.UTF8);

        // Write to "day_stats.json"
        DayCollection week = new DayCollection
        {
            week = dayList.ToArray()
        };
        jsonString = JsonUtility.ToJson(week, true);
        File.WriteAllText(dayFile, jsonString, System.Text.Encoding.UTF8);

        // Write to "total_level.json"
        jsonString = JsonUtility.ToJson(totalLevel, true);
        File.WriteAllText(totalFile, jsonString, System.Text.Encoding.UTF8);
    }

    private void ReadFile()
    {
        if (File.Exists(noteFile))
        {
            string noteContent = File.ReadAllText(noteFile);

            if (noteContent.Equals(""))
            {
                noteList = new List<Note>();
            }
            else
            {
                NoteCollection noteCollection = JsonUtility.FromJson<NoteCollection>(noteContent);
                noteList = new List<Note>(noteCollection.notes);
            }
        }
        else
        {
            File.Create(noteFile).Close();
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "audio"));
            ReadFile();
        }
    }

    // Set totalLevel and dayList
    // Invoke by report manager
    public bool ReadFileReport()
    {
        if (File.Exists(totalFile))
        {
            string totalContent = File.ReadAllText(totalFile);
            
            // File is blank.
            if (totalContent.Equals(""))
            {
                totalLevel = new TotalLevel() { totalAnnoy = 0, totalAngry = 0, totalFurious = 0 };

                dayList = new List<Day>
                {
                    new Day { day = "Sunday" },
                    new Day { day = "Monday" },
                    new Day { day = "Tuesday" },
                    new Day { day = "Wednesday" },
                    new Day { day = "Thursday" },
                    new Day { day = "Friday" },
                    new Day { day = "Saturday" }
                };
            }
            // File have content.
            else
            {
                totalLevel = JsonUtility.FromJson<TotalLevel>(totalContent);

                string dayContent = File.ReadAllText(dayFile);
                DayCollection dayCollection = JsonUtility.FromJson<DayCollection>(dayContent);
                dayList = new List<Day>(dayCollection.week);
            }

            return true;
        }
        else
        {
            File.Create(totalFile).Close();
            File.Create(dayFile).Close();
            return false;
        }
    }

    public void ShakeText()
    {
        HorizontalShake();
    }

    private void HorizontalShake()
    {
        LeanTween.moveLocalX(whatLevelTMP.gameObject, textPosition.x + shakeXY.x, delay).setOnComplete(DefaultPosition);
    }

    private void DefaultPosition()
    {
        LeanTween.moveLocal(whatLevelTMP.gameObject, textPosition, delay);
    }

    public void DeleteNote(string id)
    {
        // Delete Audio of that id
        DeleteAudio(id);

        // Remove that note from noteList
        Note toDelete = noteList.Find(n => n.id == id);
        noteList.Remove(toDelete);

        // Check if inweek delete weekly data
        bool inWeek = false;

        if (toDelete.month == DateTime.Now.Month && toDelete.year == DateTime.Now.Year)
        {
            // If saveDate is today.
            if (toDelete.day == DateTime.Now.Day)
            {
                inWeek = true;
            }
            // Else, check if saveDate is in the last week
            else
            {
                DateTime deleteDate = new DateTime(toDelete.year, toDelete.month, toDelete.day);
                int sundayOfSaveDate = toDelete.day - (int)deleteDate.DayOfWeek;

                int sundayOfToday = DateTime.Now.Day - (int)DateTime.Now.DayOfWeek;

                if (sundayOfSaveDate == sundayOfToday)
                {
                    inWeek = true;
                }
            }
        }

        // Decrease the number of that level from dayList, totalLevel
        int dayIndex = dayList.FindIndex(d => d.day.Equals(NoteValueHolder.instance.DayOfWeek));

        switch (toDelete.levelBefore)
        {
            case 3:
                dayList[dayIndex].annoy--;
                totalLevel.totalAnnoy--;
                if (inWeek)
                {
                    totalLevel.weeklyAnnoy--;
                    dayList[dayIndex].countWeekly--;
                }
                break;

            case 2:
                dayList[dayIndex].angry--;
                totalLevel.totalAngry--;
                if (inWeek)
                {
                    totalLevel.weeklyAngry--;
                    dayList[dayIndex].countWeekly--;
                }
                break;

            case 1:
                dayList[dayIndex].furious--;
                totalLevel.totalFurious--;
                if (inWeek)
                {
                    totalLevel.weeklyFurious--;
                    dayList[dayIndex].countWeekly--;
                }
                break;
        }

        WriteFile();
    }

    public void EditNoteText(string id, string text)
    {
        noteList[noteList.FindIndex(n => n.id == id)].text = text;

        WriteFile();
    }

    public void SetShownDate()
    {
        defaultDate.gameObject.SetActive(true);
        defaultDate.text = $"{DateTime.Now.ToString("d MMM yyyy\nHH:mm")}";
    }

    public void SetChangeAbleDate()
    {
        changeableDate.SetActive(true);
        changeableDate.transform.GetChild(0).GetComponent<TMP_Text>().text = NoteValueHolder.instance.SelectedDate.ToString("d MMM yyyy");
        
        if (NoteValueHolder.instance.SelectedDate == DateTime.Now.Date)
        {
            changeableDate.transform.GetChild(1).GetComponent<TMP_InputField>().text = DateTime.Now.ToString("HH");
            changeableDate.transform.GetChild(2).GetComponent<TMP_InputField>().text = DateTime.Now.ToString("mm");
        }
    }
}

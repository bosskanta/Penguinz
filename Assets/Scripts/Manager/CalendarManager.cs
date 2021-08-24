using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class CalendarManager : MonoBehaviour
{
    public class CalendarDay
    {
        public int dayNum;
        public bool haveNote;
        public bool haveAnnoy, haveAngry, haveFurious;
        public GameObject obj;

        // Specify which day on month via constructor
        public CalendarDay(int dayNum, GameObject obj)
        {
            this.dayNum = dayNum;
            this.obj = obj;
            UpdateDay(dayNum);
        }

        public void UpdateTextColor(Color32 color)
        {
            obj.transform.GetChild(2).GetComponent<TMP_Text>().color = color;
        }

        public void UpdateDay(int dayNum)
        {
            this.dayNum = dayNum;
            obj.transform.GetChild(2).GetComponent<TMP_Text>().text = dayNum.ToString();
        }

        // To specify which day have note
        public void SetNoteFlag(bool[] noteStatus)
        {
            haveAnnoy = noteStatus[0];
            haveAngry = noteStatus[1];
            haveFurious = noteStatus[2];

            // Set highlight to show that have note
            if (haveAnnoy || haveAngry || haveFurious)
            {
                haveNote = true;
            }
            // Hightlight off
            else
            {
                haveNote = false;
            }
        }

        public void SetCurrentDay()
        {
            obj.transform.GetChild(1).GetComponent<Image>().color = MyColors.SelectedDay;
            obj.transform.GetChild(2).GetComponent<TMP_Text>().color = MyColors.Black;
        }

        // This called when dropdown selected
        public void SetHighlightColor(string level)
        {
            /*************************
             * Level can be :
             * 
             * "Annoy"
             * "Angry"
             * "Furious"
             * "All"
             * 
             * From dropdown
             * 
             *************************/
            if (haveNote)
            {
                obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.ZeroAlpha;
                obj.transform.GetChild(1).GetComponent<Image>().color = MyColors.CalendarBG;

                if (level.Equals("All"))
                {
                    if (haveAnnoy && haveAngry && haveFurious)
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/All");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.White;
                    }
                    else if (haveAnnoy && haveAngry && !haveFurious) 
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/AnnoyAngry");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.White;
                    }
                    else if (haveAnnoy && !haveAngry && haveFurious)
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/AnnoyFurious");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.White;
                    }
                    else if (!haveAnnoy && haveAngry && haveFurious)
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/AngryFurious");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.White;
                    }
                    else if (haveAnnoy)
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Circle");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Annoy;
                    }
                    else if (haveAngry)
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Circle");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Angry;
                    }
                    else
                    {
                        obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Circle");
                        obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Furious;
                    }
                }
                else
                {
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Circle");
                    switch (level)
                    {
                        case "Annoy":
                            if (haveAnnoy)
                            {
                                obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Annoy;
                            }
                            break;

                        case "Angry":
                            if (haveAngry)
                            {
                                obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Angry;
                            }
                            break;

                        case "Furious":
                            if (haveFurious)
                            {
                                obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.Furious;
                            }
                            break;
                    }
                }
            }
            else
            {
                obj.transform.GetChild(1).GetComponent<Image>().color = MyColors.ZeroAlpha;
                obj.transform.GetChild(0).GetComponent<Image>().color = MyColors.ZeroAlpha;
            }
        }
    }

    public static CalendarManager instance;
    public Transform[] weeks;
    public TMP_Text MonthAndYear;
    public TMP_Text[] countsTMPText;
    public DateTime calendarDate;
    public TMP_Dropdown dropdown;

    private readonly string[] LEVELS = { "Annoy", "Angry", "Furious", "All" };

    private bool isToastActive = false;
    private List<CalendarDay> dayList;
    private List<Note> noteList;
    private List<Game> gameList;
    private readonly int[] levelsCount = new int[3];
    private string filterLevel;

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

        filterLevel = LEVELS[3];
    }

    private void Start()
    {
        dayList = new List<CalendarDay>();

        calendarDate = DateTime.Now;
        RefreshCalendar();
    }

    public void HandleInputData(int val)
    {
        filterLevel = LEVELS[(val + 3) % 4];
        //print("Calendar filter level : " + filterLevel);
        RefreshCalendar();
    }

    public bool IsToastActive()
    {
        return isToastActive;
    }
    public void SetToastActive(bool status)
    {
        isToastActive = status;
    }

    public void TodayClick()
    {
        calendarDate = DateTime.Now;
        RefreshCalendar();
    }

    private void UpdateCalendar(int year, int month)
    {
        SetData(year, month);

        DateTime firstDateOfMonth = new DateTime(year, month, 1);
        MonthAndYear.text = firstDateOfMonth.ToString("MMM yyyy");

        // +1 / -1 to change month
        calendarDate = firstDateOfMonth;

        DateTime lastMonth = firstDateOfMonth.AddMonths(-1);
        int lastMonthEndDay = DateTime.DaysInMonth(lastMonth.Year, lastMonth.Month);

        // Start day in week, 0 is Sunday - 6 is Saturday.
        int startDayInWeek = (int)firstDateOfMonth.DayOfWeek;
        int endDay = DateTime.DaysInMonth(year, month);
        int nextMonthDay = 1;

        // dayList just start declared
        if (dayList.Count == 0)
        {
            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    CalendarDay newDay;
                    int currDay = (w * 7) + i;

                    // Head
                    if (currDay < startDayInWeek)
                    {
                        currDay = lastMonthEndDay - startDayInWeek + currDay + 1;
                        newDay = new CalendarDay(currDay, weeks[w].GetChild(i).gameObject);
                        newDay.UpdateTextColor(MyColors.WhiteAlpha);
                    }
                    // Tail
                    else if (currDay - startDayInWeek >= endDay)
                    {
                        newDay = new CalendarDay(nextMonthDay, weeks[w].GetChild(i).gameObject);
                        newDay.UpdateTextColor(MyColors.WhiteAlpha);
                        nextMonthDay++;
                    }
                    // On month
                    else
                    {
                        int dayNum = (currDay - startDayInWeek) + 1;

                        bool[] noteStatus = GetNoteStatus(year, month, dayNum);

                        newDay = new CalendarDay(dayNum, weeks[w].GetChild(i).gameObject);
                        newDay.UpdateTextColor(MyColors.White);
                        newDay.SetNoteFlag(noteStatus);
                    }

                    dayList.Add(newDay);
                }
            }
        }
        // dayList has been declare, changing month
        else
        {
            for (int i = 0; i < 42; i++)
            {
                // Note on this month
                if (i < startDayInWeek)
                {
                    int dayNum = lastMonthEndDay - startDayInWeek + i + 1;
                    dayList[i].UpdateDay(dayNum);
                    dayList[i].UpdateTextColor(MyColors.WhiteAlpha);
                    dayList[i].SetNoteFlag(new bool[3] { false, false, false });
                }
                else if (i - startDayInWeek >= endDay)
                {
                    dayList[i].UpdateDay(nextMonthDay);
                    dayList[i].UpdateTextColor(MyColors.WhiteAlpha);
                    dayList[i].SetNoteFlag(new bool[3] { false, false, false });
                    nextMonthDay++;
                }
                // On month
                else
                {
                    int dayNum = (i - startDayInWeek) + 1;
                    bool[] noteStatus = GetNoteStatus(year, month, dayNum);

                    dayList[i].UpdateDay(dayNum);
                    dayList[i].UpdateTextColor(MyColors.White);
                    dayList[i].SetNoteFlag(noteStatus);
                }
            }
        }
    }

    private void UpdateHighlightColor(string level)
    {
        foreach (CalendarDay day in dayList)
        {
            day.SetHighlightColor(level);
        }
    }

    private void SetCountText(bool hasNote)
    {
        for (int i = 0; i < 3; i++)
        {
            countsTMPText[i].gameObject.transform.parent.gameObject.SetActive(false);
        }
        countsTMPText[3].gameObject.SetActive(false);

        if (hasNote)
        {
            for (int i = 0; i < 4; i++)
            {
                if (i != 3 && filterLevel == LEVELS[i] && levelsCount[i] > 0)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        countsTMPText[k].gameObject.transform.parent.gameObject.SetActive(true);
                        countsTMPText[k].text = "0";
                    }
                    countsTMPText[i].text = levelsCount[i].ToString();
                    break;
                }
                else if (i == 3 && filterLevel == LEVELS[i])
                {
                    for (int k = 0; k < 3; k++)
                    {
                        countsTMPText[k].gameObject.transform.parent.gameObject.SetActive(true);
                        countsTMPText[k].text = levelsCount[k].ToString();
                    }
                    break;
                }
                else if (filterLevel == LEVELS[i] && levelsCount[i] == 0)
                {
                    countsTMPText[3].gameObject.SetActive(true);
                    break;
                }
            }
        }
        else
        {
            countsTMPText[3].gameObject.SetActive(true);
        }
    }

    public void SwitchMonth(int direction)
    {
        calendarDate = calendarDate.AddMonths(direction);
        RefreshCalendar();
    }

    private bool[] GetNoteStatus(int year, int month, int dayNum)
    {
        bool haveAnnoy = false, haveAngry = false, haveFurious = false;

        List<Note> currDayNoteList = noteList.FindAll(n =>
                n.day == dayNum && n.month == month && n.year == year
            );

        foreach (Note note in currDayNoteList)
        {
            switch (note.levelBefore)
            {
                case 3:
                    haveAnnoy = true; break;
                case 2:
                    haveAngry = true; break;
                case 1:
                    haveFurious = true; break;
            }
        }

        List<Game> currDayGames = gameList.FindAll(g =>
                g.day == dayNum && g.month == month && g.year == year
            );

        foreach (Game game in currDayGames)
        {
            switch (game.levelBefore)
            {
                case 3:
                    haveAnnoy = true; break;
                case 2:
                    haveAngry = true; break;
                case 1:
                    haveFurious = true; break;
            }
        }

        return new bool[] { haveAnnoy, haveAngry, haveFurious };
    }

    public void SetData(int year, int month)
    {
        // Get this month note data
        noteList = NoteManager.instance.GetNotes(year, month);

        while (noteList == null)
        {
            noteList = NoteManager.instance.GetNotes(year, month);
        }

        // Get this month game data
        // Note the same as noteList
        // noteList filter by GetNotes function
        gameList = GameFileManager.instance.GetGameList();

        while (gameList == null)
        {
            gameList = GameFileManager.instance.GetGameList();
        }
        gameList = gameList.FindAll(g => g.year == year && g.month == month);

        levelsCount[0] = noteList.FindAll(n => n.levelBefore == 3).Count
            + gameList.FindAll(n => n.levelBefore == 3).Count;

        levelsCount[1] = noteList.FindAll(n => n.levelBefore == 2).Count
            + gameList.FindAll(n => n.levelBefore == 2).Count;

        levelsCount[2] = noteList.FindAll(n => n.levelBefore == 1).Count
            + gameList.FindAll(n => n.levelBefore == 1).Count;

        // dropdown - 0 == ALL, 1 == ANNOY, 2 == ANGRY, 3 == FURIOUS
        dropdown.options[0].text = $"All Levels ({levelsCount[0] + levelsCount[1] + levelsCount[2]}x)";
        dropdown.options[1].text = $"Annoy ({levelsCount[0]}x)";
        dropdown.options[2].text = $"Angry ({levelsCount[1]}x)";
        dropdown.options[3].text = $"Furious ({levelsCount[2]}x)";

        dropdown.RefreshShownValue();

        //print($"annoy:{levelsCount[0]}, angry:{levelsCount[1]}, furious:{levelsCount[2]}");

        if (levelsCount[0] + levelsCount[1] + levelsCount[2] > 0)
        {
            SetCountText(true);
        }
        else
        {
            //print($"Note list is null or empty");
            SetCountText(false);
        }
    }

    public void RefreshCalendar()
    {
        UpdateCalendar(calendarDate.Year, calendarDate.Month);
        UpdateHighlightColor(filterLevel);

        if (calendarDate.Month == DateTime.Now.Month && calendarDate.Year == DateTime.Now.Year)
        {
            dayList[dayList.FindIndex(d =>
                    d.dayNum == DateTime.Now.Day && d.obj.transform.GetChild(2).GetComponent<TMP_Text>().color != MyColors.WhiteAlpha)
                ].SetCurrentDay();
        }
    }

    public List<Note> GetThisMonthNotes()
    {
        return noteList;
    }

    public string GetFilterLevel()
    {
        return filterLevel;
    }
}

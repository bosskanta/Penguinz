using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Linq;
using UnityEngine.UI;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    public TMP_Text titleDate, dateInfo;
    public GameObject infoPanel;
    public GameObject[] prefebs;
    public GameObject[] tabIcons;
    public GameObject addButton2;
    public GameObject[] tabCounts;

    private GameObject card;
    private DateTime thisDate;
    private bool isNoteTab = true;
    private List<Note> todayNotes;
    private List<Game> todayGames;

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

        todayNotes = new List<Note>();
        todayGames = new List<Game>();
    }

    public void SetTab(bool isNoteTab)
    {
        this.isNoteTab = isNoteTab;

        if (isNoteTab)
        {
            GameObject.Find("NoteTab").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("NoteTab").GetComponent<Image>().color = new Color32(0, 0, 0, 30);
            GameObject.Find("GameTab").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("GameTab").GetComponent<Image>().color = MyColors.ZeroAlpha;
            // Notes icon
            tabIcons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/notes");
            tabIcons[0].transform.GetChild(1).GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            // Games icon
            tabIcons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/games_inactive");
            tabIcons[1].transform.GetChild(1).GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;

            addButton2.SetActive(true);
        }
        // Clicked game tab
        else
        {
            GameObject.Find("GameTab").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find("GameTab").GetComponent<Image>().color = new Color32(0, 0, 0, 30);
            GameObject.Find("NoteTab").transform.GetChild(0).gameObject.SetActive(false);
            GameObject.Find("NoteTab").GetComponent<Image>().color = MyColors.ZeroAlpha;
            // Games icon
            tabIcons[1].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/games");
            tabIcons[1].transform.GetChild(1).GetComponent<TMP_Text>().fontStyle = FontStyles.Bold;
            // Notes icon
            tabIcons[0].transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Icons/notes_inactive");
            tabIcons[0].transform.GetChild(1).GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;

            addButton2.SetActive(false);
        }

        DestroyContent();

        if (isNoteTab)
        {
            CreateNoteCard();
        }
        else
        {
            CreateGameCard();
        }
    }

    public void RefreshContent()
    {
        SetContent(thisDate.Day.ToString());
    }

    public void SetContent(string dayString)
    {
        DestroyContent();

        int day = int.Parse(dayString);
        UpdateData(day);

        UpdateTabCount();

        // Set title date text
        titleDate.text = $"{thisDate:MMM} {dayString}, {thisDate:yyyy}";
        dateInfo.text = $"{Enum.GetName(typeof(DayOfWeek), thisDate.DayOfWeek).ToUpper()}, {thisDate.ToString("MMM").ToUpper()} {dayString}";

        NoteValueHolder.instance.isDateChangeAble = true;
        NoteValueHolder.instance.SelectedDate = thisDate;

        if (isNoteTab)
        {
            CreateNoteCard();
        }
        else
        {
            CreateGameCard();
        }
    }

    private void UpdateData(int day)
    {
        int month = CalendarManager.instance.calendarDate.Month;
        int year = CalendarManager.instance.calendarDate.Year;

        thisDate = new DateTime(year, month, day);

        string filterLevel = CalendarManager.instance.GetFilterLevel();

        if (filterLevel.Equals("All"))
        {
            todayNotes = CalendarManager.instance.GetThisMonthNotes().FindAll(
                    n => n.month == thisDate.Month && n.day == thisDate.Day && n.year == thisDate.Year
                );
            todayGames = GameFileManager.instance.GetGameList().FindAll(
                    g => g.month == thisDate.Month && g.day == thisDate.Day && g.year == thisDate.Year
                );
        }
        else
        {
            todayNotes = CalendarManager.instance.GetThisMonthNotes().FindAll(
                    n => n.levelBefore.Equals(filterLevel) && n.month == thisDate.Month && n.day == thisDate.Day && n.year == thisDate.Year 
                );
            todayGames = GameFileManager.instance.GetGameList().FindAll(
                    g => g.levelBefore.Equals(filterLevel) && g.month == thisDate.Month && g.day == thisDate.Day && g.year == thisDate.Year
                );
        }

        todayNotes = todayNotes.OrderBy(n => n.hour)
            .ThenBy(n => n.minute)
            .ToList();

        todayGames = todayGames.OrderBy(n => n.hour)
            .ThenBy(n => n.minute)
            .ToList();
    }

    private void CreateNoteCard()
    {
        if (todayNotes.Count == 0)
        {
            card = Instantiate(prefebs[2], infoPanel.transform);
            return;
        }

        foreach (Note n in todayNotes)
        {
            if (n.hasAudio)
            {
                card = Instantiate(prefebs[1], infoPanel.transform);
            }
            else
            {
                card = Instantiate(prefebs[0], infoPanel.transform);
            }

            string hour = n.hour.ToString(), minute = n.minute.ToString();

            if (hour.Length == 1)
            {
                hour = "0" + hour;
            }
            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }

            /*
             * Set name to card object
             */
            card.name = $"{n.levelBefore} {hour}:{minute} {n.id} {Enum.GetName(typeof(DayOfWeek), thisDate.DayOfWeek)}";

            // Set image
            int sprite = Int32.Parse(card.name.Split((' '))[0]);

            switch (sprite)
            {
                case 1:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Furious");
                    break;

                case 2:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Angry");
                    break;

                case 3:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Annoy");
                    break;
            }

            // Set Level text and color
            switch(n.levelBefore)
            {
                case 1:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Furious"; 
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Furious;
                    break;

                case 2:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Angry"; 
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Angry;
                    break;

                case 3:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Annoy"; 
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Annoy;
                    break;
            }

            // Set Timestamp
            card.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = $"{hour}:{minute}";

            // Set Note text
            if (n.text.Equals(""))
            {
                card.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                    .fontStyle = FontStyles.Italic;
                card.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                    .text = $"This note has no note text.";
            }
            else
            {
                card.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                       .fontStyle = FontStyles.Normal;
                card.transform.GetChild(2).gameObject.GetComponent<TMP_Text>()
                       .text = $"{n.text}";
            }
        }
    }

    private void CreateGameCard()
    {
        if (todayGames.Count == 0)
        {
            card = Instantiate(prefebs[2], infoPanel.transform);
            return;
        }

        foreach (Game g in todayGames)
        {
            card = Instantiate(prefebs[3], infoPanel.transform);

            string hour = g.hour.ToString(), minute = g.minute.ToString();

            if (hour.Length == 1)
            {
                hour = "0" + hour;
            }
            if (minute.Length == 1)
            {
                minute = "0" + minute;
            }

            /*
             * Set name to card object
             */
            card.name = $"{g.levelBefore} {hour}:{minute} {g.id} {Enum.GetName(typeof(DayOfWeek), thisDate.DayOfWeek)}";

            // Set image
            int sprite = Int32.Parse(card.name.Split((' '))[0]);

            switch (sprite)
            {
                case 1:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Furious");
                    break;

                case 2:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Angry");
                    break;

                case 3:
                    card.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/Annoy");
                    break;
            }

            if (g.name.Equals("ice"))
            {
                card.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/ice1");
            }
            else
            {
                card.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>($"Arts/breath");
            }

            // Set Level text and color
            switch (g.levelBefore)
            {
                case 1:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Furious";
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Furious;
                    break;

                case 2:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Angry";
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Angry;
                    break;

                case 3:
                    card.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = "Annoy";
                    card.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().color = MyColors.Annoy;
                    break;
            }

            // Set Timestamp
            card.transform.GetChild(1).GetChild(1).GetComponent<TMP_Text>().text = $"{hour}:{minute}";

            // Set text
            card.transform.GetChild(2).GetComponent<TMP_Text>().text = $"Play {g.times} times";

            if (g.levelAfter > g.levelBefore)
            {
                card.transform.GetChild(2).GetComponent<TMP_Text>().text += $", feel better";
            }
        }
    }

    private void UpdateTabCount()
    {
        if (todayNotes.Count == 0)
        {
            tabCounts[0].gameObject.SetActive(false);
        }
        else
        {
            tabCounts[0].gameObject.SetActive(true);
            tabCounts[0].transform.GetChild(0).GetComponent<TMP_Text>().text = todayNotes.Count.ToString();
        }

        if (todayGames.Count == 0)
        {
            tabCounts[1].gameObject.SetActive(false);
        }
        else
        {
            tabCounts[1].gameObject.SetActive(true);
            tabCounts[1].transform.GetChild(0).GetComponent<TMP_Text>().text = todayGames.Count.ToString();
        }
    }

    private void DestroyContent()
    {
        // Destroy everything excepts for dateInfo
        if (infoPanel.transform.childCount > 1)
        {
            for (int i = 1; i < infoPanel.transform.childCount; i++)
            {
                Destroy(infoPanel.transform.GetChild(i).gameObject);
            }
        }
    }
}

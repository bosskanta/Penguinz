using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class ReportManager : MonoBehaviour
{
    public static ReportManager instance;

    public GameObject weeklyReport;
    public TMP_Text noteCount, annoyCount, angryCount, furiousCount;
    public TMP_Text gameCount, iceCount, breathCount;
    public TMP_Text mostDayCount;

    public GameObject mostDayBox, mostDayPrefab;
    private GameObject mostDayInstance;

    private Lastweek lastweek;
    private string checkDateFile, lastweekFile;
    private DateTime todayDate, dateFromFile;
    private string format = "dd-MM-yy";

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

        checkDateFile = Path.Combine(Application.persistentDataPath, "latest_using_date.txt");
        lastweekFile = Path.Combine(Application.persistentDataPath, "lastweek.json");

        lastweek = ReadLastWeekFile();
        SetReportUI();
        dateFromFile = ReadCheckerFile();
        todayDate = DateTime.Now.Date;
    }

    private void Start()
    {
        if (NoteManager.instance.ReadFileReport() == false)
        {
            NoteManager.instance.ReadFileReport();
        }

        CheckReport();
    }

    public Lastweek ReadLastWeekFile()
    {
        if (File.Exists(lastweekFile))
        {
            string jsonContent = File.ReadAllText(lastweekFile);
            if (jsonContent.Equals(""))
            {
                return new Lastweek();
            }
            return JsonUtility.FromJson<Lastweek>(jsonContent);
        }

        File.Create(lastweekFile).Close();
        return new Lastweek();
    }

    private DateTime ReadCheckerFile()
    {
        if (File.Exists(checkDateFile))
        {
            string content = File.ReadAllText(checkDateFile);
            return DateTime.ParseExact(content, format, System.Globalization.CultureInfo.InvariantCulture);
        }

        return DateTime.Now.Date;
    }

    private void CheckReport()
    {
        if (!File.Exists(checkDateFile))
        {
            WriteCheckerFile();
        }

        if (todayDate != dateFromFile) // That's mean first use of today
        {
            print("First use of today, check if need to show report");

            // Check if last week user has used app.
            int sundayOfFile = dateFromFile.Day - (int)dateFromFile.DayOfWeek;

            int sundayOfToday = todayDate.Day - (int)todayDate.DayOfWeek;

            if (sundayOfFile != sundayOfToday)
            {
                ShowReport();
                ResetWeeklyStats();
            }

            WriteCheckerFile();
        }
    }

    private void ResetWeeklyStats()
    {
        GameFileManager.instance.ResetWeekly();
        NoteManager.instance.ResetWeekly();
    }

    private void WriteCheckerFile()
    {
        string str = todayDate.Date.ToString(format);
        File.WriteAllText(checkDateFile, str, System.Text.Encoding.UTF8);
    }

    private void WriteLastWeekFile()
    {
        string jsonString = JsonUtility.ToJson(lastweek, true);
        File.WriteAllText(lastweekFile, jsonString, System.Text.Encoding.UTF8);
    }

    private void ShowReport()
    {
        SetLastWeekData();

        // Write to Lastweek File
        WriteLastWeekFile();

        // Show report panel
        weeklyReport.SetActive(true);
    }

    private void SetLastWeekData()
    {
        // SET NOTE COUNT
        TotalLevel totalLevel = NoteManager.instance.GetTotalLevel();
        int annoy = totalLevel.weeklyAnnoy;
        int angry = totalLevel.weeklyAngry;
        int furious = totalLevel.weeklyFurious;

        // SET GAME DATA
        List<GameTotal> totalGame = GameFileManager.instance.GetTotalList();

        int ice = totalGame.Find(g => g.name == GameFileManager.ICE_GAME).weeklyBetter;
        int breath = totalGame.Find(g => g.name == GameFileManager.BREATH_GAME).weeklyBetter;

        // SET THE MOST TAKE NOTES DAY 
        List<Day> mostDayList = NoteManager.instance.GetMostNoteDay();

        // Set Lastweek
        lastweek.annoy = annoy;
        lastweek.angry = angry;
        lastweek.furious = furious;
        lastweek.totalGames = totalGame[0].weekly + totalGame[1].weekly;
        lastweek.iceBetter = ice;
        lastweek.breathBetter = breath;

        // mostDayList can be everyday or some days.
        for (int i = 0; i < mostDayList.Count; i++)
        {
            lastweek.days[i] = mostDayList[i].day;
        }

        lastweek.mostDayCount = mostDayList.Count != 0 ? mostDayList[0].countWeekly : 0;

        // Set data to report
        SetReportUI();
    }

    private void SetReportUI()
    {
        noteCount.text = $"You have taken {lastweek.annoy + lastweek.angry + lastweek.furious} notes";

        annoyCount.text = lastweek.annoy.ToString();
        angryCount.text = lastweek.angry.ToString();
        furiousCount.text = lastweek.furious.ToString();

        gameCount.text = $"You have played {lastweek.totalGames} games";

        iceCount.text = $"{lastweek.iceBetter} times";
        breathCount.text = $"{lastweek.breathBetter} times";

        // Destroy all childs from mostDayBox
        if (mostDayBox.transform.childCount > 0)
        {
            print("Destroy child");
            for (int i = 0; i < mostDayBox.transform.childCount; i++)
            {
                Destroy(mostDayBox.transform.GetChild(i).gameObject);
            }
        }

        if (lastweek.mostDayCount == 0)
        {
            mostDayInstance = (GameObject)Instantiate(mostDayPrefab, mostDayBox.transform);
            mostDayInstance.transform.SetParent(mostDayBox.transform);
            mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Black;
            mostDayInstance.GetComponent<TMP_Text>().text = "-";
            mostDayInstance.GetComponent<TMP_Text>().fontSize = 36;
            mostDayCount.text = "0\nnote";
        }
        else if (lastweek.mostDayCount == 7)
        {
            mostDayInstance = (GameObject)Instantiate(mostDayPrefab, mostDayBox.transform);
            mostDayInstance.transform.SetParent(mostDayBox.transform);
            mostDayInstance.GetComponent<TMP_Text>().text = "You take notes in everyday";
            mostDayInstance.GetComponent<TMP_Text>().fontSize = 36;
        }
        else
        {
            int textSize = 54 - (6 * (lastweek.mostDayCount - 1));

            foreach (string day in lastweek.days)
            {
                if (day == null || day.Equals(""))
                {
                    continue;
                }

                mostDayInstance = (GameObject)Instantiate(mostDayPrefab, mostDayBox.transform);
                mostDayInstance.transform.SetParent(mostDayBox.transform);

                switch (day)
                {
                    case "Sunday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Sun; break;
                    case "Monday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Mon; break;
                    case "Tuesday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Tue; break;
                    case "Wednesday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Wed; break;
                    case "Thursday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Thu; break;
                    case "Friday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Fri; break;
                    case "Saturday":
                        mostDayInstance.GetComponent<TMP_Text>().color = MyColors.Sat; break;
                }

                mostDayInstance.GetComponent<TMP_Text>().text = day;
                mostDayInstance.GetComponent<TMP_Text>().fontSize = textSize;
            }

            if (lastweek.mostDayCount != 1)
            {
                mostDayCount.text = $"{lastweek.mostDayCount}\nnotes";
            }
            else
            {
                mostDayCount.text = "1\nnote";
            }
        }
    }
}

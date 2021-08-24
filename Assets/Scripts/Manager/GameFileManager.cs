using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameFileManager : MonoBehaviour
{
    public static GameFileManager instance;

    public static string ICE_GAME = "ice", BREATH_GAME = "breath";

    public string GameName { get; set; }
    public int Level { get; set; }

    private int id;
    private string gameStatsPath, totalGamePath, icePath;

    private List<Game> gameList;
    public List<Game> GetGameList()
    {
        return gameList;
    }

    private List<GameTotal> totalList;
    public List<GameTotal> GetTotalList()
    {
        return totalList;
    }

    public IceData IceData { get; set; }

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

        gameStatsPath = Path.Combine(Application.persistentDataPath, "games.json");
        totalGamePath = Path.Combine(Application.persistentDataPath, "game_count.json");

        icePath = Path.Combine(Application.persistentDataPath, "ice_game.json");

        GameName = "";

        //print("GameFileManager Awake. Read game stats.");
        ReadGameStats();
        ReadPlayerData();
    }

    public void IncreasetId()
    {
        id++;
    }

    public void ResetWeekly()
    {
        foreach (GameTotal gameTotal in totalList)
        {
            gameTotal.weekly = 0;
            gameTotal.weeklyWithAnger = 0;
            gameTotal.weeklyBetter = 0;
        }

        WriteTotalGame();
    }

    private void ReadPlayerData()
    {
        if (File.Exists(icePath))
        {
            string iceContent = File.ReadAllText(icePath);

            if (iceContent.Equals(""))
            {
                IceData = new IceData
                {
                    img1 = false,
                    img2 = false,
                    img3 = false
                };
            }
            else
            {
                IceData = JsonUtility.FromJson<IceData>(iceContent);
            }
        }
        else
        {
            File.Create(icePath).Close();
            ReadPlayerData();
        }
    }

    private void ReadGameStats()
    {
        if (File.Exists(gameStatsPath))
        {
            //print("Files exist");
            string content = File.ReadAllText(gameStatsPath);
            string totalContent = File.ReadAllText(totalGamePath);
            
            // File created for the 1st time.
            if (content.Equals(""))
            {
                gameList = new List<Game>();
                totalList = new List<GameTotal>
                {
                    new GameTotal { name = ICE_GAME },
                    new GameTotal { name = BREATH_GAME }
                };

                id = 1;
            }
            // File already has data.
            else
            {
                GameCollection gameCollection = JsonUtility.FromJson<GameCollection>(content);
                gameList = new List<Game>(gameCollection.games);

                id = gameList[gameList.Count - 1].id;
                id++;

                GameTotalCollection gameTotalCollection = JsonUtility.FromJson< GameTotalCollection>(totalContent);
                totalList = new List<GameTotal>(gameTotalCollection.total);
            }
        }
        else
        {
            //print("Files note exist, creating...");
            File.Create(gameStatsPath).Close();
            File.Create(totalGamePath).Close();
            ReadGameStats();
        }

        //print("Next game ID : " + id);
        //print("gameList Count : " + gameList.Count);
    }

    public void SaveIce(int imageNumber)
    {
        switch (imageNumber)
        {
            case 1:
                if (IceData.img1 == false) IceData.img1 = true;
                break;
            case 2:
                if (IceData.img2 == false) IceData.img2 = true;
                break;
            case 3:
                if (IceData.img3 == false) IceData.img3 = true;
                break;
        }

        WriteIceData();
    }

    public void SaveGame()
    {
        totalList[totalList.FindIndex(t => t.name == GameName)].total++;
        totalList[totalList.FindIndex(t => t.name == GameName)].weekly++;

        WriteTotalGame();
    }

    public void SaveNewGame(int levelAfter)
    {
        DateTime curr = DateTime.Now;

        Game newGame = new Game
        {
            id = this.id,
            name = GameName,
            levelBefore = Level,
            levelAfter = levelAfter,
            times = 1,
            day = curr.Day,
            month = curr.Month,
            year = curr.Year,
            hour = curr.Hour,
            minute = curr.Minute,
        };

        gameList.Add(newGame);

        totalList[totalList.FindIndex(t => t.name == GameName)].total++;
        totalList[totalList.FindIndex(t => t.name == GameName)].weekly++;

        totalList[totalList.FindIndex(t => t.name == GameName)].totalWithAnger++;
        totalList[totalList.FindIndex(t => t.name == GameName)].weeklyWithAnger++;

        if (levelAfter > Level)
        {
            totalList[totalList.FindIndex(t => t.name == GameName)].totalBetter++;
            totalList[totalList.FindIndex(t => t.name == GameName)].weeklyBetter++;
        }

        WriteGameStats();
        WriteTotalGame();
    }

    public void UpdateLastGamePlayTimes(int times)
    {
        gameList[gameList.Count - 1].times = times;
        WriteGameStats();
    }

    private void WriteTotalGame()
    {
        // Write to "total_games.json"
        GameTotalCollection totalCollection = new GameTotalCollection
        {
            total = totalList.ToArray()
        };
        string jsonString = JsonUtility.ToJson(totalCollection, true);
        File.WriteAllText(totalGamePath, jsonString, System.Text.Encoding.UTF8);
    }

    private void WriteGameStats()
    {
        print("Saved game ID : " + gameList[gameList.Count - 1].id);

        // Write to "game_stats.json"
        GameCollection gameCollection = new GameCollection
        {
            games = gameList.ToArray()
        };

        string jsonString = JsonUtility.ToJson(gameCollection, true);
        File.WriteAllText(gameStatsPath, jsonString, System.Text.Encoding.UTF8);
    }

    private void WriteIceData()
    {
        // Write to "ice_data.json"
        string jsonString = JsonUtility.ToJson(IceData, true);
        File.WriteAllText(icePath, jsonString, System.Text.Encoding.UTF8);
    }
}

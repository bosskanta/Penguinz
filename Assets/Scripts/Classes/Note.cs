[System.Serializable]
public class Note
{
    public string id;
    public int levelBefore, levelAfter; 
    public string text;
    public int day, month, year, hour, minute;
    public bool hasAudio;
}

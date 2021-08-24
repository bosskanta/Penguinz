using UnityEngine;
public static class MyColors
{
    private static byte maxAlpha = 255;

    public static Color32 NoteTitle = new Color32(79, 79, 79, 255);

    public static Color32 White = new Color32(255, 255, 255, maxAlpha);
    public static Color32 WhiteAlpha = new Color32(255, 255, 255, 128);

    public static Color32 Black = new Color32(0, 0, 0, maxAlpha);
    public static Color32 Gray = new Color32(0, 0, 0, 127);

    // Calendar
    public static Color32 ZeroAlpha = new Color32(0, 0, 0, 0); // No Color
    public static Color32 HaveNote = new Color32(51, 161, 253, maxAlpha); // Blue Jeans
    public static Color32 SelectedDay = new Color32(132, 220, 198, maxAlpha); // Middle Blue Green
    public static Color32 CalendarBG = new Color32(28, 50, 89, maxAlpha); // Space Cadet

    public static Color32 CaribbeanGreen = new Color32(6, 214, 160, 255); //Caribbean Green
    public static Color32 Platinum = new Color32(229, 229, 229, 255); //Platinum
    public static Color32 DavysGray = new Color32(79, 79, 79, 255); //Davys Gray, General text color

    public static Color32 OnChooseBox = new Color32(0, 107, 166, 200);
    public static Color32 IceGameBorder = new Color32(109, 132, 222, 140);
    public static Color32 BreathGameBorder = new Color32(33, 137, 126, 140);

    public static Color32 Furious = new Color32(255, 0, 0, maxAlpha);
    public static Color32 Angry = new Color32(255, 69, 0, maxAlpha);
    public static Color32 Annoy = new Color32(255, 165, 0, maxAlpha);

    public static Color32 Sun = new Color32(255, 0, 0, maxAlpha);
    public static Color32 Mon = new Color32(255, 255, 0, maxAlpha);
    public static Color32 Tue = new Color32(255, 192, 203, maxAlpha);
    public static Color32 Wed = new Color32(0, 128, 0, maxAlpha);
    public static Color32 Thu = new Color32(255, 165, 0, maxAlpha);
    public static Color32 Fri = new Color32(0, 191, 255, maxAlpha);
    public static Color32 Sat = new Color32(128, 0, 128, maxAlpha);
}

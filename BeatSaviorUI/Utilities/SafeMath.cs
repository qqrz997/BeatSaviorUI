namespace BeatSaviorUI.Utilities;

internal static class SafeMath
{
    public static float Divide(float top, float bottom) =>
        bottom == 0 ? 0 : top / bottom;

    public static float Average(float top, int topCount, float bot, int botCount) =>
        !float.IsNaN(top) && !float.IsNaN(bot) ? (top * topCount + bot * botCount) / (topCount + botCount) 
        : float.IsNaN(bot) ? top
        : bot;
}
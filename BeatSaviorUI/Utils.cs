using UnityEngine;

namespace BeatSaviorUI
{
	internal static class SafeMath
	{
		public static float Divide(float top, float bottom) =>
			bottom == 0 ? 0 : top / bottom;

		public static float Average(float top, int topCount, float bot, int botCount) =>
			!float.IsNaN(top) && !float.IsNaN(bot) ? (top * topCount + bot * botCount) / (topCount + botCount) 
			: float.IsNaN(bot) ? top
			: bot;
	}
	
	internal static class Utils
	{
		public static System.Random Random { get; } = new();

		public static float[] FloatArrayFromVector(Vector3 v)
		{
			return [v.x, v.y, v.z];
		}

		public static int MaxRawScoreForNumberOfNotes(int noteCount)
		{
			int num = 0;
			int i;
			for (i = 1; i < 8; i *= 2)
			{
				if (noteCount < i * 2)
				{
					num += i * noteCount;
					noteCount = 0;
					break;
				}
				num += i * i * 2 + i;
				noteCount -= i * 2;
			}
			num += noteCount * i;
			return num * 115;
		}
	}
}

using UnityEngine;

namespace BeatSaviorUI.Utilities
{
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

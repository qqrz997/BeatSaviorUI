﻿using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace BeatSaviorData.HarmonyPatches
{
	[HarmonyPatch(typeof(SaberSwingRatingCounter), "ProcessNewData")]
	public static class SaberSwingRatingCounterPatches
	{
		private static FieldInfo afterCutRating = typeof(SaberSwingRatingCounter).GetField("_afterCutRating", BindingFlags.NonPublic | BindingFlags.Instance);

		public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> tmp = instructions.ToList();

			List<CodeInstruction> codeFirst = new List<CodeInstruction>()
			{
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Call, SwingTranspilerHandler.PreparePreswingMethodInfo)
			};

			List<CodeInstruction> codeSecond = new List<CodeInstruction>()
			{
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldarg_0),
				new CodeInstruction(OpCodes.Ldfld, afterCutRating),
				new CodeInstruction(OpCodes.Call, SwingTranspilerHandler.AddPostswingMethodInfo)
			};

			tmp.InsertRange(166, codeSecond);		// 157
			tmp.InsertRange(114, codeFirst);		// 123

			return tmp;
		}
	}
}

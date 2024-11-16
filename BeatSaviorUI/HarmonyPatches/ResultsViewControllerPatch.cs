using HarmonyLib;

namespace BeatSaviorUI.HarmonyPatches
{
	[HarmonyPatch(typeof(ResultsViewController), "SetDataToUI")]
	class ResultsViewControllerPatches
	{
		static void Postfix(ref ResultsViewController __instance)
		{
			// Create or refresh end of song UI
			EndOfLevelUICreator.Create();
		}
	}
}
